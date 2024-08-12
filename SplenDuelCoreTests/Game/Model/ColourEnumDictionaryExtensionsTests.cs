using Splenduel.Core.Game.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplenDuelCoreTests.Game.Model
{
    internal class ColourEnumDictionaryExtensionsTests
    {
        private IDictionary<ColourEnum, int> _dict;
        [SetUp] public void SetUp() { }
        [TearDown] public void TearDown() { _dict = null; }
        [Test]
        public void Dictionary_Is_Created_If_I_tWas_Null()
        {
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Black, 1);
            Assert.IsNotNull(_dict);
        }
        [Test]
        public void Dictionary_Is_Properly_Filled_If_It_Was_Null()
        {
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Black, 1);
            Assert.That(_dict[ColourEnum.Black], Is.EqualTo(1));
        }
        [Test]
        public void Dictionary_Is_Properly_Filled()
        {
            _dict = new Dictionary<ColourEnum,int>();
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Black, 1);
            Assert.That(_dict[ColourEnum.Black], Is.EqualTo(1));
        }
        [Test]
        public void Dictionary_Is_Properly_Increased()
        {
            _dict = new Dictionary<ColourEnum,int>();
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Black, 1);
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Black, 2);
            Assert.That(_dict[ColourEnum.Black], Is.EqualTo(3));
        }
        [Test]
        public void Dictionary_Is_Properly_Filled_With_Different_Keys()
        {
            _dict = new Dictionary<ColourEnum,int>();
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Black, 1);
            _dict=_dict.CreateOrAddIfExists(ColourEnum.Red, 2);
            Assert.That(_dict[ColourEnum.Black], Is.EqualTo(1));
            Assert.That(_dict[ColourEnum.Red], Is.EqualTo(2));
        }
    }
}
