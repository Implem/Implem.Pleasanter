using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Implem.Libraries.Utilities
{
    public static class Regexes
    {
        public static string RegexFirst(
            this string self,
            string pattern,
            RegexOptions regexOptions = RegexOptions.Singleline)
        {
            foreach (Match match in self.RegexMatches(pattern, regexOptions))
            {
                return match.Value;
            }
            return string.Empty;
        }

        public static bool RegexExists(
            this string self,
            string pattern,
            RegexOptions regexOptions = RegexOptions.Singleline)
        {
            return self.RegexMatches(pattern, regexOptions).Count > 0;
        }

        public static MatchCollection RegexMatches(
            this string self,
            string pattern,
            RegexOptions regexOptions = RegexOptions.Singleline)
        {
            return Regex.Matches(self, pattern, regexOptions);
        }

        public static IEnumerable<string> RegexValues(
            this string self,
            string pattern,
            RegexOptions regexOptions = RegexOptions.Singleline)
        {
            foreach (Match match in Regex.Matches(self, pattern, regexOptions))
            {
                yield return match.Value;
            }
        }
    }
}
