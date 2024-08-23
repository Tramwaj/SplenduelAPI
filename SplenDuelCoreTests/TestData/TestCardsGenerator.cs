using Splenduel.Core.Game.Model;
using Splenduel.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplenDuelCoreTests.TestData
{
    internal class TestCardsGenerator
    {
        private string[] _cardCsv = ["White;1;;;;2;2;;;0;Pickup", "Blue;1;;;2;2;;1;;0;Turn", "Multi;1;4;;;;;1;;1;", "White;1;;4;;3;;;1;;Steal", "Red;1;;;2;4;;1;2;;Scroll"];
        //private Card[] _cards = 
        public string[] CardCsv { get => _cardCsv; set => _cardCsv = value; }
        public List<Card> Cards { get; private set; }
        public Card[] GetXCardsAsArray(int x) => Cards.Take(x).ToArray();
        public List<Card> GetXCardsAsList(int x) => Cards.Take(x).ToList();
        public List<Card> SkipXCardsAsList(int x) => Cards.Skip(x).ToList();

        public TestCardsGenerator()
        {
            Cards = _cardCsv.Select((x, i) => CardFromCsvReader.Read(x, i, 1)).ToList();
        }
    }
}
