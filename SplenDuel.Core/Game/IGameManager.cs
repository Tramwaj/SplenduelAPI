using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game
{
    public interface IGameManager
    {
        Task<DefaultResponse> CreateGame(Guid id, string player1, string player2);
    }
}
