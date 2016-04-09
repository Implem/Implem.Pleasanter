using Implem.Libraries.Utilities;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Choice : List<string>
    {
        public string SelectedValue;

        public Choice(string choice, string selectedValue)
        {
            if (choice != null)
            {
                this.AddRange(choice.Split(','));
            }
            SelectedValue = selectedValue;
        }

        public string Value()
        {
            return this._1st();
        }

        public string Text()
        {
            return Strings.CoalesceEmpty(this._2nd(), this._1st());
        }

        public string TextMini()
        {
            return Strings.CoalesceEmpty(this._3rd(), this._2nd(), this._1st(), SelectedValue);
        }

        public string CssClass()
        {
            return this._4th();
        }

        public string Style()
        {
            return this._5th();
        }
    }
}