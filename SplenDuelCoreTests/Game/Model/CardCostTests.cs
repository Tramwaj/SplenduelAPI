using Splenduel.Core.Game.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplenDuelCoreTests.Game.Model
{
    internal class CardCostTests
    {
        [SetUp] public void SetUp() { }
        [TearDown] public void TearDown() { }
        [Test]
        public void Idividual_Cost_Is_Read() {
            CardCost cardCost = new CardCost(1,0,0,0,0,0);
            Assert.That(cardCost.White, Is.EqualTo(1));
        }
        [Test]
        public void Individual_Costs_Are_Read_Full_Cost()
        {
            CardCost cost = new CardCost(1,2,3,4,5,6);
            Assert.Multiple(() =>
            {
                Assert.That(cost.White, Is.EqualTo(1));
                Assert.That(cost.Blue, Is.EqualTo(2));
                Assert.That(cost.Green, Is.EqualTo(3));
                Assert.That(cost.Red, Is.EqualTo(4));
                Assert.That(cost.Black, Is.EqualTo(5));
                Assert.That(cost.Pink, Is.EqualTo(6));
            });
        }
        [Test]
        public void Empty_Costs_Are_Equal_0()
        {
            CardCost cost = new CardCost(0, 0, 1, 0, 0,0);
            Assert.Multiple(() =>
            {
                Assert.That(cost.White, Is.EqualTo(0));
                Assert.That(cost.Blue, Is.EqualTo(0));
                Assert.That(cost.Green, Is.EqualTo(1));
                Assert.That(cost.Red, Is.EqualTo(0));
                Assert.That(cost.Black, Is.EqualTo(0));
                Assert.That(cost.Pink, Is.EqualTo(0));
            });
        }
    }
}
