using System.Xml;

namespace Splenduel.Core.Game.Model
{
    public class CardCost
    {
        private IDictionary<ColourEnum, int> _costDictionary;

        public int White => TryGetColourCost(ColourEnum.White);
        public int Blue => TryGetColourCost(ColourEnum.Blue);
        public int Green => TryGetColourCost(ColourEnum.Green);
        public int Red => TryGetColourCost(ColourEnum.Red);
        public int Black => TryGetColourCost(ColourEnum.Black);
        public int Pink => TryGetColourCost(ColourEnum.Pink);
        public IDictionary<ColourEnum, int> CostDictionary => _costDictionary;
        public CardCost()
        {

        }
        public CardCost(int White, int Blue, int Green, int Red, int Black, int Pink)
        {
            _costDictionary = new Dictionary<ColourEnum, int>();
            if (White != 0) { _costDictionary.Add(ColourEnum.White, White); }
            if (Blue != 0) { _costDictionary.Add(ColourEnum.Blue, Blue); }
            if (Green != 0) { _costDictionary.Add(ColourEnum.Green, Green); }
            if (Red != 0) { _costDictionary.Add(ColourEnum.Red, Red); }
            if (Black != 0) { _costDictionary.Add(ColourEnum.Black, Black); }
            if (Pink != 0) { _costDictionary.Add(ColourEnum.Pink, Pink); }
        }

        private int TryGetColourCost(ColourEnum colour)
        {
            int value;
            if (_costDictionary.TryGetValue(colour, out value))
                return value;
            return 0;
        }

    }
}

