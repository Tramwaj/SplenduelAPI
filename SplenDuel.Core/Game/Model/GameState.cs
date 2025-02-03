namespace Splenduel.Core.Game.Model
{
    public class GameState
    {
        public Guid GameId { get; set; }
        public bool Player1Turn { get; set; }
        public string State { get; set; }
        public Board Board { get; set; }
        public string LastAction { get; set; }
        public List<string> Actions { get; set; } = new();
        public int TurnNumber = 0;
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
                message += $" {takingPlayerBoard.Player.Name} took a scroll from {otherPlayerBoard.Player.Name}";
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
                if (request.Count(x => x.colour == ColourEnum.Pink) == 2 || (request.Count() == 3 && request.Select(x => x.colour).Distinct().Count() == 1))
                {
                    msg += PlayerGetsScroll(false, objects);
                }
                if (ActivePlayerBoard.CoinsCount > 10)
                {
                    msg += $"{ActivePlayerName} has 10 or more coins and has to drop {ActivePlayerBoard.CoinsCount - 10} of them. ";
                    this.State = ActionState.DropCoins;
                    return new ActionResponse(true, msg, objects, this.State);
                }

                this.State = ActionState.Normal;
                await this.EndTurn();
                objects.Add(this.Player1Turn);
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
            gameObjects.Add(this.Player1Turn);
            return new ActionResponse(true, message, gameObjects, this.State);
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
            string returnState = "";
            var response = new ActionResponse(true, message, gameObjects);
            return await ModifyResponseByCardAction(response, card.Action, card.Colour);
        }
        private async Task<ActionResponse> ModifyResponseByCardAction(ActionResponse response,CardActionEnum action, ColourEnum colour=ColourEnum.Grey)
        {
            switch (action)
            {
                case CardActionEnum.None:
                    await this.EndTurn();
                    this.State = response.State = ActionState.Normal;
                    response.ChangedObjects.Add(this.Player1Turn);
                    break;
                case CardActionEnum.ExtraTurn:
                    response.Message += $"{ActivePlayerName} gets an extra turn. ";
                    this.State = ActionState.Normal;
                    response.State = ActionState.Normal;
                    break;
                case CardActionEnum.Pickup:
                    if (!Board.CoinBoard.CoinsOnBoard.Any(row => row.Any(coin => coin == colour)))
                    {
                        response.Message += $"{ActivePlayerName} can't pick up a {colour} coin. ";
                        await this.EndTurn();
                        this.State = response.State = ActionState.Normal;
                        response.ChangedObjects.Add(this.Player1Turn);
                        break;
                    }
                    response.Message += $"{ActivePlayerName} can pick up a {colour} coin. ";
                    this.State = ActionState.Pickup(colour);
                    response.State = ActionState.Pickup(colour);
                    break;
                case CardActionEnum.Steal:
                    response.Message += $"{ActivePlayerName} can steal a coin from {NotActivePlayerName}. ";
                    this.State = ActionState.StealCoin;
                    response.State = ActionState.StealCoin;
                    break;
                case CardActionEnum.Scroll:
                    response.Message += PlayerGetsScroll(true, response.ChangedObjects);
                    await this.EndTurn();
                    this.State = response.State = ActionState.Normal;
                    response.ChangedObjects.Add(this.Player1Turn);
                    break;
            }
            return response;
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
            gameObjects.Add(this.Player1Turn);
            return new ActionResponse(true, message, gameObjects, this.State);
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
             if (!this.Player1Turn) this.TurnNumber++;
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
            if (NotActivePlayerBoard.Coins[colour] < 1) return ActionResponse.Nok("Player has no coins of this colour. ");
            NotActivePlayerBoard.Coins[colour]--;
            ActivePlayerBoard.Coins[colour]++;
            var gameObjects = new List<object> { ActivePlayerBoard, NotActivePlayerBoard };
            var message = $"{ActivePlayerName} stole a {colour} coin from {NotActivePlayerName}. ";
            this.State = ActionState.Normal;
            await EndTurn();
            gameObjects.Add(this.Player1Turn);
            return new ActionResponse(true, message, gameObjects, this.State);
        }

        internal async Task<ActionResponse> PlayerPicksUpCoin(CoinRequest coin)
        {
            if (coin.colour != Enum.Parse<ColourEnum>(State.Split(' ').Last())) return ActionResponse.Nok("Wrong colour. ");
            var response = this.Board.CoinBoard.TakeCoins([coin]);
            if (!response.Success) return ActionResponse.Nok(response.Message);
            await ActivePlayerBoard.AddCoin(coin.colour);
            var gameObjects = new List<object> { ActivePlayerBoard, Board.CoinBoard };
            var message = $"{ActivePlayerName} picked up a {coin.colour} coin. ";
            this.State = ActionState.Normal;
            await EndTurn();
            gameObjects.Add(this.Player1Turn);
            return new ActionResponse(true, message, gameObjects, this.State);
        }
        internal async Task<ActionResponse> ModifyResponseIfMilestoneAchieved(ActionResponse response)
        {
            if (Board.Player1Board.IsWinConditionFullfilled())
            {
                response.Message += "\n Player 1 wins!";
                response.State = ActionState.Player1Win;
                return response;
            }
            if (Board.Player2Board.IsWinConditionFullfilled())
            {
                response.Message += "\n Player 2 wins!";
                response.State = ActionState.Player2Win;
                return response;
            }
            if (NotActivePlayerBoard.Crowns >= 3 && NotActivePlayerBoard.NoblesTaken == 0)
            {
                await EndTurn();
                response.Message += $"{ActivePlayerName} gets to choose a noble. ";
                this.State = ActionState.GetNoble;
                response.State = ActionState.GetNoble;
                return response;
            }
            if (NotActivePlayerBoard.Crowns >= 6 && NotActivePlayerBoard.NoblesTaken == 1)
            {
                await EndTurn();
                response.Message += $"{ActivePlayerName} gets to choose a noble. ";
                this.State = ActionState.GetNoble;
                response.State = ActionState.GetNoble;
                return response;
            }
            return response;
        }

        internal async Task<ActionResponse> PlayerGetsNoble(int nobleChosen)
        {
            if (Board.Nobles[nobleChosen]==null) return ActionResponse.Nok("Noble already taken. ");
            var noble = Board.Nobles[nobleChosen];
            await ActivePlayerBoard.GetNoble(noble);
            var gameObjects = new List<object> { ActivePlayerBoard, Board.Nobles };
            var response = new ActionResponse(true, $"{ActivePlayerName} took noble {noble}. ",gameObjects);
            response = await ModifyResponseByCardAction(response, noble.Action);
            Board.Nobles[nobleChosen] = null;
            return response;
        }

        internal void AddMessage(string message)
        {
            if (Actions.Count > 10) Actions.RemoveAt(0);
            Actions.Add(message);
        }
    }
}
