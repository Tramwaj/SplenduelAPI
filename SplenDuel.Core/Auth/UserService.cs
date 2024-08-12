using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Splenduel.Core.Auth.Store;
using Splenduel.Core.Auth.Interfaces;

namespace Splenduel.Core.Auth
{
    public class UserService : IUserService
    {
        //private readonly IConfiguration _configuration;
        private readonly IUserStore _store;
        public UserService(/**IConfiguration configuration, **/IUserStore store)
        {
            //_configuration = configuration;
            _store = store;

            var hashedPass = BCrypt.Net.BCrypt.HashPassword("123");
            var mgc = new User { Id = Guid.NewGuid(), UserName = "mgc", Password= hashedPass };
            _store.AddUser(mgc);
            var dzo = new User { Id = Guid.NewGuid(), UserName = "dzo", Password= hashedPass };
            _store.AddUser(dzo
                );
        }
        public async Task<LoginData> Login(string username, string password)
        {
            if (!IsAnyInputEmpty(username, password)) return new LoginData("Error: username or password is empty","");     
            
            var user = await TryGetUser(username, password);
            if (user == null) return new LoginData("Error: Bad login or username","");
            string token = await GetToken(user);
            return new LoginData(token,user.UserName);
        }

        #region private methods
        private async Task<User?> TryGetUser(string username, string password)
        {
            var user = await _store.GetUser(username);
            if (user == null) return null;
            if (!VerifyPassword(password, user.Password)) return null;
            return user;
        }

        private bool VerifyPassword(string inputPassword, string dbPassword)
        {
            if (BCrypt.Net.BCrypt.Verify(inputPassword, dbPassword))
                return true;
            return false;
        }

        private bool IsAnyInputEmpty(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;
            else return true;
        }
        private async Task<string> GetToken(User user)
        {
            //todo: read from appsettings
            //configuratoin.getsection lub 4x configuration.get
            var jwtSettings = new JwtSettings();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "User") }),
                Expires = DateTime.UtcNow.AddSeconds(jwtSettings.LifetimeInSeconds),
                Issuer = jwtSettings.ValidIssuer,
                Audience = jwtSettings.ValidAudience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
