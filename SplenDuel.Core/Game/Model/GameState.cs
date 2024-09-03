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
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public string LastAction { get; set; }
        public string ActivePlayerName => Player1Turn ? Player1.Name : Player2.Name;

        public string NotActivePlayerName => Player1Turn ? Player2.Name : Player1.Name;
        public PlayerBoard ActivePlayerBoard => Player1Turn ? Board.Player1Board : Board.Player2Board;
        public PlayerBoard NotActivePlayerBoard => Player1Turn ? Board.Player2Board : Board.Player1Board;

        public GameState()
        {

        }
        public GameState(Guid gameId, bool player1Turn, Board board, Player player1, Player player2, string lastAction)
        {
            GameId = gameId;
            Player1Turn = player1Turn;
            Board = board;
            Player1 = player1;
            Player2 = player2;
            LastAction = lastAction;
        }
        public async Task<GameState> PerPlayer(string playerName)
        {
            if (Player1.Name == playerName) await this.Board.Player2Board.ClearHiddenCards();
            if (Player2.Name == playerName) await this.Board.Player1Board.ClearHiddenCards();
            else
            {
                await this.Board.Player1Board.ClearHiddenCards();
                await this.Board.Player2Board.ClearHiddenCards();
            }
            return this;
        }
    }
}
