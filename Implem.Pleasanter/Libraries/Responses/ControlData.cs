using Implem.Libraries.Utilities;
namespace Implem.Pleasanter.Libraries.Responses
{
    public class ControlData
    {
        public string Text;
        public string Title;
        public string Css;
        public string Style;

        public ControlData(
            string text, string title = null, string css = null, string style = null)
        {
            Text = text;
            Title = title;
            Css = css;
            Style = style;
        }

        public string DisplayValue()
        {
            return !Text.IsNullOrEmpty()
                ? Text
                : Displays.NotSet();
        }
    }
}