using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Model
{
    public class Noble
    {
        public int Points { get; set; }
        public CardActionEnum Action { get; set; }
        public Noble()
        {
            
        }
        public Noble(int points, CardActionEnum action)
        {
            Points = points;
            Action = action;
        }
        public override string ToString()
        {
            return $"{Points} points, action: {Action}";
        }
    }
}
