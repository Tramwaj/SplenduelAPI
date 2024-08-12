using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Home.Model
{
    public record GameInvite
    {
        public Guid Id { get; set; }
        public DateTime TimeCreated { get; set; }
        public string Inviter { get; set; }
        public string Invitee { get; set; }
        public string PlayerStarting { get; set; }

        public GameInvite(Guid id, DateTime timeCreated, string inviter, string invitee, string playerStarting)
        {
            if (playerStarting != inviter && playerStarting != invitee)
                throw new InvalidDataException($"Player starting is different than both players {inviter}||{invitee}!={playerStarting}");
            Id = id;
            TimeCreated = timeCreated;
            Inviter = inviter;
            Invitee = invitee;
            PlayerStarting = playerStarting;
        }

    }
}
