namespace Splenduel.Core.Game.Model
{
    internal static class ActionState
    {
        public static string Normal = "Normal";
        public static string EndTurn = "EndTurn";
        public static string DropCoins = "DropCoins";
        public static string ReserveCard = "ReserveCard";
        public static string StealCoin = "StealCoin";
        public static string GetNoble = "GetNoble";
        public static string Player1Win = "Player1Win";
        public static string Player2Win = "Player2Win";

        public static string Pickup(ColourEnum colour) => "Pickup " + colour.ToString();
    }
}
