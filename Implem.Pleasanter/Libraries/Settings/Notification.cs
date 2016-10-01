using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class Notification
    {
        public Types Type;
        public string Address;
        public List<string> MonitorChangesColumns;

        public enum Types : int
        {
            Mail = 1,
            Slack = 2
        }

        public Notification(Types type, string address, List<string> monitorChangesColumns)
        {
            Type = type;
            Address = address;
            MonitorChangesColumns = monitorChangesColumns;
            MonitorChangesColumns = monitorChangesColumns;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public void Update(string address, List<string> columns)
        {
            Address = address;
            MonitorChangesColumns = columns;
        }
    }
}