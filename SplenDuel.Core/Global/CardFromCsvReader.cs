using Splenduel.Core.Game.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Global
{
    public static class CardFromCsvReader
    {
        public static Card Read(string cardCsv, int id, int level)
        {
            string[] props = cardCsv.Split(';');
            Enum.TryParse(props[0], out ColourEnum cardcolour);
            int miningPower = CsvValueToInt(props[1]);
            var cardCost = new CardCost(CsvValueToInt(props[2]),
                                        CsvValueToInt(props[3]),
                                        CsvValueToInt(props[4]),
                                        CsvValueToInt(props[5]),
                                        CsvValueToInt(props[6]),
                                        CsvValueToInt(props[7]));
            int points = CsvValueToInt(props[8]);
            int crowns = CsvValueToInt(props[9]);
            CardActionEnum action = CardActionEnum.None;
            if (!string.IsNullOrEmpty(props[10])) 
                if (!Enum.TryParse(props[10], out action))
                    action = CardActionEnum.None;
            return new Card(id, level, cardcolour, points, miningPower, crowns, cardCost, action);
        }
        private static int CsvValueToInt(string val)
        {
            if (string.IsNullOrEmpty(val)) return 0;
            return int.Parse(val);
        }
    }
}
