using Splenduel.Core.Global;

namespace Splenduel.Core.Game.Model
{
    public class CoinBoard
    {
        private readonly Random random = new Random();
        private List<ColourEnum> _coinsInBag;
        private int _initialScrollCount;

        public ColourEnum[][] CoinsOnBoard { get; private set; }
        public int ScrollCount { get; private set; }
        public CoinBoard()
        {
        }
        private void ClearTheBoard()
        {
            for (int i = 0; i < 5; i++)
            {
                CoinsOnBoard[i] = new ColourEnum[5];
                for (int j = 0; j < 5; j++)
                {
                    CoinsOnBoard[i][j] = ColourEnum.Grey;
                }
            }
        }
        public CoinBoard(int initialScrollCount = 2)
        {
            _initialScrollCount = initialScrollCount;
            ScrollCount = initialScrollCount;
            CoinsOnBoard = new ColourEnum[5][];
            ClearTheBoard();
            _coinsInBag = new();
            for (int i = 0; i < 4; i++)
            {
                _coinsInBag.Add(ColourEnum.White);
                _coinsInBag.Add(ColourEnum.Blue);
                _coinsInBag.Add(ColourEnum.Green);
                _coinsInBag.Add(ColourEnum.Red);
                _coinsInBag.Add(ColourEnum.Black);
            }
            _coinsInBag.Add(ColourEnum.Pink);
            _coinsInBag.Add(ColourEnum.Pink);

            _coinsInBag.Add(ColourEnum.Gold);
            _coinsInBag.Add(ColourEnum.Gold);
            _coinsInBag.Add(ColourEnum.Gold);
            FillTheBoard();
        }
        private void PackTheCoins()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (CoinsOnBoard[i][j] != ColourEnum.Grey)
                        _coinsInBag.Add(CoinsOnBoard[i][j]);
                    CoinsOnBoard[i][j] = ColourEnum.Grey;
                }
            }
        }
        public CoinBoard(ColourEnum[][] coinsOnBoard, List<ColourEnum> coinsInBag, int scrollCount)
        {
            ScrollCount = scrollCount;
            _coinsInBag = coinsInBag;
            CoinsOnBoard = coinsOnBoard;
        }
        public void ShuffleBoard()
        {
            PackTheCoins();
            FillTheBoard();
        }

        public void FillTheBoard()
        {
            string direction = "left"; //"up" "right" "down"
            int counter = 0;
            int i = 2; int j = 2;

            if (_coinsInBag.Count > 0)
                drawOneifNeeded(i, j);
            while (_coinsInBag.Count > 0)
            {
                drawOneifNeeded(i, j);
                if (counter == 1 || counter == 9) direction = "up";
                if (counter == 2 || counter == 12) direction = "right";
                if (counter == 4 || counter == 16) direction = "down";
                if (counter == 6 || counter == 20) direction = "left";
                if (direction == "left") j--;
                if (direction == "up") i--;
                if (direction == "right") j++;
                if (direction == "down") i++;
                counter++;
            }
            void drawOneifNeeded(int i, int j)
            {
                if (CoinsOnBoard[i][j] != ColourEnum.Grey || _coinsInBag.Count == 0) return;
                var coin = _coinsInBag[random.Next(_coinsInBag.Count)];
                _coinsInBag.Remove(coin);
                CoinsOnBoard[i][j] = coin;
            }
        }

        public DefaultResponse TakeScroll()
        {
            if (this.ScrollCount < 1) return new DefaultResponse(false, "No scrolls on the board!");
            ScrollCount--;
            return new DefaultResponse(true);
        }
        public DefaultResponse ExchangeScroll(CoinRequest coinRequest)
        {
            if (coinRequest.colour == ColourEnum.Gold) return new DefaultResponse(false, "Gold coin cannot be exchanged for a scroll!");
            if (!AreCoinRequestColoursProper([coinRequest])) return new DefaultResponse(false, "Requested coin position is not proper!");
            ScrollCount++;
            CoinsOnBoard[coinRequest.i][coinRequest.j] = ColourEnum.Grey;
            return new DefaultResponse(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CoinsRequested"></param>
        /// <returns></returns>
        public DefaultResponse TakeCoins(ICollection<CoinRequest> CoinsRequested)
        {
            if (CoinsRequested.Any(x=>x.colour== ColourEnum.Gold)) return new DefaultResponse(false, "Gold coin cannot be taken from the board!");
            if (!AreCoinRequestColoursProper(CoinsRequested)) return new DefaultResponse(false, "Requested coin colours don't match!");
            if (!AreCoinRequestPositionProper(CoinsRequested)) return new DefaultResponse(false, "Requested coin positions are not proper!");
            foreach (CoinRequest coin in CoinsRequested)
            {
                CoinsOnBoard[coin.i][coin.j] = ColourEnum.Grey;
            }
            return new DefaultResponse(true);
        }

        private bool AreCoinRequestColoursProper(ICollection<CoinRequest> CoinsRequested)
        {
            foreach (CoinRequest coin in CoinsRequested)
            {
                if (coin.colour != CoinsOnBoard[coin.i][coin.j] || coin.colour == ColourEnum.Grey)
                    return false;
            }
            return true;
        }
        private bool AreCoinRequestPositionProper(ICollection<CoinRequest> CoinsRequested)
        {
            int coinCount = CoinsRequested.Count();
            if (coinCount == 1) return true;

            var coinsSorted = CoinsRequested.OrderBy(x => x.i).ThenBy(x => x.j).ToList();
            int di = coinsSorted[1].i - coinsSorted[0].i;
            int dj = coinsSorted[1].j - coinsSorted[0].j;
            if (Math.Abs(di) > 1 || Math.Abs(dj) > 1) return false;

            if (di == 0 && dj == 0) return false;

            //if (Math.Abs(di) == 2 || Math.Abs(dj) == 2)
            //{
            //    if (coinCount == 3) return false;
            //    int emptyI = coinsSorted[0].i + di / 2;
            //    int emptyj = coinsSorted[0].j + dj / 2;
            //    //if (CoinsOnBoard[emptyI][emptyj] != ColourEnum.Grey) return false;
            //    else return true;
            //}
            if (coinCount == 2) return true;

            if (coinsSorted[2].i - coinsSorted[1].i != di) return false;
            if (coinsSorted[2].j - coinsSorted[1].j != dj) return false;
            return true;
        }
        public void PutCoinsInTheBag(IDictionary<ColourEnum,int> coins)
        {
            if (coins == null) return;
            foreach (var coin in coins)
            {
                for (int i = 0; i < coin.Value; i++)
                {
                    _coinsInBag.Add(coin.Key);
                }
            }
        }
        public void PutCoinsInTheBag(ColourEnum[] coins)
        {
            if (coins == null) return;
            foreach (var coin in coins)
            {
                _coinsInBag.Add(coin);
            }
        }
    }
}
