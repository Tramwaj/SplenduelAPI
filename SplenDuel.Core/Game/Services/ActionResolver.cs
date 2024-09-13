using Microsoft.IdentityModel.Tokens;
using Splenduel.Core.Game.Model;
using Splenduel.Core.Mappers;
using Splenduel.Interfaces.DTOs;
using Splenduel.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private class CoinRequestDTO
        {
            public int i;
            public int j;
            public string colour;
            public CoinRequest CoinRequest() => new CoinRequest(i, j, Enum.Parse<ColourEnum>(colour,true));
        }

        internal async Task<GameState?> ResolveAction(ActionDTO action, GameState previousGameState, string playerName)
        {
            ActionResponse response;
            if (playerName != previousGameState.ActivePlayerName)
            {
                response = new ActionResponse(false, "Not your turn");
                await _hub.SendActionStatus(previousGameState.GameId.ToString(), response.State, response.Message);
                return null;
            }
            GameState gs = previousGameState;
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            };
            switch (action.Type)
            {
                case PlayerActionNames.GetCoins:
                    CoinRequestDTO[] coinRequestDTOs = JsonSerializer.Deserialize<CoinRequestDTO[]>(action.Payload.ToString(),jsonOptions);
                    CoinRequest[] coinRequests = coinRequestDTOs.Select(x=>x.CoinRequest()).ToArray();
                    response = await gs.PlayerTakesCoins(coinRequests);
                    break;
                case PlayerActionNames.DropCoins:
                    response = await DropCoins(action, previousGameState, playerName);
                    break;
                case PlayerActionNames.ShuffleCoins:
                    response = await gs.PlayerShufflesTheBoard();
                    break;
                case PlayerActionNames.BuyCard:
                    BuyCardRequest cardRequest = JsonSerializer.Deserialize<BuyCardRequest>(action.Payload.ToString(),jsonOptions);
                    ColourEnum colour = Enum.Parse<ColourEnum>(cardRequest.Colour);
                    //Card card = action.Payload as Card;
                    response = await gs.TryBuyCard(cardRequest.CardId, colour);
                    break;
                case PlayerActionNames.ReserveCard:
                    response = await ReserveCard(action, previousGameState, playerName);
                    break;
                case PlayerActionNames.GetNoble:
                    response = await GetNoble(action, previousGameState, playerName);
                    break;
                case PlayerActionNames.TradeScroll:
                    CoinRequestDTO coinRequestDTO = JsonSerializer.Deserialize<CoinRequestDTO>(action.Payload.ToString(), jsonOptions);
                    CoinRequest coinRequest = coinRequestDTO.CoinRequest();
                    response = await gs.PlayerExchangesScroll(coinRequest);
                    break;
                default: throw new ApplicationException("Invalid action type");
            }
            if (!response.Success)
            {
                await _hub.SendActionStatus(gs.GameId.ToString(), response.State, response.Message);
                return null;
            }
            await SendMessages(response, gs.GameId);
            
            return gs;
        }

        private async Task SendMessages(ActionResponse response, Guid gameId)
        {
            
            foreach (var obj in response.ChangedObjects)
            {
                if (obj is CoinBoard cb) await _hub.SendCoinBoard(cb.MapToVM(), gameId.ToString());
                if (obj is PlayerBoard pb) await _hub.SendPlayerBoard(pb.MapToVM(), gameId.ToString());
                if (obj is CardLevel cl)
                {
                    int level = cl.Exposed.First().Id / 100;
                    await _hub.SendCardLevel(cl.MapToVM(), level, gameId.ToString());
                }
                //if (obj is CardBoard
                //int (obj is bool )
            }
            await _hub.SendActionStatus(gameId.ToString(), response.State, response.Message);
            //todo: currently when player leaves when asked for perform an action, it will not be reflected when he comes back
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

        private async Task<ActionResponse> TradeScroll(ActionDTO action, GameState previousGameState, string playerName)
        {
            throw new NotImplementedException();
        }
    }
}
