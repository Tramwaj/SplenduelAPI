using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Store;
using Splenduel.Core.Global;
using Splenduel.Core.Mappers;
using Splenduel.Interfaces.DTOs;
using Splenduel.Interfaces.VMs;
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
        private ActionResolver _actionResolver;

        public GameManager(IGameStore store, GameCreator gameCreator, ActionResolver actionResolver)
        {
            _store = store;
            _gameCreator = gameCreator;
            _actionResolver = actionResolver;
        }

        public async Task<GameStateVM> GetGameData(Guid id, string PlayerName)
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
            return (await gameState.PerPlayer(PlayerName)).MapToVM();
        }

        public async Task<GameStateVM> ResolveAction(ActionDTO action, string playerName)
        {
            var previousGameState = await _store.GetGameState(action.GameId);
            GameState NewGameState = null;
            try
            {
                if (previousGameState == null) throw new ApplicationException("Game not found");
                if (previousGameState.ActivePlayerName != playerName) throw new ApplicationException("Not your turn");
                var newGameState = _actionResolver.ResolveAction(action, previousGameState, playerName);
                return null;// (await newGameState.PerPlayer(playerName)).MapToVM();
            }
            catch
            {
                throw;
            }
        }
    }
}
