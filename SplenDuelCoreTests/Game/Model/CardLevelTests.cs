using Splenduel.Core.Game.Model;
using SplenDuelCoreTests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplenDuelCoreTests.Game.Model
{
    internal class CardLevelTests
    {
        TestCardsGenerator cardGen = new();
        [SetUp] public void SetUp() { }
        [TearDown] public void TearDown() { }
        [Test]
        public void Ctor_provides_proper_exposed()
        {
            var cards = cardGen.Cards.Take(3).ToList();
            var testCards = cardGen.Cards.Take(3).ToList();
            CardLevel lvl = new CardLevel(cards, 3);
            Assert.Multiple(() =>
            {
                Assert.That(lvl.Exposed.Length == 3);
                Assert.IsNotNull(lvl.Exposed.Contains(testCards[0]) && lvl.Exposed.Contains(testCards[1]) && lvl.Exposed.Contains(testCards[2]));
            });
        }
        [Test]
        public void Ctor_2_provides_proper_exposed_and_deck()
        {
            CardLevel lvl;
            lvl = CreateCardLevelWith3inExposedAnd2inDeck();
            Assert.Multiple(() =>
            {
                Assert.That(lvl.Exposed.Length == 3);
                Assert.That(lvl.DeckCount == 2);
            });
        }
        [Test]
        public void Proper_Cards_are_exposed()
        {
            CardLevel lvl;
            lvl = CreateCardLevelWith3inExposedAnd2inDeck();
            var testCards = cardGen.Cards.ToList();
            var card0a = lvl.TakeCard(0,out _);
            var card1a = lvl.TakeCard(1, out _);
            var card2a = lvl.TakeCard(2, out _);
            Assert.Multiple(() =>
            {
                Assert.That(lvl.DeckCount, Is.EqualTo(0));
                Assert.That(card0a.IsValueEqualTo((Card)testCards[0]));
                Assert.That(card1a.IsValueEqualTo((Card)testCards[1]));
                Assert.That(card2a.IsValueEqualTo((Card)testCards[2]));
            });
        }
        [Test]
        public void Proper_Cards_are_drawn()
        {
            CardLevel lvl;
            lvl = CreateCardLevelWith3inExposedAnd2inDeck();
            var testCards = cardGen.SkipXCardsAsList(3);
            var card0a = lvl.TakeCard(0, out Card CardDrawn0);
            var card0b = lvl.TakeCard(0, out Card CardDrawn1);
            var card0c = lvl.TakeCard(0, out Card CardDrawn2);
            Assert.Multiple(() =>
            {
                Assert.That(CardDrawn0.IsValueEqualTo((Card)testCards.First()) || CardDrawn0.IsValueEqualTo((Card)testCards.Last()));
                Assert.That(CardDrawn1.IsValueEqualTo((Card)testCards.First()) || CardDrawn1.IsValueEqualTo((Card)testCards.Last()));
                Assert.That(CardDrawn0, Is.Not.EqualTo(CardDrawn1));
                Assert.That(CardDrawn2, Is.Null);
                Assert.That(card0b,Is.EqualTo(CardDrawn0));
            });
        }

        private CardLevel CreateCardLevelWith3inExposedAnd2inDeck()
        {
            CardLevel level;
            var cards = cardGen.GetXCardsAsArray(3);
            var cards2 = cardGen.SkipXCardsAsList(3);
            level = new(cards, cards2);
            return level;
        }
    }
}
