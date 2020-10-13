using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class UserSettings
    {
        public bool? EnableManageTenant;
        public bool? DisableTopSiteCreation;
        public bool? DisableGroupAdmin;
        public bool? DisableStartGuide;

        public string RecordingJson()
        {
            var us = new UserSettings();
            if (EnableManageTenant == true)
            {
                us.EnableManageTenant = EnableManageTenant;
            }
            if (DisableTopSiteCreation == true)
            {
                us.DisableTopSiteCreation = DisableTopSiteCreation;
            }
            if (DisableGroupAdmin == true)
            {
                us.DisableGroupAdmin = DisableGroupAdmin;
            }
            if (DisableStartGuide == true)
            {
                us.DisableStartGuide = DisableStartGuide;
            }
            return us.ToJson();
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return null;
        }

        public bool InitialValue(Context context)
        {
            return RecordingJson() == "[]";
        }

        public bool StartGuide(Context context)
        {
            return Parameters.Service.ShowStartGuide
                && context.DisableStartGuide != true
                && DisableStartGuide != true
                && DisableTopSiteCreation != true;
        }

        public bool ShowStartGuideAvailable(Context context)
        {
            return Parameters.Service.ShowStartGuide
                && context.DisableStartGuide != true
                && DisableStartGuide == true
                && DisableTopSiteCreation != true;
        }
    }
}