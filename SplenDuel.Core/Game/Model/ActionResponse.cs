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
        public string State { get; set; }
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
    internal static class ActionState
    {
        public static string Normal = "Normal";
        public static string EndTurn = "EndTurn";
        public static string DropCoins = "DropCoins";
        public static string ExchangeScroll = "ExchangeScroll";
        public static string ReserveCard = "ReserveCard";
        public static string StealCoin = "StealCoin";
        public static string Pickup(ColourEnum colour) => "Pickup " + colour.ToString();
    }
}
