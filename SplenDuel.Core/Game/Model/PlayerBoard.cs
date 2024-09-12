using Splenduel.Core.Game.Model.Extensions;
using Splenduel.Core.Global;
using Splenduel.Core.Utils;
using Splenduel.Interfaces.VMs;
using System.Linq;

namespace Splenduel.Core.Game.Model
{
    public class PlayerBoard
    {
        public ICollection<Card> HiddenCards { get; private set; } = new List<Card>();
        public int ScrollsCount { get; set; } = 0;
        public IDictionary<ColourEnum, int> PointsByColour { get; private set; } = new Dictionary<ColourEnum, int>();

        public int TotalPoints => PointsByColour.Values.Sum();
        public int Crowns { get; private set; } = 0;
        public ICollection<Card> OwnedCards { get; private set; } = new List<Card>();
        public IDictionary<ColourEnum, int> MiningValues { get; private set; } = MapColourDictionaryFunctions.CreateColourEnumZeroDictionary();
        public IDictionary<ColourEnum, int> Coins { get; private set; } = MapColourDictionaryFunctions.CreateColourEnumZeroDictionary();

        public int HiddenCardsCount => HiddenCards.Count();

        public Player Player { get; set; }


        public PlayerBoard() { }
        public PlayerBoard(bool playerIsSecond)
        {
            if (playerIsSecond) ScrollsCount = 1;
        }
        public PlayerBoard(Player player, int scrollsCount, ICollection<Card> ownedCards, ICollection<Card> hiddenCards, IDictionary<ColourEnum, int> coins)
        {
            this.Player = player;
            ScrollsCount = scrollsCount;
            this.OwnedCards = ownedCards;
            this.HiddenCards = hiddenCards;
            this.Coins = coins;
            foreach (var card in this.OwnedCards)
            {
                UpdateResourcesForCard(card);
            }
        }
        public async Task AddCoin(ColourEnum coin)
        {
            if (this.Coins.Keys.Any(x => x == coin)) this.Coins[coin]++;
            else this.Coins.Add(coin, 1);
        }
        public async Task AddCoins(ICollection<ColourEnum> coins)
        {
            foreach (var coin in coins)
            {
                await AddCoin(coin);
            }
        }
        public async Task<DefaultResponse> DropCoins(ICollection<ColourEnum> coins)
        {
            foreach (var coin in coins)
            {
                if (this.Coins.TryGetValue(coin, out int coinValue))
                {
                    if (coinValue < 1) return DefaultResponse.Nok("Player does not have enough coins to drop!");
                    this.Coins[coin]--;
                }
                else return DefaultResponse.Nok("Player does not have enough coins to drop!");
            }
            return DefaultResponse.Ok(coins.Count().ToString());
        }
        public bool CanAfford(IDictionary<ColourEnum, int> cost, out IDictionary<ColourEnum, int> remainder)
        {
            remainder = this.Coins.ToDictionary();
            this.Coins.TryGetValue(ColourEnum.Gold, out int goldCoins);
            foreach (var singleCost in cost)
            {
                int value = singleCost.Value;
                var key = singleCost.Key;
                if (this.MiningValues.TryGetValue(key, out int miningValue))
                    value -= miningValue;
                if (value <= 0) continue;
                if (this.Coins.TryGetValue(key, out int coinValue))
                {
                    int remainderValue = coinValue - value;
                    value -= coinValue;
                    remainder[key] = remainderValue > 0 ? remainderValue : 0;
                    if (value <= 0) continue;
                    if (remainderValue < 0)
                    {
                        goldCoins += remainderValue;
                        if (goldCoins < 0) return false;
                        remainder[ColourEnum.Gold] = goldCoins;
                    }
                }
                else
                {
                    goldCoins -= value;
                    if (goldCoins < 0) return false;
                    remainder[ColourEnum.Gold] = goldCoins;
                }
            }
            return true;
        }
        public async Task<DefaultResponse> BuyCard(Card card, ColourEnum colour = ColourEnum.Pink)
        {
            if (card == null) return DefaultResponse.Nok("Card is null!");
            Card newCard = card.Copy();
            if (card.Colour == ColourEnum.Multi)
            {
                newCard.Colour = colour;
                if (!this.MiningValues.TryGetValue(colour, out int value) || value <1)
                {
                    return DefaultResponse.Nok("You don't have this colour!");
                }
                if (colour == ColourEnum.Pink || colour == ColourEnum.Multi) return DefaultResponse.Nok("No colour requested for multi-coloured card!");
            }
            var payment = new Dictionary<ColourEnum, int>();
            if (!this.CanAfford(newCard.Cost.CostDictionary, out var newCoins)) return DefaultResponse.Nok("Payment was not possible!");
            else
            {
                foreach (var singleCost in Coins)
                {
                    if (newCoins.TryGetValue(singleCost.Key, out int newValue))
                    {
                        if (newValue < Coins[singleCost.Key]) payment.Add(singleCost.Key, Coins[singleCost.Key] - newValue);
                    }
                }
                this.Coins = newCoins;
            }
            this.OwnedCards.Add(newCard);
            UpdateResourcesForCard(newCard);
            if (this.IsWinConditionFullfilled()) return DefaultResponse.Ok("Win");
            var response = new DefaultResponse(true, payment);
            return response;
        }
        private void UpdateResourcesForCard(Card card)
        {
            this.MiningValues = this.MiningValues.CreateOrAddIfExists(card.Colour, card.MiningPower);
            this.Crowns += card.Crowns;
            this.PointsByColour = this.PointsByColour.CreateOrAddIfExists(card.Colour, card.Points);
        }

        private bool IsWinConditionFullfilled()
        {
            if (this.Crowns >= 10) return true;
            if (this.TotalPoints >= 20) return true;
            if (this.PointsByColour.Values.Any(x => x >= 10)) return true;
            return false;
        }

        public async Task<DefaultResponse> GetCoinForScroll(ColourEnum colour)
        {
            if (this.ScrollsCount < 1) return new DefaultResponse(false, "Player has no scrolls!");
            await AddCoin(colour);
            ScrollsCount--;
            return DefaultResponse.ok;
        }
        public async Task<DefaultResponse> GetReservedCard(Card card, bool isGoldCoinAdded = true)
        {
            if (this.HiddenCardsCount >= 3) return DefaultResponse.Nok("Player already has 3 hidden cards");
            this.HiddenCards.Add(card);
            if (isGoldCoinAdded) this.Coins.CreateOrAddIfExists(ColourEnum.Gold, 1);
            return DefaultResponse.ok;
        }
        public async Task ClearHiddenCards()
        {
            this.HiddenCards.Clear();
        }
    }
}
