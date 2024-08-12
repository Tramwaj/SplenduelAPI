using Splenduel.Core.Home.Model;
using Splenduel.Core.Home.Store;

namespace Splenduel.InMemoryStorage
{
    public class HomeInMemory : IHomeStore
    {
        private HashSet<GameInvite> invites = [];
        private HashSet<Duel> duels = [];
        public async Task<ICollection<GameInvite>> GetInvites(string player = "")
        {
            if (invites.Count == 0)
            {
                return new List<GameInvite>();
            }
            if (player == "") return invites.ToList();
            else return invites.Where(x => x.Inviter == player || x.Invitee == player).ToList();
        }
        public async Task<GameInvite?> GetInvite(Guid id)
        {
            var invite = invites.FirstOrDefault(x => x.Id == id);
            return invite;
        }

        public async Task<bool> RemoveInvite(Guid id)
        {
            if (invites.Any(x => x.Id == id))
            {
                var entity = invites.First(x => x.Id == id);
                invites.Remove(entity);
                return true;
            }
            return false;
        }

        public async Task<bool> SaveInvite(GameInvite invite)
        {
            invites.Add(invite);
            return true;
        }

        public async Task<bool> CreateMatch(Duel match)
        {
            if (duels.Any(x => x.Id == match.Id))
                return false;
            duels.Add(match);
            return true;
        }

        public async Task<ICollection<Duel>> GetMatches(string player = "")
        {
            if (duels.Count == 0)
            {
                return new List<Duel>();
            }
            if (player == "") return duels.ToList();
            else return duels.Where(x => x.Player1== player || x.Player2== player).ToList();
        }

        public async Task<Duel> GetMatch(Guid matchId)
        {
            var duel = duels.FirstOrDefault(x => x.Id == matchId);
            return duel;
        }
    }
}
