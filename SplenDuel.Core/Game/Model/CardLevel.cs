using System.Runtime.InteropServices.Marshalling;

namespace Splenduel.Core.Game.Model
{
    public class CardLevel
    {
        private readonly Random _random = new Random();
        private Card[] _exposed;
        private ICollection<Card> _deck;

        public Card?[] Exposed => _exposed;
        public int DeckCount => _deck.Count;
        public CardLevel(ICollection<Card> cards, int exposedCount)
        {
            _exposed = new Card[exposedCount];
            _deck = cards;
            for (int i = 0; i < exposedCount; i++)
            {
                _exposed[i] = DrawCardFromDeck();
            }
        }
        public CardLevel(Card[] exposed, ICollection<Card> deck)
        {
            this._exposed = exposed;
            if (deck == null || deck.Count == 0)
                deck = new List<Card>();
            this._deck = deck.ToList();
        }
        public CardLevel(){}

        public Card? TakeCard(int position, out Card? cardDrawn) //obsolete?
        {
            var cardTaken = _exposed[position];
            cardDrawn = DrawCardFromDeck();
            _exposed[position] = cardDrawn;
            return cardTaken;
        }
        public void TakeCardById(int cardId)
        {
            int position = Array.FindIndex(_exposed, x => x.Id == cardId);
            var cardDrawn = DrawCardFromDeck();
            _exposed[position] = cardDrawn;
        }
        //todo: tests
        public Card? DrawCardFromDeck()
        {
            if (_deck.Count == 0) return new Card();
            var cardDrawn = _deck.ElementAt(_random.Next(_deck.Count));
            _deck.Remove(cardDrawn);
            return cardDrawn;
        }
    }
}
