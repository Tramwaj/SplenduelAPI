using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Auth.Interfaces
{
    public class LoginData
    {
        public string Token { get; set; }
        public string UserName { get; set; }

        public LoginData(string token, string userName)
        {
            Token = token;
            UserName = userName;
        }
    }
}
