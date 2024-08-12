using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Auth.Interfaces
{
    public interface IUserService
    {
        public Task<LoginData> Login(string username, string password);
    }
}
