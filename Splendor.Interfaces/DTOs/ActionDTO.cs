using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Interfaces.DTOs
{
    public class ActionDTO
    {
        public string Type { get; set; }
        public Guid GameId { get; set; }
        public object Payload { get; set; }
        public ActionDTO()
        {
            
        }
        public ActionDTO(string type, object parameters)
        {
            Type = type;
            Payload = parameters;
        }
    }
}
