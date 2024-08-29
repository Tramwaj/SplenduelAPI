using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Splenduel.Interfaces.Services;
using Splenduel.Interfaces.VMs;
namespace SplenduelAPI.Hubs
{
    public class GameHub : Hub
    {
        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Message", "Connected successfully!");
        }
        public async Task SubscribeToGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("ReceiveMessage",$"{Context.User?.Identity?.Name ?? "Unknown user "} Connected successfully!");
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendCoinBoard(CoinBoardVM coinBoard, string gameId)
        {
            await Clients.Group(gameId).SendAsync("ReceiveCoinBoard", coinBoard);
        }

    }
}
