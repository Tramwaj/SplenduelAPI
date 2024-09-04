using Microsoft.IdentityModel.Tokens;
using Splenduel.Core.Game.Model.ViewModels;
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
            string message = $"{ActivePlayerName} shuffled the coinboard";
            var TakeScrollResponse = this.Board.CoinBoard.TakeScroll();
            if (TakeScrollResponse.Success)
            {
                NotActivePlayerBoard.ScrollsCount++;
                gameObjects.Add(this.NotActivePlayerBoard);
                message += $" and {NotActivePlayerName} took a scroll";
            }
            else if (ActivePlayerBoard.ScrollsCount >0)
            {
                ActivePlayerBoard.ScrollsCount--;
                NotActivePlayerBoard.ScrollsCount++;
                gameObjects.Add(this.ActivePlayerBoard);
                gameObjects.Add(this.NotActivePlayerBoard);
                message += $" and gave a scroll to {NotActivePlayerName}";
            }
            return new ActionResponse(true, message,gameObjects );
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
