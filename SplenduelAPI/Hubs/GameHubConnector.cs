using Microsoft.AspNetCore.SignalR;
using Splenduel.Interfaces.Services;
using Splenduel.Interfaces.VMs;

namespace SplenduelAPI.Hubs
{
    public class GameHubConnector : IGameInfoSender
    {
        private readonly IHubContext<GameHub> _hubContext;

        public GameHubConnector(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendCoinBoard(CoinBoardVM coinBoard, string gameId)
        {
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveCoinBoard", coinBoard);
        }

        public async Task SendPlayerBoard(PlayerBoardVM playerBoard, string gameId)
        {
            await _hubContext.Clients.Group(gameId).SendAsync("ReceivePlayerBoard", playerBoard);
        }

        public async Task SendCardLevel(CardLevelVM cardLevel, int level, string gameId)
        {
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveCardLevel", cardLevel, level);
        }

        public async Task SendEndTurnMessage(string gameId)
        {
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveEndTurnMessage");
        }
        public async Task SendActionStatus(string gameId, string status, string message)
        {
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveActionStatus", status, message);
        }
        public async Task SendPersonalActionStatus(string name, string status, string message)
        {
            await _hubContext.Clients.User(name).SendAsync("ReceivePersonalActionStatus", status, message);
        }
    }
}
