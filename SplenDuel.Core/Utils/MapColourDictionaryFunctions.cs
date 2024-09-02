using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Utils
{
    public static class MapColourDictionaryFunctions
    {
        public static IDictionary<ColourEnum, int> CreateColourEnumZeroDictionary()
        {
            return new Dictionary<ColourEnum, int>
            {
                { ColourEnum.White, 0 },
                { ColourEnum.Blue, 0 },
                { ColourEnum.Green, 0 },
                { ColourEnum.Red, 0 },
                { ColourEnum.Black, 0 },
                { ColourEnum.Pink, 0 },
                { ColourEnum.Gold, 0 },
            };
        }
    }
}
