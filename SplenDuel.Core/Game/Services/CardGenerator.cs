using Splenduel.Core.Game.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Services
{
    public class CardGenerator
    {
        private List<Card> _level1Deck = new List<Card>
        {
            new Card(0,1, ColourEnum.Red, 0, 1, 0, new CardCost(0,0,0,0,3,0)),
            new Card(1,1, ColourEnum.Green, 0,1,0,new CardCost(0,0,0,2,2,1)),
            new Card(2,1, ColourEnum.Green, 0, 1,0,new CardCost(1,1,0,1,1,0)),
            new Card(3,1, ColourEnum.Grey, 3,0,0,new CardCost(0,0,0,4,0,1)),
            new Card(4,1, ColourEnum.Red, 0, 1, 0, new CardCost(1,1,1,0,1,0)),
            new Card(5,1, ColourEnum.Red, 0, 1, 0, new CardCost(2,0,0,0,2,1 ))
        };
        private List<Card> _level1Exposed = new();
        public CardGenerator()
        {
            SetStartingCards();
        }
        private void SetStartingCards()
        {
            for (int i = 0; i < 3; i++)
            {
                int rand = Random.Shared.Next(_level1Deck.Count);
                _level1Exposed.Add(_level1Deck[rand]);
                _level1Deck.RemoveAt(rand);
            }
        }

        public ICollection<Card> GetBoard()
        {
            return new List<Card>(_level1Exposed);
        }
        public ICollection<Card> GetLevel1Exposed()
        {
            return new List<Card>(_level1Exposed);
        }
        public Card GetCard(int index)
        {
            return _level1Exposed[index];
        }
        //smells a bit..tbsl
        public ICollection<Card> TakeCard(int index)
        {
            _level1Exposed.RemoveAt(index);
            if (_level1Deck.Count > 0)
            {
                int rand = Random.Shared.Next(_level1Deck.Count);
                _level1Exposed.Insert(index, _level1Deck[rand]);
                _level1Deck.RemoveAt(rand);
            }
            else _level1Exposed.Insert(index, null);
            return new List<Card>(_level1Exposed);
        }

        public int GetDeckCount()
        {
            return _level1Deck.Count();
        }

    }
}
