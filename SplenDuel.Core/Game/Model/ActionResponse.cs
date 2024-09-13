using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Model
{
    internal class ActionResponse : DefaultResponse
    {        
        public List<object> ChangedObjects => (Object as List<object>)?? null;
        public string State { get; set; } = ActionState.Normal;
        public ActionResponse(bool success, string? message = "") : base(success, message)
        {
        }
        public ActionResponse(bool success, string message, List<object> changedObjects) : base(success,message)
        {
            Object = changedObjects;
        }
        public ActionResponse(bool success, string message, List<object> changedObjects, string state) : base(success, message)
        {
            Object = changedObjects;
            State = state;
        }
        public static new ActionResponse Nok(string error) => new ActionResponse(false, error);
    }
}
