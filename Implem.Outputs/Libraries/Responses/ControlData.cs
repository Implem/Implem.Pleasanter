namespace Implem.Pleasanter.Libraries.Responses
{
    public class ControlData
    {
        public string Text;
        public string Css;
        public string Style;

        public ControlData(string text, string css = "", string style = "")
        {
            Text = text;
            Css = css;
            Style = style;
        }
    }
}