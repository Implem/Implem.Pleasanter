using System.Linq;
using System.Text;
namespace Implem.Libraries.Utilities
{
    public static class StringBuilders
    {
        public static void Append(this StringBuilder self, params string[] strings)
        {
            strings.ForEach(str => self.Append(str));
        }
    }
}
