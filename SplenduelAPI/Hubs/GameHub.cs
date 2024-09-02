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
        public async Task SendPlayerBoard(PlayerBoardVM playerBoard, string playerName, string gameId)
        {
            await Clients.Group(gameId).SendAsync("ReceivePlayerBoard", playerBoard, playerName);
        }
        public async Task SendCardLevel(CardLevelVM cardLevel, int number, string gameId)
        {
            await Clients.Group(gameId).SendAsync("ReceiveCardLevel", cardLevel, number);
        }
        public async Task SendEndTurnMessage(string gameId)
        {
            await Clients.Group(gameId).SendAsync("ReceiveEndTurnMessage");
        }
        public async Task SendActionStatus(string gameId, string message,string dupa)
        {
            await Clients.Group(gameId).SendAsync("ReceiveActionStatus", message, dupa);
        }

    }
}
