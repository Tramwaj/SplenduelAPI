using Splenduel.Core.Global;
using Splenduel.Core.Home.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Home.Interfaces
{
    public interface IHomeManager
    {
        public Task<DefaultResponse> CreateGameInvite(string inviter, string invitee, bool inviteeStarts);
        public Task<ICollection<GameInvite>> GetInvites(string player = "");
        public Task<Guid> AcceptGameInvite(Guid id);
        public Task<bool> RejectGameInvite(Guid id);
        public Task<HomeViewModel> GetHomeViewModel(string user);
    }
}
