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
        }
        internal async Task<ActionResponse> PlayerShufflesTheBoard()
        {
            this.Board.CoinBoard.ShuffleBoard();
            var gameObjects = new List<object>{this.Board.CoinBoard };
            string message = $"{ActivePlayerName} shuffled the coinboard.";
            message += PlayerGetsScroll(false, gameObjects);
            
            this.State = ActionState.Normal;
            return new ActionResponse(true, message,gameObjects, ActionState.Normal);
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
                var state = this.State = ActionState.EndTurn;
                //todo: check for coins over 10
                var msg = $"{ActivePlayerName} took coins: {coinsInfo}";
                var objects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
                if (request.Count(x=>x.colour==ColourEnum.Pink)==2 || request.Select(x=>x.colour).Distinct().Count()==1)
                {
                    msg += PlayerGetsScroll(false, objects);
                }
                await this.EndTurn();
                return new ActionResponse(true, msg, objects, state);
            }
            return new ActionResponse(false, response.Message);
        }

        internal async Task<ActionResponse> TryBuyCard(int cardId, ColourEnum colour)
        {
            CardLevel cardLevel = this.Board.GetCardLevel(cardId);
            Card card = cardLevel.Exposed.First(x => x.Id == cardId);
            if (cardLevel==null) return ActionResponse.Nok("Card not found on the board");
            var buyCardResponse = await ActivePlayerBoard.BuyCard(card, colour);
            if (!buyCardResponse.Success)
            {
                return new ActionResponse(false, buyCardResponse.Message);
            }
            cardLevel.TakeCardById(card.Id);
            this.Board.CoinBoard.PutCoinsInTheBag(buyCardResponse.Object as IDictionary<ColourEnum, int>);
            var gameObjects = new List<object> { ActivePlayerBoard, cardLevel };
            var message = $"{ActivePlayerName} bought card {card.ToString()}";
            this.State = ActionState.EndTurn;
            this.EndTurn();
            return new ActionResponse(true, message, gameObjects, ActionState.EndTurn);
        }
        internal async Task<ActionResponse> PlayerExchangesScroll(CoinRequest coinRequest)
        {
            if (ActivePlayerBoard.ScrollsCount < 1) return ActionResponse.Nok("No scrolls to exchange"); 
            //this has to be here because of not being able to drop the partially changed GameState right now (could be resolved with different approach to persistence)
            var response = this.Board.CoinBoard.ExchangeScroll(coinRequest);
            if (!response.Success) return ActionResponse.Nok(response.Message);
            await ActivePlayerBoard.GetCoinForScroll(coinRequest.colour);
            var gameObjects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
            var message = $"{ActivePlayerName} exchanged a scroll for a {coinRequest.colour} coin";
            this.State = ActionState.Normal;
            return new ActionResponse(true, message, gameObjects, ActionState.Normal);
        }
        private async Task EndTurn()
        {
            this.Player1Turn=!this.Player1Turn;
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
    }
}
