using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Splenduel.InMemoryStorage
{
    public static class CardsImporter
    {
        internal static async Task<string[]> GetAllFromLevel(int lvl)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] lines;
            switch (lvl)
            {
                case 1:
                    lines = await File.ReadAllLinesAsync($"{path}/Cards_lvl1.csv");
                    break;
                case 2:
                    lines = await File.ReadAllLinesAsync($"{path}/Cards_lvl2.csv");
                    break;
                case 3:
                    lines = await File.ReadAllLinesAsync($"{path}/Cards_lvl3.csv");
                    break;
                default:
                    throw new Exception($"Cards level: {lvl} not possible");
            }
            return lines.Skip(1).ToArray();
        }
    }
}
