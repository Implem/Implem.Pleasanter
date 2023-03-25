using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnEntry(
            Context context,
            SiteSettings ss,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                if (!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false
                    || serverScript)
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
                return new ErrorData(type: Error.Types.None);
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnGet(
            Context context,
            SiteSettings ss,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            return context.CanRead(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.NotFound);
        }

        public static ErrorData OnEditing(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (ss.GetNoDisplayIfReadOnly())
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            switch (userModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && userModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? new ErrorData(type: Error.Types.None)
                            : new ErrorData(type: Error.Types.NotFound);
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? new ErrorData(type: Error.Types.None)
                        : !context.CanRead(ss: ss)
                            ? new ErrorData(type: Error.Types.NotFound)
                            : new ErrorData(type: Error.Types.HasNotPermission);
                default:
                    return new ErrorData(type: Error.Types.NotFound);
            }
        }

        public static ErrorData OnCreating(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!context.CanCreate(ss: ss) || userModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate(
                    context: context,
                    ss: ss,
                    mine: userModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Theme":
                        if (userModel.Theme_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowCreationAtTopSite":
                        if (userModel.AllowCreationAtTopSite_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowGroupAdministration":
                        if (userModel.AllowGroupAdministration_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowGroupCreation":
                        if (userModel.AllowGroupCreation_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowApi":
                        if (userModel.AllowApi_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "EnableSecondaryAuthentication":
                        if (userModel.EnableSecondaryAuthentication_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DisableSecondaryAuthentication":
                        if (userModel.DisableSecondaryAuthentication_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Lockout":
                        if (userModel.Lockout_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LockoutCounter":
                        if (userModel.LockoutCounter_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SecondaryAuthenticationCode":
                        if (userModel.SecondaryAuthenticationCode_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LdapSearchRoot":
                        if (userModel.LdapSearchRoot_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SecondaryAuthenticationCodeExpirationTime":
                        if (userModel.SecondaryAuthenticationCodeExpirationTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SynchronizedTime":
                        if (userModel.SynchronizedTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (userModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (userModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (userModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (userModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (userModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (userModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (userModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (context.Forms.Exists("Users_TenantManager")
                && userModel.Self(context: context))
            {
                return new ErrorData(type: Error.Types.PermissionNotSelfChange);
            }
            if (!context.CanUpdate(ss: ss) || userModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate(
                    context: context,
                    ss: ss,
                    mine: userModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Theme":
                        if (userModel.Theme_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowCreationAtTopSite":
                        if (userModel.AllowCreationAtTopSite_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowGroupAdministration":
                        if (userModel.AllowGroupAdministration_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowGroupCreation":
                        if (userModel.AllowGroupCreation_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AllowApi":
                        if (userModel.AllowApi_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "EnableSecondaryAuthentication":
                        if (userModel.EnableSecondaryAuthentication_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DisableSecondaryAuthentication":
                        if (userModel.DisableSecondaryAuthentication_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Lockout":
                        if (userModel.Lockout_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LockoutCounter":
                        if (userModel.LockoutCounter_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SecondaryAuthenticationCode":
                        if (userModel.SecondaryAuthenticationCode_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SecondaryAuthenticationCodeExpirationTime":
                        if (userModel.SecondaryAuthenticationCodeExpirationTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LdapSearchRoot":
                        if (userModel.LdapSearchRoot_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SynchronizedTime":
                        if (userModel.SynchronizedTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (userModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (userModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (userModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (userModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (userModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (userModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (userModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(
            Context context,
            SiteSettings ss,
            UserModel userModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanDelete(ss: ss) && !userModel.ReadOnly
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnRestoring(
            Context context,
            SiteSettings ss,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnImporting(
            Context context,
            SiteSettings ss,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanImport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnExporting(
            Context context,
            SiteSettings ss,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanExport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordChange(Context context)
        {
            if (!Parameters.Service.ShowChangePassword)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordChanging(Context context, UserModel userModel)
        {
            if (!Parameters.Service.ShowProfiles
                && !Parameters.Service.ShowChangePassword
                && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.UserId == context.UserId)
            {
                if (userModel.OldPassword == userModel.ChangedPassword)
                {
                    return new ErrorData(type: Error.Types.PasswordNotChanged);
                }
                if (!userModel.GetByCredentials(
                    context: context,
                    loginId: userModel.LoginId,
                    password: userModel.OldPassword,
                    updateLockout: true))
                {
                    return new ErrorData(type: Error.Types.IncorrectCurrentPassword);
                }
            }
            else
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordChangingAtLogin(
            Context context, UserModel userModel)
        {
            if (!Parameters.Service.ShowProfiles && !Parameters.Service.ShowChangePassword)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.Password == userModel.ChangedPassword)
            {
                return new ErrorData(type: Error.Types.PasswordNotChanged);
            }
            else if (!userModel.GetByCredentials(
                context: context,
                loginId: userModel.LoginId,
                password: userModel.Password,
                tenantId: context.Forms.Int("SelectedTenantId"),
                updateLockout: true))
            {
                return new ErrorData(type: Error.Types.IncorrectCurrentPassword);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordResetting(Context context)
        {
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Permissions.CanManageTenant(context: context))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnAddingMailAddress(
            Context context, UserModel userModel, string mailAddress)
        {
            var errorData = MailAddressValidators.BadMailAddress(
                addresses: mailAddress,
                only: true);
            if (!Parameters.Service.ShowProfiles && !context.HasPrivilege)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (errorData.Type.Has())
            {
                return errorData;
            }
            if (mailAddress.Trim() == string.Empty)
            {
                return new ErrorData(type: Error.Types.InputMailAddress);
            }
            if (!Permissions.CanManageTenant(context: context)
                && !userModel.Self(context: context))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiEditing(Context context, UserModel userModel)
        {
            if ((!Parameters.Api.Enabled
                || context.ContractSettings.Api == false
                || context.UserSettings?.AllowApi(context: context) == false))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiCreating(Context context, UserModel userModel)
        {
            if ((!Parameters.Api.Enabled
                || context.ContractSettings.Api == false
                || context.UserSettings?.AllowApi(context: context) == false))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiUpdatingMailAddress(UserApiModel userApiModel)
        {
            foreach (string mailAddress in userApiModel.MailAddresses)
            {
                var errorData = MailAddressValidators.BadMailAddress(
                    addresses: mailAddress,
                    only: true);
                if (errorData.Type.Has())
                {
                    return errorData;
                }
                if (mailAddress.Trim() == string.Empty)
                {
                    return new ErrorData(
                        type: Error.Types.BadMailAddress,
                        data: new string[] { mailAddress });
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiDeleting(Context context, UserModel userModel)
        {
            if ((!Parameters.Api.Enabled
                || context.ContractSettings.Api == false
                || context.UserSettings?.AllowApi(context: context) == false))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnSwitchUser(Context context, UserModel userModel)
        {
            if (!Permissions.PrivilegedUsers(context.LoginId))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.Disabled)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnReturnSwitchUser(Context context)
        {
            if (!Permissions.PrivilegedUsers(context.LoginId))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
