using System.Collections.Generic;
using System.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Unique
    {
        public static string New(IEnumerable<string> existing, string name, int start = 1)
        {
            var number = start;
            while(existing.Any(o => o == name + number))
            {
                number++;
            }
            return name + number;
        }
    }
}
