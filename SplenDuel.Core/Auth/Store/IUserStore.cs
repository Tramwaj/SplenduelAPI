using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Auth.Store
{
    public interface IUserStore
    {
        Task<User?> GetUser(string username);
        Task<Guid?> GetUserId(string username);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(string username);
        Task<bool> AddUser(User user);
        Task<ICollection<User>> GetUsers();
    }
}
