using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splenduel.Core.Home.Model;

namespace Splenduel.Core.Home.Store
{
    public interface IHomeStore
    {
        Task<ICollection<GameInvite>> GetInvites(string player="");
        Task<GameInvite?> GetInvite(Guid id);
        Task<bool> SaveInvite(GameInvite invite);
        Task<bool> RemoveInvite(Guid id);
        Task<bool> CreateMatch(Model.Duel match);
        Task<Duel> GetMatch(Guid matchId);
        Task<ICollection<Model.Duel>> GetMatches(string player = "");
    }
}
