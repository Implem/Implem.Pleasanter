using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class BackgroundServerScripts
    {
        public SettingList<BackgroundServerScript> Scripts = new SettingList<BackgroundServerScript>();
        public int TenantId;

        public BackgroundServerScripts()
        {
            TenantId = 0;
        }

        public BackgroundServerScripts(int tenatId = 0)
        {
            TenantId = tenatId;
        }

        public BackgroundServerScripts(Context context)
        {
            TenantId = context.TenantId;
        }

        public string RecordingJson(Context context)
        {
            return this.ToJson();
        }
    }
}
