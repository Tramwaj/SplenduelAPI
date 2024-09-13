namespace Splenduel.Core.Game.Model
{
    internal static class ActionState
    {
        public static string Normal = "Normal";
        public static string EndTurn = "EndTurn";
        public static string DropCoins = "DropCoins";
        //public static string ExchangeScroll = "ExchangeScroll";
        public static string ReserveCard = "ReserveCard";
        public static string StealCoin = "StealCoin";
        public static string Pickup(ColourEnum colour) => "Pickup " + colour.ToString();
    }
}
