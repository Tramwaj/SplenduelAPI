using Splenduel.Core.Auth.Store;
using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Store;
using Splenduel.Core.Global;
using Splenduel.Core.Home.Model;
using Splenduel.Core.Home.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Services
{
    public class GameCreator
    {
        private IUserStore _userStore;
        private IHomeStore _homeStore;
        private IGameStore _gameStore;

        public GameCreator(IUserStore userStore, IHomeStore homeStore, IGameStore gameStore)
        {
            _userStore = userStore;
            _homeStore = homeStore;
            _gameStore = gameStore;
        }
        public async Task<GameState> CreateNewGame(Guid id)
        {
            var duel = await _homeStore.GetMatch(id);
            if (duel == null) throw new ApplicationException("Game hasn't been created by home manager");

            var player1 = await GetPlayer(duel.Player1);
            if (player1 == null) throw new ApplicationException($"{duel.Player1} not found");
            var player2 = await GetPlayer(duel.Player2);
            if (player2 == null) throw new ApplicationException($"{duel.Player2} not found");

            string lastAction = "CreateGame";
            try
            {
                Board board = await CreateBoard();
                return new GameState(id, true, board, player1, player2, lastAction);
            }
            catch
            {
                throw;
            }
        }

        private async Task<Board> CreateBoard()
        {
            try
            {
                var lvl1Cards = (await _gameStore.GetAllCardsCsvFromLevel(1)).Select((x, i) => CardFromCsvReader.Read(x, i)).ToList();
                var cardLevel1 = new CardLevel(lvl1Cards, 5);
                var lvl2Cards = (await _gameStore.GetAllCardsCsvFromLevel(2)).Select((x, i) => CardFromCsvReader.Read(x, i)).ToList();
                var cardLevel2 = new CardLevel(lvl2Cards, 4);
                var lvl3Cards = (await _gameStore.GetAllCardsCsvFromLevel(3)).Select((x, i) => CardFromCsvReader.Read(x, i)).ToList();
                var cardLevel3 = new CardLevel(lvl3Cards, 3);
                var coinBoard = new CoinBoard(3);
                var player1Board = new PlayerBoard(true);
                var player2Board = new PlayerBoard(false);
                return new Board(cardLevel1, cardLevel2, cardLevel3, coinBoard, player1Board, player2Board);
            }
            catch
            {
                throw;
            }
        }

        private async Task<Player?> GetPlayer(string player)
        {
            string p1Name = player;
            Guid p1id = await _userStore.GetUserId(p1Name) ?? Guid.Empty;
            if (p1id == Guid.Empty) return null;
            return new Player(p1Name, p1id);

        }
    }
}
