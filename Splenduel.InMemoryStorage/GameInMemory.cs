using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Store;
using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.InMemoryStorage
{
    public class GameInMemory : IGameStore
    {
        private Dictionary<Guid, GameState> _gameState = new();
        public async Task<DefaultResponse> CreateGame(GameState gameState)
        {
            var id = gameState.GameId;
            if (id == Guid.Empty || id == null) return DefaultResponse.Nok("Game_id is not proper");
            if (_gameState.ContainsKey(id)) return DefaultResponse.Nok("Game_id is already in database!");
            _gameState.Add(id, gameState);
            return DefaultResponse.ok;
        }

        public async Task<string[]> GetAllCardsCsvFromLevel(int lvl)
        {
            return await CardsImporter.GetAllFromLevel(lvl);
        }

        public async Task<GameState> GetGameState(Guid id)
        {
            if (!_gameState.ContainsKey(id)) return null;

            var gamestate = _gameState[id];
            return gamestate;

        }

        public async Task<DefaultResponse> UpdateGame(GameState gameState)
        {
            if (!_gameState.ContainsKey(gameState.GameId)) return DefaultResponse.Nok("GameID not found");
            _gameState[gameState.GameId] = gameState;
            return DefaultResponse.ok;
        }
    }
}
