using Microsoft.IdentityModel.Tokens;
using Splenduel.Core.Game.Model.ViewModels;
using Splenduel.Interfaces.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Model
{
    public class GameState
    {
        public Guid GameId { get; set; }
        public bool Player1Turn { get; set; }
        public string State { get; set; }
        public Board Board { get; set; }
        public string LastAction { get; set; }
        public string ActivePlayerName => Player1Turn ? Board.Player1Board.Player.Name : Board.Player2Board.Player.Name;

        public string NotActivePlayerName => Player1Turn ? Board.Player2Board.Player.Name : Board.Player1Board.Player.Name;
        public PlayerBoard ActivePlayerBoard => Player1Turn ? Board.Player1Board : Board.Player2Board;
        public PlayerBoard NotActivePlayerBoard => Player1Turn ? Board.Player2Board : Board.Player1Board;

        public GameState()
        {

        }
        public GameState(Guid gameId, bool player1Turn, Board board, string lastAction)
        {
            GameId = gameId;
            Player1Turn = player1Turn;
            Board = board;
            LastAction = lastAction;
            State = ActionState.Normal;
        }
        internal async Task<ActionResponse> PlayerShufflesTheBoard()
        {
            this.Board.CoinBoard.ShuffleBoard();
            var gameObjects = new List<object> { this.Board.CoinBoard };
            string message = $"{ActivePlayerName} shuffled the coinboard.";
            message += PlayerGetsScroll(false, gameObjects);

            this.State = ActionState.Normal;
            return new ActionResponse(true, message, gameObjects, ActionState.Normal);
        }
        private string PlayerGetsScroll(bool active, List<object> gameObjects)
        {
            var takingPlayerBoard = active ? ActivePlayerBoard : NotActivePlayerBoard;
            var otherPlayerBoard = active ? NotActivePlayerBoard : ActivePlayerBoard;

            string message = "";
            var TakeScrollResponse = this.Board.CoinBoard.TakeScroll();
            if (TakeScrollResponse.Success)
            {
                takingPlayerBoard.ScrollsCount++;
                gameObjects.Add(takingPlayerBoard);
                message += $"{takingPlayerBoard.Player.Name} took a scroll.";
            }
            else if (otherPlayerBoard.ScrollsCount > 0)
            {
                otherPlayerBoard.ScrollsCount--;
                takingPlayerBoard.ScrollsCount++;
                gameObjects.Add(takingPlayerBoard);
                gameObjects.Add(otherPlayerBoard);
                message += $" {takingPlayerBoard.Player.Name} gave a scroll to {otherPlayerBoard.Player.Name}";
            }
            else
            {
                message += $"No scrolls to be given to {takingPlayerBoard.Player.Name}";
            }
            return message;
        }
        internal async Task<ActionResponse> PlayerTakesCoins(CoinRequest[] request)
        {
            var response = this.Board.CoinBoard.TakeCoins(request);
            if (response.Success)
            {
                await ActivePlayerBoard.AddCoins(request.Select(x => x.colour).ToList());
                var coinsInfo = string.Join(", ", request.Select(x => x.colour.ToString()));
                //todo: check for coins over 10
                var msg = $"{ActivePlayerName} took coins: {coinsInfo}. ";
                var objects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
                if (request.Count(x => x.colour == ColourEnum.Pink) == 2 || request.Select(x => x.colour).Distinct().Count() == 1)
                {
                    msg += PlayerGetsScroll(false, objects);
                }
                if (ActivePlayerBoard.CoinsCount <= 10)
                {
                    this.State = ActionState.Normal;
                    await this.EndTurn();
                    return new ActionResponse(true, msg, objects, ActionState.EndTurn);
                }
                msg += $"{ActivePlayerName} has 10 or more coins and has to drop {ActivePlayerBoard.CoinsCount - 10} of them. ";
                this.State = ActionState.DropCoins;
                return new ActionResponse(true, msg, objects, this.State);
            }
            return new ActionResponse(false, response.Message);
        }
        internal async Task<ActionResponse> PlayerDropsCoins(ColourEnum[] coins)
        {
            var response = await ActivePlayerBoard.DropCoins(coins);
            if (!response.Success) return new ActionResponse(false, response.Message);
            this.Board.CoinBoard.PutCoinsInTheBag(coins);
            var gameObjects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
            var message = $"{ActivePlayerName} dropped coins: {string.Join(",", coins.Select(c => c.ToString()).ToArray())}. ";
            this.State = ActionState.Normal;
            await this.EndTurn();
            return new ActionResponse(true, message, gameObjects, ActionState.EndTurn);
        }
        internal async Task<ActionResponse> TryBuyCard(int cardId, ColourEnum colour)
        {
            Card? card = null;
            CardLevel? cardLevel = null;
            List<object> gameObjects = new();
            bool CardIsReserved = FindReservedCard(cardId, out card);
            if (!CardIsReserved)
            {
                FindCard(cardId, out cardLevel, out card);
                if (cardLevel == null) return ActionResponse.Nok("Card not found on the board. ");
            }
            var buyCardResponse = await ActivePlayerBoard.BuyCard(card, colour);
            if (!buyCardResponse.Success)
            {
                return new ActionResponse(false, buyCardResponse.Message);
            }
            if (!CardIsReserved)
            {
                cardLevel.TakeCardById(card.Id);
                gameObjects.Add(cardLevel);
            }
            else ActivePlayerBoard.HiddenCards.Remove(card);

            this.Board.CoinBoard.PutCoinsInTheBag(buyCardResponse.Object as IDictionary<ColourEnum, int>);
            gameObjects.Add(ActivePlayerBoard);
            var message = $"{ActivePlayerName} bought card {card}. ";

            string returnState="";
            switch (card.Action)
            {
                case CardActionEnum.None:
                    this.State = ActionState.Normal;
                    returnState = ActionState.EndTurn;
                    await this.EndTurn();
                    break;
                case CardActionEnum.ExtraTurn:
                    message += $"{ActivePlayerName} gets an extra turn. ";
                    this.State = ActionState.Normal;
                    returnState = ActionState.Normal;
                    break;
                case CardActionEnum.CoinPickup:
                    message += $"{ActivePlayerName} can pick up a {colour} coin. ";
                    this.State = ActionState.Pickup(colour);
                    returnState = ActionState.Pickup(colour);
                    break;
                case CardActionEnum.Steal:
                    message += $"{ActivePlayerName} can steal a coin from {NotActivePlayerName}. ";
                    this.State = ActionState.StealCoin;
                    returnState = ActionState.StealCoin;
                    break;
                case CardActionEnum.Scroll:
                    message += PlayerGetsScroll(true, gameObjects);
                    this.State = ActionState.Normal;
                    returnState = ActionState.EndTurn;
                    break;
            }
            return new ActionResponse(true, message, gameObjects, returnState);


        }

        private void FindCard(int cardId, out CardLevel? cardLevel, out Card? card)
        {
            cardLevel = this.Board.GetCardLevel(cardId);
            card = cardLevel.Exposed.First(x => x.Id == cardId);
        }
        private bool FindReservedCard(int cardId, out Card? card)
        {
            card = ActivePlayerBoard.HiddenCards.FirstOrDefault(x => x.Id == cardId);
            if (card is null) return false;
            return true;
        }

        internal async Task<ActionResponse> TryReserveCard(int cardId, ColourEnum colour)
        {
            Card card;
            CardLevel cardLevel = null;
            string message;
            if (ActivePlayerBoard.HiddenCardsCount >= 3) throw new InvalidOperationException("Player already has 3 hidden cards");
            if (cardId % 100 != 99)
            {
                FindCard(cardId, out cardLevel, out card);
                if (cardLevel == null) return ActionResponse.Nok("Card not found on the board. ");
                cardLevel.TakeCardById(card.Id);
                message = $"{ActivePlayerName} reserved card {card.ToString()}. ";
            }
            else
            {
                switch (cardId)
                {
                    case 99:
                        cardLevel = this.Board.Level1; break;
                    case 199:
                        cardLevel = this.Board.Level2; break;
                    case 299:
                        cardLevel = this.Board.Level3; break;
                }
                card = cardLevel?.DrawCardFromDeck();
                if (card is null) return ActionResponse.Nok("No cards left in the deck. ");
                message = $"{ActivePlayerName} reserved a card from the deck. ";
            }
            ActivePlayerBoard.HiddenCards.Add(card);
            var gameObjects = new List<object> { ActivePlayerBoard, cardLevel };
            this.State = ActionState.Normal;
            await this.EndTurn();
            return new ActionResponse(true, message, gameObjects, ActionState.EndTurn);
        }
        internal async Task<ActionResponse> PlayerExchangesScroll(CoinRequest coinRequest)
        {
            if (ActivePlayerBoard.ScrollsCount < 1) return ActionResponse.Nok("No scrolls to exchange. ");
            //this has to be here because of not being able to drop the partially changed GameState right now (could be resolved with different approach to persistence)
            var response = this.Board.CoinBoard.ExchangeScroll(coinRequest);
            if (!response.Success) return ActionResponse.Nok(response.Message);
            await ActivePlayerBoard.GetCoinForScroll(coinRequest.colour);
            var gameObjects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
            var message = $"{ActivePlayerName} exchanged a scroll for a {coinRequest.colour} coin. ";
            this.State = ActionState.Normal;
            return new ActionResponse(true, message, gameObjects, ActionState.Normal);
        }
        private async Task EndTurn()
        {
            this.Player1Turn = !this.Player1Turn;
        }


        /// <summary>
        /// This is probably no going to be used (cards shall be visible to all players)
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public async Task<GameState> PerPlayer(string playerName)
        {
            if (Board.Player1Board.Player.Name == playerName) await this.Board.Player2Board.ClearHiddenCards();
            if (Board.Player2Board.Player.Name == playerName) await this.Board.Player1Board.ClearHiddenCards();
            else
            {
                await this.Board.Player1Board.ClearHiddenCards();
                await this.Board.Player2Board.ClearHiddenCards();
            }
            return this;
        }

        internal async Task<ActionResponse> PlayerTakesGoldCoin(CoinRequest coinRequest)
        {
            if (ActivePlayerBoard.HiddenCardsCount >= 3) return ActionResponse.Nok("Player already has 3 hidden cards");
            var response = this.Board.CoinBoard.TakeGoldCoin(coinRequest);
            if (!response.Success) return ActionResponse.Nok(response.Message);
            await ActivePlayerBoard.AddCoin(ColourEnum.Gold);
            var gameObjects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
            var message = $"{ActivePlayerName} took a gold coin. ";
            this.State = ActionState.ReserveCard;
            return new ActionResponse(true, message, gameObjects, ActionState.ReserveCard);
        }
        internal async Task<ActionResponse> PlayerStealsCoin(ColourEnum colour)
        {
            if (NotActivePlayerBoard.Coins[colour]<1) return ActionResponse.Nok("Player has no coins of this colour. ");
            NotActivePlayerBoard.Coins[colour]--;
            ActivePlayerBoard.Coins[colour]++;
            var gameObjects = new List<object> { ActivePlayerBoard, NotActivePlayerBoard };
            var message = $"{ActivePlayerName} stole a {colour} coin from {NotActivePlayerName}. ";
            this.State = ActionState.Normal;
            return new ActionResponse(true, message, gameObjects, ActionState.Normal);
        }
    }
}
