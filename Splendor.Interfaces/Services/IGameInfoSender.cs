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
        public Task SendPlayerBoard(PlayerBoardVM playerBoard,string playerName, string gameId);
        //public Task SendNobleBoard(NobleBoardVM nobleBoard, string gameId);
    }
}
