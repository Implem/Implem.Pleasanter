using Implem.Libraries.Utilities;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class UserSettings
    {
        public bool? DisableTopSiteCreation;

        public string RecordingJson()
        {
            var us = new UserSettings();
            if (DisableTopSiteCreation == true)
            {
                us.DisableTopSiteCreation = DisableTopSiteCreation;
            }
            return us.ToJson();
        }
    }
}