using Splenduel.Core.Game.Model;
using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Store
{
    public interface IGameStore
    {
        public Task<GameState> GetGameState(Guid id);
        public Task<DefaultResponse> CreateGame(GameState gameState);
        public Task<DefaultResponse> UpdateGame(GameState gameState);
        public Task<string[]> GetAllCardsCsvFromLevel(int lvl);
    }
}
