using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Responses
{
    public class ControlData
    {
        public string Text;
        public string Title;
        public string Css;
        public string Style;
        public int? Order;
        public Dictionary<string, string> Attributes;

        public ControlData(
            string text,
            string title = null,
            string css = null,
            string style = null,
            int? order = null,
            Dictionary<string, string> attributes = null)
        {
            Text = text;
            Title = title;
            Css = css;
            Style = style;
            Order = order;
            Attributes = attributes;
        }

        public string DisplayValue(Context context)
        {
            return !Text.IsNullOrEmpty()
                ? Text
                : Displays.NotSet(context: context);
        }
    }
}