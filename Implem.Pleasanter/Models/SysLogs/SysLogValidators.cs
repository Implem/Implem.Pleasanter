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
                ? new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    api: api,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription())
                : new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
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
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            switch (sysLogModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && sysLogModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? new ErrorData(
                                context: context,
                                type: Error.Types.None,
                                api: api,
                                sysLogsStatus: 200,
                                sysLogsDescription: Debugs.GetSysLogsDescription())
                            : new ErrorData(
                                context: context,
                                type: Error.Types.NotFound,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? new ErrorData(
                            context: context,
                            type: Error.Types.None,
                            api: api,
                            sysLogsStatus: 200,
                            sysLogsDescription: Debugs.GetSysLogsDescription())
                        : !context.CanRead(ss: ss)
                            ? new ErrorData(
                                context: context,
                                type: Error.Types.NotFound,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription())
                            : new ErrorData(
                                context: context,
                                type: Error.Types.HasNotPermission,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                default:
                    return new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
            }
        }

        public static ErrorData OnCreating(
            Context context,
            SiteSettings ss,
            SysLogModel sysLogModel,
            bool copy = false,
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
                    ? new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription())
                    : new ErrorData(
                        context: context,
                        type: Error.Types.HasNotPermission,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
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
                        if (sysLogModel.SysLogType_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "OnAzure":
                        if (sysLogModel.OnAzure_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "MachineName":
                        if (sysLogModel.MachineName_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ServiceName":
                        if (sysLogModel.ServiceName_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "TenantName":
                        if (sysLogModel.TenantName_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Application":
                        if (sysLogModel.Application_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Class":
                        if (sysLogModel.Class_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Method":
                        if (sysLogModel.Method_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Api":
                        if (sysLogModel.Api_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "RequestData":
                        if (sysLogModel.RequestData_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "HttpMethod":
                        if (sysLogModel.HttpMethod_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "RequestSize":
                        if (sysLogModel.RequestSize_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ResponseSize":
                        if (sysLogModel.ResponseSize_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Elapsed":
                        if (sysLogModel.Elapsed_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ApplicationAge":
                        if (sysLogModel.ApplicationAge_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ApplicationRequestInterval":
                        if (sysLogModel.ApplicationRequestInterval_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "SessionAge":
                        if (sysLogModel.SessionAge_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "SessionRequestInterval":
                        if (sysLogModel.SessionRequestInterval_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "WorkingSet64":
                        if (sysLogModel.WorkingSet64_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "VirtualMemorySize64":
                        if (sysLogModel.VirtualMemorySize64_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ProcessId":
                        if (sysLogModel.ProcessId_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ProcessName":
                        if (sysLogModel.ProcessName_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "BasePriority":
                        if (sysLogModel.BasePriority_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Url":
                        if (sysLogModel.Url_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UrlReferer":
                        if (sysLogModel.UrlReferer_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserHostName":
                        if (sysLogModel.UserHostName_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserHostAddress":
                        if (sysLogModel.UserHostAddress_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserLanguage":
                        if (sysLogModel.UserLanguage_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserAgent":
                        if (sysLogModel.UserAgent_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "SessionGuid":
                        if (sysLogModel.SessionGuid_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ErrMessage":
                        if (sysLogModel.ErrMessage_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ErrStackTrace":
                        if (sysLogModel.ErrStackTrace_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "InDebug":
                        if (sysLogModel.InDebug_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "AssemblyVersion":
                        if (sysLogModel.AssemblyVersion_Updated(
                            context: context,
                            column: column,
                            copy: copy))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Comments":
                        if (sysLogModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (sysLogModel.Class_Updated(
                                    columnName: column.Name,
                                    copy: copy,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Num":
                                if (sysLogModel.Num_Updated(
                                    columnName: column.Name,
                                    copy: copy,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Date":
                                if (sysLogModel.Date_Updated(
                                    columnName: column.Name,
                                    copy: copy,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Description":
                                if (sysLogModel.Description_Updated(
                                    columnName: column.Name,
                                    copy: copy,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Check":
                                if (sysLogModel.Check_Updated(
                                    columnName: column.Name,
                                    copy: copy,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Attachments":
                                if (sysLogModel.Attachments_Updated(
                                    columnName: column.Name,
                                    copy: copy,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
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
                    ? new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription())
                    : new ErrorData(
                        context: context,
                        type: Error.Types.HasNotPermission,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
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
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "OnAzure":
                        if (sysLogModel.OnAzure_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "MachineName":
                        if (sysLogModel.MachineName_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ServiceName":
                        if (sysLogModel.ServiceName_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "TenantName":
                        if (sysLogModel.TenantName_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Application":
                        if (sysLogModel.Application_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Class":
                        if (sysLogModel.Class_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Method":
                        if (sysLogModel.Method_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Api":
                        if (sysLogModel.Api_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "RequestData":
                        if (sysLogModel.RequestData_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "HttpMethod":
                        if (sysLogModel.HttpMethod_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "RequestSize":
                        if (sysLogModel.RequestSize_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ResponseSize":
                        if (sysLogModel.ResponseSize_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Elapsed":
                        if (sysLogModel.Elapsed_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ApplicationAge":
                        if (sysLogModel.ApplicationAge_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ApplicationRequestInterval":
                        if (sysLogModel.ApplicationRequestInterval_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "SessionAge":
                        if (sysLogModel.SessionAge_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "SessionRequestInterval":
                        if (sysLogModel.SessionRequestInterval_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "WorkingSet64":
                        if (sysLogModel.WorkingSet64_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "VirtualMemorySize64":
                        if (sysLogModel.VirtualMemorySize64_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ProcessId":
                        if (sysLogModel.ProcessId_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ProcessName":
                        if (sysLogModel.ProcessName_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "BasePriority":
                        if (sysLogModel.BasePriority_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Url":
                        if (sysLogModel.Url_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UrlReferer":
                        if (sysLogModel.UrlReferer_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserHostName":
                        if (sysLogModel.UserHostName_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserHostAddress":
                        if (sysLogModel.UserHostAddress_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserLanguage":
                        if (sysLogModel.UserLanguage_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "UserAgent":
                        if (sysLogModel.UserAgent_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "SessionGuid":
                        if (sysLogModel.SessionGuid_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ErrMessage":
                        if (sysLogModel.ErrMessage_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "ErrStackTrace":
                        if (sysLogModel.ErrStackTrace_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "InDebug":
                        if (sysLogModel.InDebug_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "AssemblyVersion":
                        if (sysLogModel.AssemblyVersion_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    case "Comments":
                        if (sysLogModel.Comments_Updated(context: context))
                        {
                            return new ErrorData(
                                context: context,
                                type: Error.Types.HasNotChangeColumnPermission,
                                data: column.LabelText,
                                api: api,
                                sysLogsStatus: 403,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                        {
                            case "Class":
                                if (sysLogModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Num":
                                if (sysLogModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Date":
                                if (sysLogModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Description":
                                if (sysLogModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Check":
                                if (sysLogModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                            case "Attachments":
                                if (sysLogModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context))
                                {
                                    return new ErrorData(
                                        context: context,
                                        type: Error.Types.HasNotChangeColumnPermission,
                                        data: column.LabelText,
                                        api: api,
                                        sysLogsStatus: 403,
                                        sysLogsDescription: Debugs.GetSysLogsDescription());
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
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
                ? new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    api: api,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription())
                : !context.CanRead(ss: ss)
                    ? new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription())
                    : new ErrorData(
                        context: context,
                        type: Error.Types.HasNotPermission,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
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
                ? new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    api: api,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription())
                : new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
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
                ? new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    api: api,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription())
                : !context.CanRead(ss: ss)
                    ? new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription())
                    : new ErrorData(
                        context: context,
                        type: Error.Types.HasNotPermission,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
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
                ? new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    api: api,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription())
                : !context.CanRead(ss: ss)
                    ? new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription())
                    : new ErrorData(
                        context: context,
                        type: Error.Types.HasNotPermission,
                        api: api,
                        sysLogsStatus: 403,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
        }
    }
}
