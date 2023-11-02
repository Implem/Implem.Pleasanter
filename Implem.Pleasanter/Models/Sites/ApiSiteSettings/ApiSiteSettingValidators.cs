using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models.ApiSiteSettings;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Models
{
    /// <summary>
    /// UpdateSiteSettingByApi Validator
    /// </summary>
    public static class ApiSiteSettingValidators
    {
        /// <summary>
        /// UpdateSiteSettingByApi Validator
        /// </summary>
        /// <param name="context">Request Context</param>
        /// <param name="referenceType">Site Reference Type</param>
        /// <param name="ss">SiteSettings</param>
        /// <param name="siteSettingsModel">Api Site Setting Data</param>
        /// <returns></returns>
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
            if (ApiSiteSetting.ServerScriptRefType.Contains(referenceType) && siteSettingsModel.ServerScripts != null)
            {
                List<ServerScriptApiSettingModel> apiServerScripts = siteSettingsModel.ServerScripts;
                List<ISiteSettingBaseProperties> baseApiProperties = apiServerScripts.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    siteSettingType: ApiSiteSetting.SITE_SETTING_TYPE_SERVER_SCRIPT);
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            // Validate Scripts
            if (siteSettingsModel.Scripts != null)
            {
                List<ScriptApiSettingModel> apiScripts = siteSettingsModel.Scripts;
                List<ISiteSettingBaseProperties> baseApiProperties = apiScripts.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    ApiSiteSetting.SITE_SETTING_TYPE_SCRIPT);
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            // Validate Htmls
            if (siteSettingsModel.Htmls != null)
            {
                List<HtmlApiSettingModel> apiHtmls = siteSettingsModel.Htmls;
                List<ISiteSettingBaseProperties> baseApiProperties = apiHtmls.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    ApiSiteSetting.SITE_SETTING_TYPE_HTML);
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            // Validate Styles
            if (siteSettingsModel.Styles != null)
            {
                List<StyleApiSettingModel> apiStyles = siteSettingsModel.Styles;
                List<ISiteSettingBaseProperties> baseApiProperties = apiStyles.Cast<ISiteSettingBaseProperties>().ToList();
                baseApiValidator = ApiSiteSettingValidators.OnSiteSettingByApi(
                    apiSiteSettingBaseProperties: baseApiProperties,
                    ss: ss,
                    ApiSiteSetting.SITE_SETTING_TYPE_SCRIPT);
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Validate common attributes of ServerScript, Script, Style, Htmls
        /// </summary>
        /// <param name="apiSiteSettingBaseProperties">Contains validate properties</param>
        /// <param name="ss" ></param>
        /// <returns> <c>ErrorData </c> object. Incase ErrorData.Type equal Error.Types.None meaning passed
        /// </returns>
        public static ErrorData OnSiteSettingByApi(
            List<ISiteSettingBaseProperties> apiSiteSettingBaseProperties,
            SiteSettings ss,
            string siteSettingType)
        {
            if (apiSiteSettingBaseProperties == null)
            {
                return new ErrorData(type: Error.Types.None);
            }
            foreach (ISiteSettingBaseProperties apiSiteSetting in apiSiteSettingBaseProperties)
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
                    && siteSettingType == ApiSiteSetting.SITE_SETTING_TYPE_HTML)
                {
                    // HtmlPositionType is required and valid value
                    if (string.IsNullOrEmpty(apiSiteSetting.HtmlPositionType)
                        || !ApiSiteSetting.HtmlPositionTypes.Contains(apiSiteSetting.HtmlPositionType))
                    {
                        return new ErrorData(type: Error.Types.NotFound);
                    }
                }
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
