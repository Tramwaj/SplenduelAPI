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

        internal async Task<GameState?> ResolveAction(ActionDTO action, GameState previousGameState, string playerName)
        {
            ActionResponse response;
            if (playerName != previousGameState.ActivePlayerName)
            {
                response = new ActionResponse(false, "Not your turn");
                await SendMessages(response, previousGameState.GameId);
                return null;
            }
            GameState gs = previousGameState;
            switch (action.Type)
            {
                case PlayerActionEnum.GetCoins:
                    response = null;// decipher coinrequest, and call gs
                    break;
                case PlayerActionEnum.DropCoins:
                    response = await DropCoins(action, previousGameState, playerName);
                    break;
                case PlayerActionEnum.ShuffleCoins:
                    response = await gs.PlayerShufflesTheBoard();
                    break;
                case PlayerActionEnum.BuyCard:
                    response = await BuyCard(action, previousGameState, playerName);
                    break;
                case PlayerActionEnum.ReserveCard:
                    response = await ReserveCard(action, previousGameState, playerName);
                    break;
                case PlayerActionEnum.GetNoble:
                    response = await GetNoble(action, previousGameState, playerName);
                    break;
                case PlayerActionEnum.TradeScroll:
                    response = await TradeScroll(action, previousGameState, playerName);
                    break;
                default: throw new ApplicationException("Invalid action type");
            }
            await SendMessages(response, gs.GameId);
            return gs;
        }

        private async Task SendMessages(ActionResponse response, Guid gameId)
        {
            if (!response.Success)
            {
                await _hub.SendActionStatus(gameId.ToString(), "Error in sending message: " + response.Message);
                return;
            }
            foreach(var obj in response.ChangedObjects)
            {
                if (obj is CoinBoard cb) await _hub.SendCoinBoard(cb.MapToVM(), gameId.ToString());
                if (obj is PlayerBoard pb) await _hub.SendPlayerBoard(pb.MapToVM(), gameId.ToString());

                //if (obj is CardLevel cl) await _hub.SendCardLevel(cl.MapToVM(), cl.Level, gameId.ToString());
                //if (obj is CardBoard
                //int (obj is bool )
            }
            await _hub.SendActionStatus(gameId.ToString(), response.Message);
        }

        private async Task<ActionResponse> GetCoins(ActionDTO action, GameState gs, string playerName)
        {
            var takeCoinRequest = new List<CoinRequest>();// ((CoinRequest[])action.Payload[0]);
            var coinBoardResponse = gs.Board.CoinBoard.TakeCoins(takeCoinRequest);
            if (coinBoardResponse.Success)
            {
                var coins = takeCoinRequest.Select(x => x.colour).ToList();
                await gs.ActivePlayerBoard.AddCoins(coins);
                gs.LastAction = $"{playerName} took coins";
            }
            else gs.LastAction = $"{playerName} did not take coins";
            throw new NotImplementedException();
        }

        private async Task<ActionResponse> DropCoins(ActionDTO action, GameState gs, string playerName)
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

        private async Task<ActionResponse> BuyCard(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }

        private async Task<ActionResponse> ReserveCard(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }

        private async Task<ActionResponse> GetNoble(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }

        private async  Task<ActionResponse> TradeScroll(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }
    }
}
