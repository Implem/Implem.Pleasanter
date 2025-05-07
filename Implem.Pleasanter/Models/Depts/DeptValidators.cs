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
    public static class DeptValidators
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
                return new ErrorData(type: Error.Types.None);
            }
            return Permissions.CanManageTenant(context: context)
                || context.UserSettings?.EnableManageTenant == true
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
            DeptModel deptModel,
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
            switch (deptModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && deptModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            DeptModel deptModel,
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
            if (!context.CanCreate(ss: ss) || deptModel.ReadOnly)
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
                    mine: deptModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "DeptCode":
                        if (deptModel.DeptCode_Updated(
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
                    case "DeptName":
                        if (deptModel.DeptName_Updated(
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
                    case "Body":
                        if (deptModel.Body_Updated(
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
                    case "Disabled":
                        if (deptModel.Disabled_Updated(
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
                        if (deptModel.Comments_Updated(context: context))
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
                                if (deptModel.Class_Updated(
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
                                if (deptModel.Num_Updated(
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
                                if (deptModel.Date_Updated(
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
                                if (deptModel.Description_Updated(
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
                                if (deptModel.Check_Updated(
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
                                if (deptModel.Attachments_Updated(
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
            DeptModel deptModel,
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
            if (!context.CanUpdate(ss: ss) || deptModel.ReadOnly)
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
                    mine: deptModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "DeptCode":
                        if (deptModel.DeptCode_Updated(context: context))
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
                    case "DeptName":
                        if (deptModel.DeptName_Updated(context: context))
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
                    case "Body":
                        if (deptModel.Body_Updated(context: context))
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
                    case "Disabled":
                        if (deptModel.Disabled_Updated(context: context))
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
                        if (deptModel.Comments_Updated(context: context))
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
                                if (deptModel.Class_Updated(
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
                                if (deptModel.Num_Updated(
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
                                if (deptModel.Date_Updated(
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
                                if (deptModel.Description_Updated(
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
                                if (deptModel.Check_Updated(
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
                                if (deptModel.Attachments_Updated(
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
            DeptModel deptModel,
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
            return context.CanDelete(ss: ss) && !deptModel.ReadOnly
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
