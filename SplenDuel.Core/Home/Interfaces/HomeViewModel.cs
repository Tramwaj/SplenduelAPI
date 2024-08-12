using Splenduel.Core.Home.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Home.Interfaces
{
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public ICollection<GameInvite> OwnInvites { get; set; }
        public ICollection<GameInvite> Invitations { get; set; }
        public ICollection<Model.Duel> CurrentDuels { get; set; }
        public ICollection<Model.Duel> PastDuels { get; set; }
    }
}
