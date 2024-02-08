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
                    siteSettingType: "Styles");
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            if (siteSettingsModel.Processes != null)
            {
                baseApiValidator = ApiSiteSettingValidators.ProcessesValidator(
                    processes: siteSettingsModel.Processes,
                    ss: ss,
                    context: context);
                if (baseApiValidator.Type != Error.Types.None) { return baseApiValidator; }
            }
            if (siteSettingsModel.StatusControls != null)
            {
                baseApiValidator = ApiSiteSettingValidators.StatusControlValidator(
                    statusControls: siteSettingsModel.StatusControls,
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
                if (apiSiteSetting.Id == null) {
                    return new ErrorData(type: Error.Types.NotFound);
                }
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
                            else if (process.Delete == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                            {
                                return;
                            }
                            break;
                        case "Name":
                            if (string.IsNullOrEmpty((string)value))
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
                            break;
                        case "ScreenType":
                            if (value != null && !Enum.IsDefined(typeof(Process.ScreenTypes), value))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "ActionType":
                            if (value != null && !Enum.IsDefined(typeof(Process.ActionTypes), value))
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
                                    if (validateInput.Id.ToInt() != 0 && validateInput.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                    {
                                        continue;
                                    }
                                    if (validateInput.Id.ToInt() == 0
                                        || (validateInput.Delete != null && validateInput.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                        || string.IsNullOrEmpty(validateInput.ColumnName))
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
                                    if (dataChange.Id.ToInt() != 0 && dataChange.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                    {
                                        continue;
                                    }
                                    if (dataChange.Id.ToInt() == 0
                                        || (dataChange.Delete != null && dataChange.Delete.ToInt() != ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                        || string.IsNullOrEmpty(dataChange.Type.ToString())
                                        || string.IsNullOrEmpty(dataChange.ColumnName))
                                    {
                                        valid = new ErrorData(type: Error.Types.NotFound);
                                        return;
                                    }
                                    if (!Enum.IsDefined(typeof(DataChange.Types), dataChange.Type) 
                                        || ((dataChange.Type.ToString() == "InputDate" || dataChange.Type.ToString() == "InputDateTime")
                                            && (string.IsNullOrEmpty(dataChange.Value)
                                                || !decimal.TryParse(dataChange.Value?.Split_1st(), out decimal result)
                                                || !Enum.IsDefined(typeof(DataChange.Periods), dataChange.Value?.Split_2nd()))))
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
                                    if (notification.Id.ToInt() != 0 && notification.Delete.ToInt() == ApiSiteSetting.DeleteFlag.IsDelete.ToInt())
                                    {
                                        continue;
                                    }
                                    if (notification.Id.ToInt() == 0
                                        || !Enum.IsDefined(typeof(Notification.Types), notification.Type)
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
                        case "Permission":
                            if (process.Permission?.Users != null
                                && process.Permission.Users.Count() != UserUtilities.CountByIds(
                                    context: context,
                                    ss: ss,
                                    ids: process.Permission.Users))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            if (process.Permission?.Groups != null
                                && process.Permission.Groups.Count() != GroupUtilities.CountByIds(
                                    context: context,
                                    ss: ss,
                                    ids: process.Permission.Groups))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            if (process.Permission?.Depts != null
                               && process.Permission.Depts.Count() != DeptUtilities.CountByIds(
                                   context: context,
                                   ss: ss,
                                   ids: process.Permission.Depts))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                    }
                }
            });
            return valid;
        }

        public static ErrorData StatusControlValidator(
                  List<StatusControlApiSettingModel> statusControls,
                  SiteSettings ss,
                  Context context)
        {
            var valid = new ErrorData(type: Error.Types.None);
            statusControls.ForEach(statusControl =>
            {
                Type objectType = statusControl.GetType();
                PropertyInfo[] properties = objectType.GetProperties();
                foreach (var property in properties)
                {
                    var value = property.GetValue(statusControl);
                    switch (property.Name)
                    {
                        case "Id":
                            if (value.ToInt() == 0)
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                        case "Name":
                            if (statusControl.Delete != ApiSiteSetting.DeleteFlag.IsDelete.ToInt() && string.IsNullOrEmpty((string)value))
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
                        case "Permission":
                            if (statusControl.Permission?.Users != null
                                && statusControl.Permission.Users.Count() != UserUtilities.CountByIds(
                                    context: context,
                                    ss: ss,
                                    ids: statusControl.Permission.Users))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            if (statusControl.Permission?.Groups != null
                                && statusControl.Permission.Groups.Count() != GroupUtilities.CountByIds(
                                    context: context,
                                    ss: ss,
                                    ids: statusControl.Permission.Groups))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            if (statusControl.Permission?.Depts != null
                               && statusControl.Permission.Depts.Count() != DeptUtilities.CountByIds(
                                   context: context,
                                   ss: ss,
                                   ids: statusControl.Permission.Depts))
                            {
                                valid = new ErrorData(type: Error.Types.NotFound);
                                return;
                            }
                            break;
                    }
                }
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
