using Splenduel.Core.Game.Services;

namespace SplenDuelCoreTests
{
    public class CardGeneratorTests
    {
        CardGenerator cardGen;
        [SetUp]
        public void Setup()
        {
            cardGen = new CardGenerator();
        }

        [Test]
        public void AssertThereAreThreeDifferentCardsShown()
        {
            Assert.That(cardGen.GetLevel1Exposed().Count, Is.EqualTo(3));
        }
        [Test]
        public void AssertOnlyDrawnCardIsChanged()
        {
            var cards = cardGen.GetLevel1Exposed();            
            var cardsAfter = cardGen.TakeCard(1);
            Assert.Multiple(() =>
            {
                Assert.That(cards.ElementAt(0), Is.EqualTo(cardsAfter.ElementAt(0)));
                Assert.That(cards.ElementAt(1), Is.Not.EqualTo(cardsAfter.ElementAt(1)));
                Assert.That(cards.ElementAt(2), Is.EqualTo(cardsAfter.ElementAt(2)));
            });
        }
        [Test]
        public void DeckSizeShrinksWithTakingCards()
        {
            int no1 = cardGen.GetDeckCount();
            _ = cardGen.TakeCard(1);
            int no2 = cardGen.GetDeckCount();
            Assert.That(no1, Is.EqualTo(no2+1));
        }
    }
}