using System;
namespace Implem.Pleasanter.Libraries.Responses
{
    [Serializable]
    public class Message
    {
        public string Id;
        public string Text;
        public string Css;

        public Message(string id, string text, string css)
        {
            Id = id;
            Text = text;
            Css = css;
        }
    }
}