using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Global
{
    public enum ActionType
    {
        BuyCard,
        ReserveCard,
        TakeCoins,
        FillCoinBoard
    }
    public interface IAction
    {
        public ActionType Type { get; }
        public Task<DefaultResponse> Execute();


    }
}
