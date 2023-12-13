using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models.ApiSiteSettings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Models
{
    public static class ApiSiteSettingValidators
    {
        public static ErrorData OnChageSiteSettingByApi(
            string referenceType,
            SiteSettings ss,
            SiteSettingsApiModel siteSettingsModel)
        {
            if (siteSettingsModel == null)
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            ErrorData baseApiValidator;
            if (ApiSiteSetting.ServerScriptRefTypes.Contains(referenceType) && siteSettingsModel.ServerScripts != null)
            {
                var apiServerScripts = siteSettingsModel.ServerScripts;
                var baseApiProperties = apiServerScripts.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    siteSettingType: "ServerScripts");
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            if (siteSettingsModel.Scripts != null)
            {
                var  apiScripts = siteSettingsModel.Scripts;
                var baseApiProperties = apiScripts.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    siteSettingType: "Scripts");
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            if (siteSettingsModel.Htmls != null)
            {
                var apiHtmls = siteSettingsModel.Htmls;
                var baseApiProperties = apiHtmls.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    siteSettingType: "Htmls");
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            if (siteSettingsModel.Styles != null)
            {
                var apiStyles = siteSettingsModel.Styles;
                var baseApiProperties = apiStyles.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    siteSettingType: "Scripts");
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnSiteSettingByApi(
            List<ISiteSettingBaseProperties> apiSiteSettingBaseProperties,
            SiteSettings ss,
            string siteSettingType)
        {
            if (apiSiteSettingBaseProperties == null)
            {
                return new ErrorData(type: Error.Types.None);
            }
            foreach (var apiSiteSetting in apiSiteSettingBaseProperties)
            {
                if (apiSiteSetting.Id == null) return new ErrorData(type: Error.Types.NotFound);
                var scriptModel = ss.Scripts?.FirstOrDefault(o => o.Id == apiSiteSetting.Id.ToInt());
                if (apiSiteSetting.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt()
                     && string.IsNullOrEmpty(apiSiteSetting.Title))
                {
                    return new ErrorData(type: Error.Types.NotFound);
                }
                if (apiSiteSetting.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt()
                    && siteSettingType == "Htmls")
                {
                    if (string.IsNullOrEmpty(apiSiteSetting.HtmlPositionType)
                        || !Enum.IsDefined(typeof(Html.PositionTypes), apiSiteSetting.HtmlPositionType))
                    {
                        return new ErrorData(type: Error.Types.NotFound);
                    }
                }
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
