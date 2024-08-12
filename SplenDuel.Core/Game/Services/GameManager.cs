using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Store;
using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Services
{
    public class GameManager
    {
        private IGameStore _store;
        private GameCreator _gameCreator;

        public GameManager(IGameStore store, GameCreator gameCreator)
        {
            _store = store;
            _gameCreator = gameCreator;
        }

        public async Task<GameState> GetGameData(Guid id, string PlayerName)
        {
            var gameState = await _store.GetGameState(id);
            if (gameState == null)
            {
                try
                {
                    gameState = await _gameCreator.CreateNewGame(id);
                    var response = await _store.CreateGame(gameState);
                    if (!response.Success) throw new ApplicationException(response.Error);
                }
                catch
                {
                    throw;
                }
            }

            return await gameState.PerPlayer(PlayerName);
        }
    }
}
