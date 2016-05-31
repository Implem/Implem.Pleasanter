using System.Linq;
namespace Implem.Pleasanter.Libraries.Styles
{
    public static class Css
    {
        public static string Class(string _default, string additional)
        {
            if (additional == string.Empty || additional == null)
            {
                return _default.Trim();
            }
            else if (_default.EndsWith(" "))
            {
                return (_default + additional).Trim();
            }
            else if (additional.Substring(0, 1) == " ")
            {
                return _default.Trim() + additional;
            }
            else
            {
                return additional;
            }
        }

        public static bool ContainsClass(this string self, string css)
        {
            return self.Split(' ').Any(o => o == css);
        }
    }
}