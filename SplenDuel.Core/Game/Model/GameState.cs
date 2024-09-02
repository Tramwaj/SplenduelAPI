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
        public string ActivePlayerName
        {
            get
            {
                if (Player1Turn) return Player1.Name;
                return Player2.Name;
            }
        }
        public string NotActivePlayerName
        {
            get
            {
                if (Player1Turn) return Player2.Name;
                return Player1.Name;
            }
        }
        public PlayerBoard ActivePlayerBoard
        {
            get
            {
                if (Player1Turn) return Board.Player1Board;
                return Board.Player2Board;
            }
        }
        public PlayerBoard NotActivePlayerBoard
        {
            get
            {
                if (Player1Turn) return Board.Player2Board;
                return Board.Player1Board;
            }
        }

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
            if (Player1.Name==playerName) await this.Board.Player2Board.ClearHiddenCards();
            if (Player2.Name == playerName) await this.Board.Player1Board.ClearHiddenCards();
            else
            {
                await this.Board.Player1Board.ClearHiddenCards();
                await this.Board.Player2Board.ClearHiddenCards();
            }
            return this;
        }
    }
    public class Board
    {
        public CardLevel Level1 { get; set; }
        public CardLevel Level2 { get; set; }
        public CardLevel Level3 { get; set; }
        public CoinBoard CoinBoard { get; set; }
        public PlayerBoard Player1Board { get; set; }
        public PlayerBoard Player2Board { get; set; }

        public Board(CardLevel level1, CardLevel level2, CardLevel level3, CoinBoard coinBoard, PlayerBoard player1Board, PlayerBoard player2Board)
        {
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
            this.CoinBoard = coinBoard;
            Player1Board = player1Board;
            Player2Board = player2Board;
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public Player(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}
