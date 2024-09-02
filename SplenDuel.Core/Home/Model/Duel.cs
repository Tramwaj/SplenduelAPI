using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Home.Model
{
    public class Duel
    {
        public Guid Id { get;}
        public string Player1 {get;}
        public string Player2 {get;}
        public DateTime StartingTime {get;}
        public DateTime? EndTime {get;}

        public Duel(Guid id, string player1, string player2, DateTime startingTime)
        {
            this.Id = id;
            Player1 = player1;
            Player2 = player2;
            StartingTime = startingTime;
            EndTime = null;
        }

        public Duel(Guid id, string player1, string player2, DateTime startingTime, DateTime? endingTime) : this(id, player1, player2, startingTime)
        {
            EndTime = endingTime;
        }
    }
}
