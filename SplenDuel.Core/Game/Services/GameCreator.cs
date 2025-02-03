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
                board.Player1Board.Player = player1;
                board.Player2Board.Player = player2;
                return new GameState(id, true, board, lastAction);
            }
            catch
            {
                throw;
            }
        }
        private Card GiveCardRandomAction(Card card)
        {
            var random = new Random();
            var action = (CardActionEnum)random.Next(0, 4);
            card.Action = action;
            return card;
        }

        private async Task<Board> CreateBoard()
        {
            try
            {
                var lvl1Cards = (await _gameStore.GetAllCardsCsvFromLevel(1)).Select((x, i) => CardFromCsvReader.Read(x, i, 1)).ToList();
                    //.Select(c => GiveCardRandomAction(c)).ToList();
                var cardLevel1 = new CardLevel(lvl1Cards, 5);
                var lvl2Cards = (await _gameStore.GetAllCardsCsvFromLevel(2)).Select((x, i) => CardFromCsvReader.Read(x, i + 100, 2)).ToList();
                var cardLevel2 = new CardLevel(lvl2Cards, 4);
                var lvl3Cards = (await _gameStore.GetAllCardsCsvFromLevel(3)).Select((x, i) => CardFromCsvReader.Read(x, i + 200, 3)).ToList();
                var cardLevel3 = new CardLevel(lvl3Cards, 3);
                var coinBoard = new CoinBoard(2);
                var player1Board = new PlayerBoard(false);
                var cost = new CardCost(0, 0, 0, 0, 0, 0);
                //await player1Board.BuyCard(new Card(500, 3, ColourEnum.Blue, 0, 3, 0, cost, CardActionEnum.None));
                //await player1Board.BuyCard(new Card(500, 3, ColourEnum.White, 0, 3, 0, cost, CardActionEnum.None));
                //await player1Board.BuyCard(new Card(500, 3, ColourEnum.Red, 0, 3, 0, cost, CardActionEnum.None));
                //await player1Board.BuyCard(new Card(500, 3, ColourEnum.Black, 0, 3, 0, cost, CardActionEnum.None));
                //await player1Board.BuyCard(new Card(500, 3, ColourEnum.Green, 0, 3, 0, cost, CardActionEnum.None));
                //await player1Board.AddCoins(new List<ColourEnum> { ColourEnum.Pink, ColourEnum.Pink });
                var player2Board = new PlayerBoard(true);
                //await player2Board.BuyCard(new Card(500, 3, ColourEnum.Blue, 0, 3, 0, cost, CardActionEnum.None));
                //await player2Board.BuyCard(new Card(500, 3, ColourEnum.White, 0, 3, 0, cost, CardActionEnum.None));
                //await player2Board.BuyCard(new Card(500, 3, ColourEnum.Red, 0, 3, 0, cost, CardActionEnum.None));
                //await player2Board.BuyCard(new Card(500, 3, ColourEnum.Black, 0, 3, 0, cost, CardActionEnum.None));
                //await player2Board.BuyCard(new Card(500, 3, ColourEnum.Green, 0, 3, 0, cost, CardActionEnum.None));
                //await player2Board.AddCoins(new List<ColourEnum> { ColourEnum.Pink, ColourEnum.Pink });
                Noble[] nobles = [new Noble(2, CardActionEnum.Scroll),
                    new Noble(2, CardActionEnum.ExtraTurn),
                    new Noble(2, CardActionEnum.Steal),
                    new Noble(3, CardActionEnum.None)];
                return new Board(cardLevel1, cardLevel2, cardLevel3, coinBoard, player1Board, player2Board, nobles);
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
