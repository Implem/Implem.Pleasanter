using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Choice
    {
        public string Value;
        public string Text;
        public string TextMini;
        public string CssClass;
        public string Style;

        public Choice(
            string value,
            string text,
            string textMini = null,
            string cssClass = null)
        {
            Value = value;
            Text = !text.IsNullOrEmpty()
                ? text
                : Value;
            TextMini = !textMini.IsNullOrEmpty()
                ? textMini
                : Text;
            CssClass = cssClass;
        }

        public Choice(string choice, bool raw = false, string value = null)
        {
            if (choice != null)
            {
                if (raw)
                {
                    Value = value ?? choice;
                    Text = choice;
                    TextMini = choice;
                    CssClass = string.Empty;
                    Style = string.Empty;
                }
                else
                {
                    var array = Regex.Split(
                        choice,
                        Parameters.General.ChoiceSplitRegexPattern
                        ).Select(item =>
                            Regex.Replace(
                                item,
                                Parameters.General.ChoiceReplaceRegexPattern,
                                Parameters.General.ChoiceReplaceRegexReplacement
                            )
                        );
                    Value = array._1st();
                    Text = Strings.CoalesceEmpty(array._2nd(), Value);
                    TextMini = Strings.CoalesceEmpty(array._3rd(), Text);
                    CssClass = array._4th();
                    Style = array._5th();
                }
            }
        }
    }
}