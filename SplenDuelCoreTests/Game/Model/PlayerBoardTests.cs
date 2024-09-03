using Splenduel.Core.Game.Model;
using SplenDuelCoreTests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplenDuelCoreTests.Game.Model
{
    internal class PlayerBoardTests
    {
        private TestCardsGenerator _cards;
        [SetUp]
        public void SetUp()
        {
            _cards = new TestCardsGenerator();
        }
        [TearDown] public void TearDown() { }
        [Test]
        public void Basic_Constructor_Initializes_Properties_Correctly_Second_Player()
        {
            var playerBoard = new PlayerBoard();
            Assert.Multiple(() =>
            {
                Assert.That(playerBoard.ScrollsCount, Is.EqualTo(1));
                Assert.That(playerBoard.PointsByColour, Is.Not.Null);
                Assert.That(playerBoard.TotalPoints, Is.EqualTo(0));
            });
        }

        [Test]
        public void Basic_Constructor_Initializes_Properties_Correctly_First_Player()
        {
            var playerBoard = new PlayerBoard(true);
            Assert.Multiple(() =>
            {
                Assert.That(playerBoard.ScrollsCount, Is.EqualTo(0));
                Assert.That(playerBoard.PointsByColour, Is.Not.Null);
                Assert.That(playerBoard.TotalPoints, Is.EqualTo(0));
            });
        }

        [Test]
        public void Advanced_Constructor_Initializes_Properties_Properly()
        {
            int scrollsCount = 2;
            var ownedCards = _cards.SkipXCardsAsList(3);
            var hiddenCards = _cards.GetXCardsAsList(2);
            var coins = new Dictionary<ColourEnum, int>()
            {
                [ColourEnum.Black] = 2,
                [ColourEnum.Red] = 1
            };
            var playerBoard = new PlayerBoard(2, ownedCards.ToList(), hiddenCards.ToList(), coins.ToDictionary<ColourEnum, int>());
            Assert.Multiple(() =>
            {
                Assert.That(playerBoard.TotalPoints, Is.EqualTo(ownedCards.Sum(x => x.Points)));
                Assert.That(playerBoard.Crowns, Is.EqualTo(ownedCards.Sum(x => x.Crowns)));
                Assert.That(playerBoard.OwnedCards, Is.EqualTo(ownedCards));
                Assert.That(playerBoard.PointsByColour[ColourEnum.Red], Is.EqualTo(2));
                Assert.That(playerBoard.MiningValues[ColourEnum.Red], Is.EqualTo(1));
                Assert.That(playerBoard.MiningValues[ColourEnum.White], Is.EqualTo(1));
                Assert.That(playerBoard.Coins.TryGetValue(ColourEnum.Gold, out _), Is.False);
            });
        }

        [Test]
        public async Task Card_Not_Afforded_Cannot_Be_Bought()
        {
            int scrollsCount = 2;
            var ownedCards = _cards.SkipXCardsAsList(3);
            var hiddenCards = _cards.GetXCardsAsList(2);
            var coins = new Dictionary<ColourEnum, int>()
            {
                [ColourEnum.Black] = 2,
                [ColourEnum.Red] = 1
            };
            var playerBoard = new PlayerBoard(2, ownedCards.ToList(), hiddenCards.ToList(), coins.ToDictionary<ColourEnum, int>());
            var card = new Card(1,1, ColourEnum.Black, 1, 1, 1, new CardCost(White: 1, Blue: 0, Green: 1, Red: 1, Black: 0, Pink: 0));
            var response = await playerBoard.BuyCard(card);
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(false));
                Assert.That(response.Message, Is.EqualTo("Payment was not possible!"));
            });
        }
        [Test]
        public async Task Card_Afforded_With_Gold_Is_Bought()
        {
            int scrollsCount = 2;
            var ownedCards = _cards.SkipXCardsAsList(3);
            var hiddenCards = _cards.GetXCardsAsList(2);
            var coins = new Dictionary<ColourEnum, int>()
            {
                [ColourEnum.Black] = 2,
                [ColourEnum.Red] = 1,
                [ColourEnum.Gold] = 1
            };
            var playerBoard = new PlayerBoard(2, ownedCards.ToList(), hiddenCards.ToList(), coins.ToDictionary<ColourEnum, int>());
            var card = new Card(1, ColourEnum.Black, points: 1, miningPower: 1, crowns: 1, new CardCost(White: 1, Blue: 0, Green: 1, Red: 1, Black: 0, Pink: 0));
            var response = await playerBoard.BuyCard(card);
            Assert.Multiple(() =>
            {
                Assert.That(response.Success, Is.EqualTo(true));
                Assert.That(response.Message, Is.EqualTo(""));
                Assert.That(playerBoard.OwnedCards.Count, Is.EqualTo(ownedCards.Count + 1));
                Assert.That(playerBoard.TotalPoints, Is.EqualTo(ownedCards.Sum(x => x.Points) + 1));
            });
        }

        [Test]
        public void AddCoin_Adds_Coin_Correctly()
        {
            // Arrange
            var playerBoard = new PlayerBoard();
            var blue = ColourEnum.Blue;
            var green = ColourEnum.Green;

            // Act
            playerBoard.AddCoin(blue).Wait();
            playerBoard.AddCoin(blue).Wait();
            playerBoard.AddCoin(green).Wait();


            // Assert
            Assert.IsTrue(playerBoard.Coins.ContainsKey(blue));
            Assert.That(playerBoard.Coins[blue], Is.EqualTo(2));
            Assert.That(playerBoard.Coins[green], Is.EqualTo(1));
        }

    }
}
