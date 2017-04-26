using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using System;
using System.Linq;
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
        public bool? Import;
        public bool? Export;
        public bool? Notice;
        public bool? Mail;
        public bool? Style;
        public bool? Script;
        public DateTime Deadline;

        public ContractSettings()
        {
        }

        public ContractSettings(string name)
        {
            Name = name;
            Init();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            Init();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        private void Init()
        {
            var _default = GetDefault();
            if (_default != null)
            {
                Users = Users ?? _default.Users;
                Sites = Sites ?? _default.Sites;
                Items = Items ?? _default.Items;
                Import = Import ?? _default.Import;
                Export = Export ?? _default.Export;
                Notice = Notice ?? _default.Notice;
                Mail = Mail ?? _default.Mail;
                Style = Style ?? _default.Style;
                Script = Script ?? _default.Script;
            }
            else
            {
                Users = 0;
                Sites = 0;
                Items = 0;
                Import = true;
                Export = true;
                Notice = true;
                Mail = true;
                Style = true;
                Script = true;
            }
        }

        private ContractType GetDefault()
        {
            return Parameters.ContractTypes.FirstOrDefault(o => o.Name == Name);
        }

        public string RecordingJson()
        {
            var _default = GetDefault();
            if (_default != null)
            {
                if (Users == _default.Users) Users = null;
                if (Sites == _default.Sites) Sites = null;
                if (Items == _default.Items) Items = null;
                if (Import == _default.Import) Import = null;
                if (Export == _default.Export) Export = null;
                if (Notice == _default.Notice) Notice = null;
                if (Mail == _default.Mail) Mail = null;
                if (Style == _default.Style) Style = null;
                if (Script == _default.Script) Script = null;
            }
            return null;
        }
    }
}