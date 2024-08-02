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
        public bool? DisableGroupCreation;
        public bool? DisableMovingFromTopSite;
        public bool? DisableApi;
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
            if (DisableMovingFromTopSite == true)
            {
                us.DisableMovingFromTopSite = DisableMovingFromTopSite;
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

        public bool AllowCreationAtTopSite(Context context)
        {
            if (context.HasPrivilege) return true;
            return (!Parameters.User.DisableTopSiteCreation
                || context.User.AllowCreationAtTopSite)
                    && DisableTopSiteCreation != true;
        }

        public bool AllowGroupAdministration(Context context)
        {
            if (context.HasPrivilege) return true;
            return (!Parameters.User.DisableGroupAdmin
                || context.User.AllowGroupAdministration)
                    && DisableGroupAdmin != true;
        }

        public bool AllowGroupCreation(Context context)
        {
            if (context.HasPrivilege) return true;
            return (!Parameters.User.DisableGroupCreation
                || context.User.AllowGroupCreation)
                    && DisableGroupCreation != true;
        }

        public bool AllowMovingFromTopSite(Context context)
        {
            if (context.HasPrivilege) return true;
            return (!Parameters.User.DisableMovingFromTopSite
                || context.User.AllowMovingFromTopSite)
                    && DisableMovingFromTopSite != true;
        }

        public bool AllowApi(Context context)
        {
            if (context.HasPrivilege) return true;
            return (!(Parameters.User.DisableApi || context.DisableApi)
                || context.User.AllowApi)
                    && DisableApi != true;
        }
    }
}