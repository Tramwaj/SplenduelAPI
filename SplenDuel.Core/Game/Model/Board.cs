
namespace Splenduel.Core.Game.Model
{
    public class Board
    {
        public CardLevel Level1 { get; set; }
        public CardLevel Level2 { get; set; }
        public CardLevel Level3 { get; set; }
        public CardLevel[] AllLevels => new CardLevel[] { Level1, Level2, Level3 };
        public CoinBoard CoinBoard { get; set; }
        public PlayerBoard Player1Board { get; set; }
        public PlayerBoard Player2Board { get; set; }

        public Board(CardLevel level1, CardLevel level2, CardLevel level3, CoinBoard coinBoard, PlayerBoard player1Board, PlayerBoard player2Board)
        {
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
            this.CoinBoard = coinBoard;
            Player1Board = player1Board;
            Player2Board = player2Board;
        }

        internal CardLevel GetCardLevel(int cardId)
        {
            if (cardId == 0) return null;
            if (Level1.Exposed.Any(x=>x.Id==cardId)) return Level1;
            if (Level2.Exposed.Any(x => x.Id == cardId)) return Level2;
            if (Level3.Exposed.Any(x => x.Id == cardId)) return Level3;
            return null;
        }
    }
}
