namespace Implem.Pleasanter.Libraries.Responses
{
    public class ControlData
    {
        public string Text;
        public string Css;
        public string Style;

        public ControlData(string text, string css = null, string style = null)
        {
            Text = text;
            Css = css;
            Style = style;
        }
    }
}