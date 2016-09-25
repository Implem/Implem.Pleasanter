using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Notification
    {
        public Types Type;
        public string Address;
        public List<string> Columns;

        public enum Types : int
        {
            Mail = 1,
            Slack = 2
        }

        public Notification(int type, string address, List<string> columns)
        {
            Type = (Types)type;
            Address = address;
            Columns = columns;
        }

        public void Update(string address, List<string> columns)
        {
            Address = address;
            Columns = columns;
        }
    }
}