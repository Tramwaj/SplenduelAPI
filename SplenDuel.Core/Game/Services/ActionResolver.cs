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
            public CoinRequest CoinRequest() => new CoinRequest(i, j, Enum.Parse<ColourEnum>(colour, true));
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
                    if (gs.State != ActionState.Normal) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    CoinRequestDTO[] coinRequestDTOs = JsonSerializer.Deserialize<CoinRequestDTO[]>(action.Payload.ToString(), jsonOptions);
                    CoinRequest[] coinRequests = coinRequestDTOs.Select(x => x.CoinRequest()).ToArray();
                    response = await gs.PlayerTakesCoins(coinRequests);
                    break;
                case PlayerActionNames.DropCoins:
                    if (gs.State != ActionState.DropCoins) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    ColourEnum[] coins = JsonSerializer.Deserialize<string[]>(action.Payload.ToString(), jsonOptions).Select(c => Enum.Parse<ColourEnum>(c)).ToArray();
                    response = await gs.PlayerDropsCoins(coins);
                    break;
                case PlayerActionNames.ShuffleCoins:
                    if (gs.State != ActionState.Normal) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    response = await gs.PlayerShufflesTheBoard();
                    break;
                case PlayerActionNames.BuyCard:
                    if (gs.State != ActionState.Normal) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    CardRequest cardRequest = JsonSerializer.Deserialize<CardRequest>(action.Payload.ToString(), jsonOptions);
                    ColourEnum colour = Enum.Parse<ColourEnum>(cardRequest.Colour);
                    //Card card = action.Payload as Card;
                    response = await gs.TryBuyCard(cardRequest.CardId, colour);
                    break;
                case PlayerActionNames.TakeGoldCoin:
                    if (gs.State != ActionState.Normal) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    CoinRequestDTO goldCoinRequestDTO = JsonSerializer.Deserialize<CoinRequestDTO>(action.Payload.ToString(), jsonOptions);
                    CoinRequest goldCoinRequest = goldCoinRequestDTO.CoinRequest();
                    response = await gs.PlayerTakesGoldCoin(goldCoinRequest);
                    break;
                case PlayerActionNames.ReserveCard:
                    if (gs.State != ActionState.ReserveCard) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    CardRequest reserveCardRequest = JsonSerializer.Deserialize<CardRequest>(action.Payload.ToString(), jsonOptions);
                    ColourEnum reservedColour = Enum.Parse<ColourEnum>(reserveCardRequest.Colour);
                    response = await gs.TryReserveCard(reserveCardRequest.CardId, reservedColour);
                    break;
                case PlayerActionNames.TradeScroll:
                    if (gs.State != ActionState.Normal) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    CoinRequestDTO coinRequestDTO = JsonSerializer.Deserialize<CoinRequestDTO>(action.Payload.ToString(), jsonOptions);
                    CoinRequest coinRequest = coinRequestDTO.CoinRequest();
                    response = await gs.PlayerExchangesScroll(coinRequest);
                    break;
                case PlayerActionNames.StealCoin:
                    if (gs.State != ActionState.StealCoin) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    ColourEnum colourEnum = Enum.Parse<ColourEnum>(action.Payload.ToString());
                    response = await gs.PlayerStealsCoin(colourEnum);
                    break;
                case PlayerActionNames.PickupCoin:
                    if (!gs.State.Contains("Pickup")) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    var coinDTO = JsonSerializer.Deserialize<CoinRequestDTO>(action.Payload.ToString(), jsonOptions);
                    var coin = coinDTO.CoinRequest();
                    response = await gs.PlayerPicksUpCoin(coin);
                    break;
                case PlayerActionNames.GetNoble:
                    if (gs.State != ActionState.GetNoble) { response = new ActionResponse(false, $"Not the right state: {action.Type} on  {gs.State}"); break; }
                    int nobleChosen = int.Parse(action.Payload.ToString());
                    response = await gs.PlayerGetsNoble(nobleChosen);
                    break;
                default:
                    response = new ActionResponse(false, "Action not found");
                    break;
            }
            if (!response.Success)
            {
                await _hub.SendActionStatus(gs.GameId.ToString(), response.State, response.Message);
                return null;
            }
            if (response.State == ActionState.EndTurn.ToString())
            {
                response = await gs.ModifyResponseIfMilestoneAchieved(response);
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
                if (obj is Noble[] nobles) await _hub.SendNobles(nobles.Select(n => n.MapToVM()).ToArray(), gameId.ToString());
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
    }
}
