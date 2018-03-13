using Implem.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.Styles
{
    public static class Css
    {
        public static string Class(string _default, string additional)
        {
            if (additional.IsNullOrEmpty())
            {
                return _default?.Trim();
            }
            else if (_default?.EndsWith(" ") == true)
            {
                return (_default + additional).Trim();
            }
            else if (additional.Substring(0, 1) == " ")
            {
                return (_default?.Trim() + additional).Trim();
            }
            else
            {
                return additional;
            }
        }
    }
}