using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Libraries.Classes
{
    public class TextData : Dictionary<string, string>
    {
        public string Value;

        public TextData(string value, char parameterDelimiter, char dataDelimiter)
        {
            Value = value;
            this.AddRange(value.Split(parameterDelimiter)
                .Where(o => o.Trim() != string.Empty)
                .ToDictionary(
                    o => o.Substring(0, o.IndexOf(dataDelimiter)).ToLower().Trim(),
                    o => o.Substring(o.IndexOf(dataDelimiter) + 1)));
        }
    }
}
