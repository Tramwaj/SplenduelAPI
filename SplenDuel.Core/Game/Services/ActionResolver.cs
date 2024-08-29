using Splenduel.Core.Game.Model;
using Splenduel.Core.Mappers;
using Splenduel.Interfaces.DTOs;
using Splenduel.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Services
{
    public class ActionResolver
    {
        private IGameInfoSender _hub;

        public ActionResolver(IGameInfoSender hub)
        {
            _hub = hub;
        }

        internal async Task<GameState> ResolveAction(ActionDTO action, GameState previousGameState, string playerName)
        {
            switch (action.Type)
            {
                case PlayerActionEnum.GetCoins:
                    return await GetCoins(action, previousGameState, playerName);
                case PlayerActionEnum.DropCoins:
                    return await DropCoins(action, previousGameState, playerName);
                case PlayerActionEnum.ShuffleCoins:
                    return await ShuffleCoins(action, previousGameState, playerName);
                case PlayerActionEnum.BuyCard:
                    return await BuyCard(action, previousGameState, playerName);
                case PlayerActionEnum.ReserveCard:
                    return await ReserveCard(action, previousGameState, playerName);
                case PlayerActionEnum.GetNoble:
                    return await GetNoble(action, previousGameState, playerName);
                case PlayerActionEnum.TradeScroll:
                    return await TradeScroll(action, previousGameState, playerName);
                default: throw new ApplicationException("Invalid action type");
            }
        }

        private async Task<GameState> GetCoins(ActionDTO action, GameState gs, string playerName)
        {
            var takeCoinRequest = new List<CoinRequest>();// ((CoinRequest[])action.Payload[0]);
            var coinBoardResponse = gs.Board.CoinBoard.TakeCoins(takeCoinRequest);
            if (coinBoardResponse.Success)
            {
                var coins = takeCoinRequest.Select(x => x.colour).ToList();
                await gs.CurrentPlayerBoard.AddCoins(coins);
                gs.LastAction = $"{playerName} took coins";
            }
            else gs.LastAction = $"{playerName} did not take coins";
            return gs;
        }

        private async Task<GameState> DropCoins(ActionDTO action, GameState gs, string playerName)
        {
            //if (action.Payload[0] is List<ColourEnum> coins)
            //{
            //    var dropCoinsResponse = await gs.CurrentPlayerBoard.DropCoins(coins);
            //    if (dropCoinsResponse.Success)
            //    {
            //        gs.LastAction += $"{playerName} dropped coins";
            //        return gs;
            //    }
            //    else throw new ArgumentException(dropCoinsResponse.Error);
            //}
            //else 
            throw new ArgumentException("Parameters should be a list of ColourEnum");
        }

        private async Task<GameState> ShuffleCoins(ActionDTO action, GameState gs, string playerName)
        {
            var coinBoardResponse = gs.Board.CoinBoard.TakeScroll();
            string lastActionEnd;
            if (coinBoardResponse.Success)
            {
                gs.CurrentPlayerBoard.ScrollsCount++;
                lastActionEnd = $"and took a scroll";
            }
            else lastActionEnd = $"and did not take a scroll(no scrolls left)";
            gs.Board.CoinBoard.ShuffleBoard();
            gs.LastAction = $"{playerName} shuffled the coin board {lastActionEnd}";
            await _hub.SendCoinBoard(gs.Board.CoinBoard.MapToVM(), gs.GameId.ToString());
            return gs;
        }

        private async Task<GameState> BuyCard(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }

        private async Task<GameState> ReserveCard(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }

        private async Task<GameState> GetNoble(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }

        private async Task<GameState> TradeScroll(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }
    }
}
