using System;
namespace Implem.Pleasanter.Libraries.Responses
{
    [Serializable]
    public class Message
    {
        public string Text;
        public string Css;

        public Message(string text, string css)
        {
            Text = text;
            Css = css;
        }
    }
}