using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Model
{
    public class CoinRequest
    {
        public int i;
        public int j;
        public ColourEnum colour;

        public CoinRequest()
        {
            
        }
        public CoinRequest(int i, int j, ColourEnum colour)
        {
            this.i = i;
            this.j = j;
            this.colour = colour;
        }
    }
}
