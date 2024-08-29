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

    }
}
