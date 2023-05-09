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
    public static class SysLogValidators
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
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            if (!api && ss.GetNoDisplayIfReadOnly(context: context))
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            // シスログ一覧の閲覧は特権ユーザにのみ許可
            return context.HasPrivilege
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
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
            SysLogModel sysLogModel,
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
            if (ss.GetNoDisplayIfReadOnly(context: context))
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            switch (sysLogModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && sysLogModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            SysLogModel sysLogModel,
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
            if (!context.CanCreate(ss: ss) || sysLogModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate(
                    context: context,
                    ss: ss,
                    mine: sysLogModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "SysLogType":
                        if (sysLogModel.SysLogType_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "OnAzure":
                        if (sysLogModel.OnAzure_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "MachineName":
                        if (sysLogModel.MachineName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ServiceName":
                        if (sysLogModel.ServiceName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TenantName":
                        if (sysLogModel.TenantName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Application":
                        if (sysLogModel.Application_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Class":
                        if (sysLogModel.Class_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Method":
                        if (sysLogModel.Method_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "RequestData":
                        if (sysLogModel.RequestData_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "HttpMethod":
                        if (sysLogModel.HttpMethod_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "RequestSize":
                        if (sysLogModel.RequestSize_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ResponseSize":
                        if (sysLogModel.ResponseSize_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Elapsed":
                        if (sysLogModel.Elapsed_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApplicationAge":
                        if (sysLogModel.ApplicationAge_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApplicationRequestInterval":
                        if (sysLogModel.ApplicationRequestInterval_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SessionAge":
                        if (sysLogModel.SessionAge_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SessionRequestInterval":
                        if (sysLogModel.SessionRequestInterval_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "WorkingSet64":
                        if (sysLogModel.WorkingSet64_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "VirtualMemorySize64":
                        if (sysLogModel.VirtualMemorySize64_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProcessId":
                        if (sysLogModel.ProcessId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProcessName":
                        if (sysLogModel.ProcessName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "BasePriority":
                        if (sysLogModel.BasePriority_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Url":
                        if (sysLogModel.Url_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UrlReferer":
                        if (sysLogModel.UrlReferer_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserHostName":
                        if (sysLogModel.UserHostName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserHostAddress":
                        if (sysLogModel.UserHostAddress_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserLanguage":
                        if (sysLogModel.UserLanguage_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserAgent":
                        if (sysLogModel.UserAgent_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SessionGuid":
                        if (sysLogModel.SessionGuid_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ErrMessage":
                        if (sysLogModel.ErrMessage_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ErrStackTrace":
                        if (sysLogModel.ErrStackTrace_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "InDebug":
                        if (sysLogModel.InDebug_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AssemblyVersion":
                        if (sysLogModel.AssemblyVersion_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (sysLogModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (sysLogModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (sysLogModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (sysLogModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (sysLogModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (sysLogModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (sysLogModel.Attachments_Updated(
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
            SysLogModel sysLogModel,
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
            if (!context.CanUpdate(ss: ss) || sysLogModel.ReadOnly)
            {
                return !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate(
                    context: context,
                    ss: ss,
                    mine: sysLogModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "SysLogType":
                        if (sysLogModel.SysLogType_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "OnAzure":
                        if (sysLogModel.OnAzure_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "MachineName":
                        if (sysLogModel.MachineName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ServiceName":
                        if (sysLogModel.ServiceName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TenantName":
                        if (sysLogModel.TenantName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Application":
                        if (sysLogModel.Application_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Class":
                        if (sysLogModel.Class_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Method":
                        if (sysLogModel.Method_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "RequestData":
                        if (sysLogModel.RequestData_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "HttpMethod":
                        if (sysLogModel.HttpMethod_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "RequestSize":
                        if (sysLogModel.RequestSize_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ResponseSize":
                        if (sysLogModel.ResponseSize_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Elapsed":
                        if (sysLogModel.Elapsed_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApplicationAge":
                        if (sysLogModel.ApplicationAge_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApplicationRequestInterval":
                        if (sysLogModel.ApplicationRequestInterval_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SessionAge":
                        if (sysLogModel.SessionAge_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SessionRequestInterval":
                        if (sysLogModel.SessionRequestInterval_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "WorkingSet64":
                        if (sysLogModel.WorkingSet64_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "VirtualMemorySize64":
                        if (sysLogModel.VirtualMemorySize64_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProcessId":
                        if (sysLogModel.ProcessId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProcessName":
                        if (sysLogModel.ProcessName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "BasePriority":
                        if (sysLogModel.BasePriority_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Url":
                        if (sysLogModel.Url_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UrlReferer":
                        if (sysLogModel.UrlReferer_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserHostName":
                        if (sysLogModel.UserHostName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserHostAddress":
                        if (sysLogModel.UserHostAddress_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserLanguage":
                        if (sysLogModel.UserLanguage_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserAgent":
                        if (sysLogModel.UserAgent_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SessionGuid":
                        if (sysLogModel.SessionGuid_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ErrMessage":
                        if (sysLogModel.ErrMessage_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ErrStackTrace":
                        if (sysLogModel.ErrStackTrace_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "InDebug":
                        if (sysLogModel.InDebug_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AssemblyVersion":
                        if (sysLogModel.AssemblyVersion_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (sysLogModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (sysLogModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (sysLogModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (sysLogModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (sysLogModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (sysLogModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (sysLogModel.Attachments_Updated(
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
            SysLogModel sysLogModel,
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
            return context.CanDelete(ss: ss) && !sysLogModel.ReadOnly
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
            return context.CanExport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : !context.CanRead(ss: ss)
                    ? new ErrorData(type: Error.Types.NotFound)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }
    }
}
