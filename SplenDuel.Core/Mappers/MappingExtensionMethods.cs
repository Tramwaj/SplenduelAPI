using Splenduel.Interfaces.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Mappers
{
    public static class MappingExtensionMethods
    {
        public static GameStateVM MapToVM(this Splenduel.Core.Game.Model.GameState gameState)
        {
            return new GameStateVM
            {
                GameId = gameState.GameId,
                Player1Turn = gameState.Player1Turn,
                Board = gameState.Board.MapToVM(),
                LastAction = gameState.LastAction,
                ActionState = gameState.State,
                Actions = gameState.Actions
            };
        }
        public static BoardVM MapToVM(this Splenduel.Core.Game.Model.Board board)
        {
            return new BoardVM
            {
                Level1 = board.Level1.MapToVM(),
                Level2 = board.Level2.MapToVM(),
                Level3 = board.Level3.MapToVM(),
                CoinBoard = board.CoinBoard.MapToVM(),
                Player1Board = board.Player1Board.MapToVM(),
                Player2Board = board.Player2Board.MapToVM(),
                Nobles = board.Nobles.Select(n => n.MapToVM()).ToArray()
            };
        }
        public static CardLevelVM MapToVM(this Splenduel.Core.Game.Model.CardLevel cardLevel)
        {
            return new CardLevelVM
            {
                Exposed = cardLevel.Exposed.Select(c => c.MapToVM()).ToList(),
                DeckCount = cardLevel.DeckCount
            };
        }
        public static CardVM MapToVM(this Splenduel.Core.Game.Model.Card card)
        {
            var cardcost = card.Cost == null ? new SingleCost[0] : card.Cost.CostDictionary.Select(c => new SingleCost(c.Key.ToString(), c.Value)).ToArray();
            return new CardVM
            {
                Id = card.Id,
                Level = card.Level,
                MiningPower = card.MiningPower,
                Points = card.Points,
                Crowns = card.Crowns,
                Colour = card.Colour.ToString(),
                Action = card.Action.ToString(),
                CardCost = cardcost
            };
        }
        public static CoinBoardVM MapToVM(this Splenduel.Core.Game.Model.CoinBoard coinBoard)
        {
            return new CoinBoardVM
            {
                CoinsOnBoard = coinBoard.CoinsOnBoard.Select(c => c.Select(x => x.ToString()).ToArray()).ToArray(),
                ScrollCount = coinBoard.ScrollCount
            };
        }
        public static PlayerBoardVM MapToVM(this Splenduel.Core.Game.Model.PlayerBoard playerBoard)
        {
            return new PlayerBoardVM
            {
                Player = playerBoard.Player.MapToVM(),
                HiddenCards = playerBoard.HiddenCards.Select(c => c.MapToVM()).ToList(),
                ScrollsCount = playerBoard.ScrollsCount,
                PointsByColour = playerBoard.PointsByColour.ToDictionary(k => k.Key.ToString(), v => v.Value),
                TotalPoints = playerBoard.TotalPoints,
                Crowns = playerBoard.Crowns,
                OwnedCards = playerBoard.OwnedCards.Select(c => c.MapToVM()).ToList(),
                MiningValues = playerBoard.MiningValues.ToDictionary(k => k.Key.ToString(), v => v.Value),
                Coins = playerBoard.Coins.ToDictionary(k => k.Key.ToString(), v => v.Value),
                HiddenCardsCount = playerBoard.HiddenCardsCount
            };
        }
        public static PlayerVM MapToVM(this Splenduel.Core.Game.Model.Player player)
        {
            return new PlayerVM
            {
                Name = player.Name,
                Id = player.Id
            };
        }
        public static NobleVM MapToVM(this Splenduel.Core.Game.Model.Noble noble)
        {
            if (noble == null) return null;
            return new NobleVM
            {
                Points = noble.Points,
                Action = noble.Action.ToString()
            };
        }
    }
}

