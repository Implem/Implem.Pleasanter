namespace Implem.Pleasanter.Libraries.Responses
{
    public class Message
    {
        public string Html;
        public string Status;

        public Message(string html, string status)
        {
            Html = html;
            Status = status;
        }
    }
}