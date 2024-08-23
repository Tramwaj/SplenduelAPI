using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Interfaces.DTOs
{
    public class ActionDTO
    {
        public string ActionType { get; set; }
        public string PlayerName { get; set; }
        public Guid GameId { get; set; }
        public object Param1 { get; set; }
        public object? Param2 { get; set; }

        public ActionDTO(string actionType, string playerName, object param1, object? param2 = null)
        {
            PlayerName= playerName;
            ActionType = actionType;
            Param1 = param1;
            Param2 = param2;
        }
    }
}
