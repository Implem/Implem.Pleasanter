using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class Choice
    {
        public string Value;
        public string Text;
        public string TextMini;
        public string CssClass;
        public string Style;

        public Choice(string value, string text)
        {
            Value = value;
            Text = text;
            TextMini = text;
        }

        public Choice(string choice)
        {
            if (choice != null)
            {
                var array = choice.Split(',');
                Value = array._1st();
                Text = Strings.CoalesceEmpty(array._2nd(), Value);
                TextMini = Strings.CoalesceEmpty(array._3rd(), Text);
                CssClass = array._4th();
                Style = array._5th();
            }
        }

        public string SearchText()
        {
            return new List<string>()
            {
                Value,
                Text,
                TextMini
            }.Distinct().Join(" ");
        }
    }
}