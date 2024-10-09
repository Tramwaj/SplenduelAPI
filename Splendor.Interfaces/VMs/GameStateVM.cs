using System.Collections.Generic;

namespace Splenduel.Interfaces.VMs
{
    public class GameStateVM
    {
        public Guid GameId { get; set; }
        public bool Player1Turn { get; set; }
        public BoardVM Board { get; set; }
        public string LastAction { get; set; }
        public string ActionState { get; set; }
        public List<string> Actions { get; set; }

    }
    public class BoardVM
    {
        public CardLevelVM Level1 { get; set; }
        public CardLevelVM Level2 { get; set; }
        public CardLevelVM Level3 { get; set; }
        public CoinBoardVM CoinBoard { get; set; }
        public PlayerBoardVM Player1Board { get; set; }
        public PlayerBoardVM Player2Board { get; set; }
        public NobleVM[] Nobles { get; set; }

    }
    public class PlayerVM
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

    }
    public class CardLevelVM
    {
        public List<CardVM> Exposed { get; set; }
        public int DeckCount { get; set; }

    }
    public class CardVM
    {
        public int Id { get; set; }
        public int MiningPower { get; set; }
        public int Level { get; set; }
        public int Points { get; set; }
        public string Colour { get; set; }
        public int Crowns { get; set; }
        public string Action { get; set; }
        public SingleCost[] CardCost { get; set; }
    }
    public class SingleCost
    {
        public string Colour { get; set; }
        public int Amount{ get; set; }

        public SingleCost(string colour, int amount)
        {
            Colour = colour;
            Amount = amount;
        }
    }
    public class CoinBoardVM
    {
        public string[][] CoinsOnBoard { get; set; }
        public int ScrollCount { get; set; }

    }
    public class PlayerBoardVM
    {
        public PlayerVM Player { get; set; }
        public List<CardVM> HiddenCards { get; set; }
        public int ScrollsCount { get; set; }
        public Dictionary<string, int> PointsByColour { get; set; }
        public int TotalPoints { get; set; }
        public int Crowns { get; set; }
        public List<CardVM> OwnedCards { get; set; }
        public Dictionary<string, int> MiningValues { get; set; }
        public Dictionary<string, int> Coins { get; set; }
        public int HiddenCardsCount { get; set; }

    }
    public class NobleVM
    {
        public int Points { get; set; }
        public string Action { get; set; }

    }
}
