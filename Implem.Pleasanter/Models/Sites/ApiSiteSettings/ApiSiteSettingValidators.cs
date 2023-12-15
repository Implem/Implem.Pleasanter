using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models.ApiSiteSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Implem.Pleasanter.Models
{
    public static class ApiSiteSettingValidators
    {
        public static ErrorData OnChageSiteSettingByApi(
            string referenceType,
            SiteSettings ss,
            SiteSettingsApiModel siteSettingsModel,
            Context context)
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
                    siteSettingType: "Styles");
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            // Validate Processes
            if (siteSettingsModel.Processes != null)
            {
                var apiProcesses = siteSettingsModel.Processes;
                baseApiValidator = ApiSiteSettingValidators.ProcessesValidator(
                    processes: apiProcesses,
                    ss: ss,
                    context: context);
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

        public static ErrorData ProcessesValidator(
                  List<ProcessApiSettingModel> processes,
                  SiteSettings ss,
                  Context context)
        {
            var valid = new ErrorData(type: Error.Types.None);
            processes.ForEach(process =>
            {
                Type objectType = process.GetType();
                PropertyInfo[] properties = objectType.GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(process);
                    switch (property.Name)
                    {
                        case "Id":
                            if (value.ToInt() == 0)
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "Delete":
                            if (value != null && value.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            else if (value.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                            {
                                return;
                            }
                            break;
                        case "ScreenType":
                            if (value != null && !Enum.IsDefined(typeof(Process.ScreenTypes), value))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "CurrentStatus":
                        case "ChangedStatus":
                            if (value != null && !Enum.IsDefined(typeof(Process.Status), value))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "ExecutionType":
                            if (value != null && !Enum.IsDefined(typeof(Process.ExecutionTypes), value))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "ValidationType":
                            if (value != null && !Enum.IsDefined(typeof(Process.ValidationTypes), value))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "ValidateInputs":
                            if (process.ValidateInputs != null)
                            {
                                foreach (var validateInput in process.ValidateInputs)
                                {
                                    if (validateInput.Id.ToInt() == 0
                                        || (validateInput.Delete != null && validateInput.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt()))
                                    {
                                        valid = new ErrorData(type: Error.Types.NotFound);
                                        return;
                                    }
                                }
                            }
                            break;
                        case "DataChanges":
                            if (process.DataChanges != null)
                            {
                                foreach (var dataChange in process.DataChanges)
                                {
                                    if (dataChange.Id.ToInt() == 0
                                        || (dataChange.Delete != null && dataChange.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                        || !Enum.IsDefined(typeof(DataChange.Types), dataChange.Type))
                                    {
                                        valid = new ErrorData(type: Error.Types.NotFound);
                                        return;
                                    }
                                }
                            }
                            break;
                        case "Notifications":
                            if (process.Notifications != null)
                            {
                                foreach (var notification in process.Notifications)
                                {
                                    if (notification.Id.ToInt() == 0
                                        || (notification.Delete != null && notification.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                        || string.IsNullOrEmpty(notification.Subject)
                                        || string.IsNullOrEmpty(notification.Address)
                                        || string.IsNullOrEmpty(notification.Body))
                                    {
                                        valid = new ErrorData(type: Error.Types.NotFound);
                                        return;
                                    }
                                }
                            }
                            break;
                    }
                }
                //if (valid.Type != Error.Types.None) {
                //    return valid;
                //}
            });
            return valid;
        }

        public static ErrorData StatusControlsValidtor(
          List<StatusControl> statusControls,
          SiteSettings ss)
        {
            return new ErrorData(type: Error.Types.None);
        }
    }
}
