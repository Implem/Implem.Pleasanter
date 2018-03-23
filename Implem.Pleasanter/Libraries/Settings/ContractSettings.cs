using Implem.Libraries.Utilities;
using System;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class ContractSettings
    {
        public string Name;
        public string DisplayName;
        public int? Users;
        public long? Sites;
        public long? Items;
        public int? StorageSize;
        public bool? Import;
        public bool? Export;
        public bool? Notice;
        public bool? Remind;
        public bool? Mail;
        public bool? Style;
        public bool? Script;
        public bool? Api;
        public DateTime Deadline;

        public ContractSettings()
        {
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public string RecordingJson()
        {
            return null;
        }

        public bool InitialValue()
        {
            return this.ToJson() == "[]";
        }
    }
}