using Implem.Libraries.Utilities;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class UserSettings
    {
        public bool? DisableTopSiteCreation;
        public bool? DisableGroupAdmin;

        public string RecordingJson()
        {
            var us = new UserSettings();
            if (DisableTopSiteCreation == true)
            {
                us.DisableTopSiteCreation = DisableTopSiteCreation;
            }
            if (DisableGroupAdmin == true)
            {
                us.DisableGroupAdmin = DisableGroupAdmin;
            }
            return us.ToJson();
        }

        public bool InitialValue()
        {
            return RecordingJson() == "[]";
        }
    }
}