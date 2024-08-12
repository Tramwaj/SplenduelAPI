using Splenduel.Core.Game.Model;
using Splenduel.Core.Game.Services;
using Splenduel.Core.Global;
using SplenDuelCoreTests.TestData;

namespace SplenDuelCoreTests
{
    public class CardFromCsvReaderTests
    {
        TestCardsGenerator cards = new();
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Card_Is_Read_Properly_1()
        {
            var cardCsv = cards.CardCsv[0];
            Card cardFromCsv = CardFromCsvReader.Read(cardCsv, 1);
            Assert.Multiple(() =>
            {
                Assert.That(cardFromCsv, Is.Not.Null);
                Assert.That(cardFromCsv.Colour, Is.EqualTo(ColourEnum.White));
                Assert.That(cardFromCsv.MiningPower, Is.EqualTo(1));
                Assert.That(cardFromCsv.Points, Is.EqualTo(0));
                Assert.That(cardFromCsv.Crowns, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.White, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Blue, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Green, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Red, Is.EqualTo(2));
                Assert.That(cardFromCsv.Cost.Black, Is.EqualTo(2));
                Assert.That(cardFromCsv.Cost.Pink, Is.EqualTo(0));
                Assert.That(cardFromCsv.Action, Is.EqualTo(CardActionEnum.Pickup));
            });
        }
        [Test]
        public void Card_Is_Read_Properly_2()
        {
            var cardCsv = cards.CardCsv[1];
            Card cardFromCsv = CardFromCsvReader.Read(cardCsv, 1);
            Assert.Multiple(() =>
            {
                Assert.That(cardFromCsv, Is.Not.Null);
                Assert.That(cardFromCsv.Colour, Is.EqualTo(ColourEnum.Blue));
                Assert.That(cardFromCsv.MiningPower, Is.EqualTo(1));
                Assert.That(cardFromCsv.Points, Is.EqualTo(0));
                Assert.That(cardFromCsv.Crowns, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.White, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Blue, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Green, Is.EqualTo(2));
                Assert.That(cardFromCsv.Cost.Red, Is.EqualTo(2));
                Assert.That(cardFromCsv.Cost.Black, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Pink, Is.EqualTo(1));
                Assert.That(cardFromCsv.Action, Is.EqualTo(CardActionEnum.Turn));
            });
        }
        [Test]
        public void Card_Is_Read_Properly_3()
        {
            var cardCsv = cards.CardCsv[2];
            Card cardFromCsv = CardFromCsvReader.Read(cardCsv, 1);
            Assert.Multiple(() =>
            {
                Assert.That(cardFromCsv, Is.Not.Null);
                Assert.That(cardFromCsv.Colour, Is.EqualTo(ColourEnum.Multi));
                Assert.That(cardFromCsv.MiningPower, Is.EqualTo(1));
                Assert.That(cardFromCsv.Points, Is.EqualTo(0));
                Assert.That(cardFromCsv.Crowns, Is.EqualTo(1));
                Assert.That(cardFromCsv.Cost.White, Is.EqualTo(4));
                Assert.That(cardFromCsv.Cost.Blue, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Green, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Red, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Black, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Pink, Is.EqualTo(1));
                Assert.That(cardFromCsv.Action, Is.EqualTo(CardActionEnum.None));
            });
        }
        [Test]
        public void Card_Is_Read_Properly_4()
        {
            var cardCsv = cards.CardCsv[3];
            Card cardFromCsv = CardFromCsvReader.Read(cardCsv, 1);
            Assert.Multiple(() =>
            {
                Assert.That(cardFromCsv, Is.Not.Null);
                Assert.That(cardFromCsv.Colour, Is.EqualTo(ColourEnum.White));
                Assert.That(cardFromCsv.MiningPower, Is.EqualTo(1));
                Assert.That(cardFromCsv.Points, Is.EqualTo(1));
                Assert.That(cardFromCsv.Crowns, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.White, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Blue, Is.EqualTo(4));
                Assert.That(cardFromCsv.Cost.Green, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Red, Is.EqualTo(3));
                Assert.That(cardFromCsv.Cost.Black, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Pink, Is.EqualTo(0));
                Assert.That(cardFromCsv.Action, Is.EqualTo(CardActionEnum.Steal));
            });
        }
        [Test]
        public void Card_Is_Read_Properly_5()
        {
            var cardCsv = cards.CardCsv[4];
            Card cardFromCsv = CardFromCsvReader.Read(cardCsv, 1);
            Assert.Multiple(() =>
            {
                Assert.That(cardFromCsv, Is.Not.Null);
                Assert.That(cardFromCsv.Colour, Is.EqualTo(ColourEnum.Red));
                Assert.That(cardFromCsv.MiningPower, Is.EqualTo(1));
                Assert.That(cardFromCsv.Points, Is.EqualTo(2));
                Assert.That(cardFromCsv.Crowns, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.White, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Blue, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Green, Is.EqualTo(2));
                Assert.That(cardFromCsv.Cost.Red, Is.EqualTo(4));
                Assert.That(cardFromCsv.Cost.Black, Is.EqualTo(0));
                Assert.That(cardFromCsv.Cost.Pink, Is.EqualTo(1));
                Assert.That(cardFromCsv.Action, Is.EqualTo(CardActionEnum.Scroll));
            });
        }        
    }
}