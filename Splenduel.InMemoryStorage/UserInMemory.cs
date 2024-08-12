using Splenduel.Core.Auth;
using Splenduel.Core.Auth.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.InMemoryStorage
{
    public class UserInMemory : IUserStore
    {
        private HashSet<User> _users = new HashSet<User>();
        public async Task<bool> AddUser(User user)
        {
            if (_users.Any(x => x.Id == user.Id || x.UserName == user.UserName))
                return false;

            _users.Add(user);
            return true;

        }

        public async Task<bool> DeleteUser(string username)
        {
            var entity = _users.FirstOrDefault(u => u.UserName == username);
            if (entity == null)
                return false;

            _users.Remove(entity);
            return true;

        }

        public async Task<User?> GetUser(string username)
        {
            var user = _users.FirstOrDefault(x => x.UserName == username);
            return user;
        }

        public async Task<Guid?> GetUserId(string username)
        {
            var user = _users.FirstOrDefault(x => x.UserName == username);
            if (user is null) return null;
            return user.Id;
        }

        public async Task<ICollection<User>> GetUsers()
        {
            if (_users.Count == 0)
                return new List<User>();
            return _users.ToList();
        }

        public async Task<bool> UpdateUser(User user)
        {
            var userToChange = _users.FirstOrDefault(x => x == user);
            if (userToChange == null)
                return false;
            _users.Remove(userToChange);
            _users.Add(user);
            return true;
        }
    }
}
