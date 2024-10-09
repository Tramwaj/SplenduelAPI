using Microsoft.AspNetCore.SignalR;
using Splenduel.Interfaces.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Interfaces.Services
{
    public interface IGameInfoSender
    {
        public Task SendCoinBoard(CoinBoardVM coinBoard, string gameId);
        public Task SendPlayerBoard(PlayerBoardVM playerBoard, string gameId);
        //public Task SendNobleBoard(NobleBoardVM nobleBoard, string gameId);
        public Task SendCardLevel(CardLevelVM cardLevel,int level, string gameId);
        public Task SendEndTurnMessage(string gameId);
        public Task SendActionStatus(string gameId,string status, string message);
        public Task SendPersonalActionStatus(string name, string status, string message);
        Task SendNobles(NobleVM[] nobles, string gameId);
    }
}
