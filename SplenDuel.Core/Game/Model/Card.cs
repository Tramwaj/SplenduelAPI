using System.ComponentModel;
using System.Net.NetworkInformation;

namespace Splenduel.Core.Game.Model
{
    public class Card
    {
        public Card()
        {
            
        }
        public Card(int id, int level, ColourEnum colour, int points, int miningPower, int crowns, CardCost cost, CardActionEnum cardAction= CardActionEnum.None)
        {
            Id = id;
            Level = level;
            Colour = colour;
            Points = points;
            MiningPower = miningPower;
            Crowns = crowns;
            Cost = cost;
            Action = cardAction;
        }
        static Card Blank()
        {
            return new Card(0,0, ColourEnum.Grey, 0, 0, 0, null);

        }
        public bool IsEqualTo(Card card)
        {
            return Level==card.Level && Points==card.Points && MiningPower==card.MiningPower && Crowns==card.Crowns && Cost==card.Cost && Action==card.Action;
        }
        public override string ToString()
        {
            return $"Card: {Colour}:{MiningPower}, {Points}p, {Crowns}c  {(Action!=CardActionEnum.None?Action:"")}";
            //todo: complete card.tostring
        }

        public int Id { get; set; }
        public int Level { get; set; }
        public ColourEnum Colour { get; set; }
        public int Points { get; set; }
        public int MiningPower { get; set; }
        public int Crowns { get; set; }
        public CardCost Cost { get; set; }
        public CardActionEnum Action { get; set; }
    }
}
