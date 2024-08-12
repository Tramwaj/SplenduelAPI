using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.Core.Game.Model.Extensions
{
    public static class ColourEnumDictionaryExtensions
    {
        public static IDictionary<ColourEnum, int> CreateOrAddIfExists(this IDictionary<ColourEnum,int> dict, ColourEnum colour, int value)
        {
            if (dict == null) {  dict = new Dictionary<ColourEnum,int>(); dict.Add(colour, value); return dict; }
            if (dict.ContainsKey(colour)) { dict[colour] += value; }
            else dict.Add(colour, value);
            return dict;
        }
    }
}
