using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Auth
{
    public class JwtSettings
    {        
        public string ValidIssuer { get; set; } = "https://localhost:44347/";
        public string ValidAudience { get; set; } = "https://localhost:44347/";
        public string Secret { get; set; } = "1983as6fg6h4e5r646504nk2hlasdasd";
        public int LifetimeInSeconds { get; set; } = 10800;
    }
}
