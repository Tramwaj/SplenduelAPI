using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Splenduel.Core.Auth;
using Splenduel.Core.Auth.Interfaces;
using Splenduel.Core.Auth.Store;
using Splenduel.Core.Game.Services;
using Splenduel.Core.Game.Store;
using Splenduel.Core.Home;
using Splenduel.Core.Home.Interfaces;
using Splenduel.Core.Home.Store;
using Splenduel.InMemoryStorage;
using Splenduel.Interfaces.Services;
using SplenduelAPI.Hubs;
using System.Text;
using TextStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//todo: setup cors properly
builder.Services.AddCors(options => options
            .AddPolicy("AllowLocalhost3000", policy => policy.WithOrigins("http://localhost:3000")
                                               .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              .AllowCredentials()));
//.WithOrigins("http://localhost:3000")));

//todo: configure jwt properly
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:ValidIssuer"),
                        ValidAudience = builder.Configuration.GetValue<string>("JwtSettings:ValidAudience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Secret")!)),
                        SaveSigninToken = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/gameHub")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IHomeManager, HomeManager>();
builder.Services.AddTransient<GameManager, GameManager>();
builder.Services.AddTransient<GameCreator, GameCreator>();
builder.Services.AddSingleton<IHomeStore, HomeInMemory>();
builder.Services.AddTransient<IGameInfoSender, GameHubConnector>();
builder.Services.AddSingleton<IUserStore, UserInMemory>();
builder.Services.AddSingleton<IGameStore, GameInMemory>();

//builder.Services.AddSingleton<INotificationSink, NotificationService> ();
//builder.Services.AddHostedService(sp => (NotificationService)sp.GetService<INotificationSink>());
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
   // app.UseSwagger();
    //app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.UseExceptionHandler( fgh fhfgh fgh);

//app.UseWebSockets();

app.Run();

