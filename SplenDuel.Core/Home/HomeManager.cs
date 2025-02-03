using Splenduel.Core.Auth.Store;
using Splenduel.Core.Game.Model;
using Splenduel.Core.Global;
using Splenduel.Core.Home.Interfaces;
using Splenduel.Core.Home.Model;
using Splenduel.Core.Home.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Home
{
    public class HomeManager : IHomeManager
    {
        private IHomeStore _homeStore;
        private IUserStore _userStore;

        public HomeManager(IHomeStore homeStore, IUserStore userStore)
        {
            _homeStore = homeStore;
            _userStore = userStore;
        }

        public async Task<DefaultResponse> CreateGameInvite(string inviter, string invitee, bool inviteeStarts)
        {
            string playerStarting = inviteeStarts ? invitee : inviter;
            var guid = Guid.NewGuid();
            var invite = new GameInvite(guid, DateTime.Now, inviter, invitee, playerStarting);
            var currentInvites = await _homeStore.GetInvites();
            if (currentInvites.Any(x => x.Inviter == inviter && x.Invitee == invitee)) return DefaultResponse.Nok("Invite is already sent");
            if (await _homeStore.SaveInvite(invite))
                return DefaultResponse.ok;
            return DefaultResponse.Nok("Could not save the invite");
        }
        public async Task<ICollection<GameInvite>> GetInvites(string player = "")
        {
            return await _homeStore.GetInvites(player);
        }
        public async Task<Guid> AcceptGameInvite(Guid id)
        {
            var invite = await _homeStore.GetInvite(id);
            if (invite == null) throw new ApplicationException($"id:{id} not found");
            var newDuel = this.CreateNewMatch(invite);
            try
            {
                if (await _homeStore.RemoveInvite(id) && await _homeStore.CreateMatch(newDuel))
                {
                    return newDuel.Id;
                }
            }
            catch { throw; }
            return Guid.Empty;
        }
        public async Task<bool> RejectGameInvite(Guid id)
        {
            var invite = await _homeStore.GetInvite(id);
            if (invite == null) return false;
            if (await _homeStore.RemoveInvite(id)) return true;
            else return false;
        }
        public async Task<HomeViewModel> GetHomeViewModel(string user)
        {
            HomeViewModel vm = new();
            var invites = await _homeStore.GetInvites(user);
            vm.OwnInvites = invites.Where(x => x.Inviter == user).ToList();
            vm.Invites = invites.Where(x => x.Invitee == user).ToList();
            var matches = await _homeStore.GetMatches(user);
            vm.CurrentDuels = matches.Where(x => x.EndTime == null).ToList();
            vm.PastDuels = matches.Where(x => x.EndTime != null).ToList();
            vm.Players = (await _userStore.GetUsers()).Select(x=>new Player(x.UserName, x.Id)).ToList();
            return vm;
        }
        
        private Model.Duel CreateNewMatch(GameInvite invite)
        {
            string secondPlayer = invite.Inviter != invite.PlayerStarting ? invite.Inviter : invite.Invitee;
            var match = new Model.Duel(Guid.NewGuid(), invite.PlayerStarting, secondPlayer, DateTime.Now);
            return match;
        }

    }
}
