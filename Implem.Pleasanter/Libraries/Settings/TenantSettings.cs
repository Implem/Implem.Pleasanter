using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class TenantSettings
    {
        public BackgroundServerScripts BackgroundServerScripts { get; set; }

        public TenantSettings() { }

        public TenantSettings(Context context)
        {
            BackgroundServerScripts = new BackgroundServerScripts(context: context);
        }

        public bool InitialValue(Context context)
        {
            return RecordingJson(context: context) == "{}";
        }

        internal string RecordingJson(Context context)
        {
            return this.ToJson();
        }

        internal TenantSettings GetTenantSettings(Context context, string value)
        {
            if (value.IsNullOrEmpty()) return null;
            return value.Deserialize<TenantSettings>();
        }
    }
}
