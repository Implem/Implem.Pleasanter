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
            // Check ReferenceType and validate ServerScripts
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
            // Validate Scripts
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
            // Validate Htmls
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
            // Validate Styles
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
                // Validate always required Id
                if (apiSiteSetting.Id == null) return new ErrorData(type: Error.Types.NotFound);
                var scriptModel = ss.Scripts?.FirstOrDefault(o => o.Id == apiSiteSetting.Id.ToInt());
                // Validate Title is required in case update, create
                if (apiSiteSetting.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt()
                     && string.IsNullOrEmpty(apiSiteSetting.Title))
                {
                    return new ErrorData(type: Error.Types.NotFound);
                }
                // Validate HtmlPositionType value in case change Htmls setting
                if (apiSiteSetting.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt()
                    && siteSettingType == "Htmls")
                {
                    // HtmlPositionType is required and valid value
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
