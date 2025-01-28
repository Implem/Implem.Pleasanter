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
    public static class ResultValidators
    {
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
            if (api && !ss.IsSite(context: context) && !context.CanRead(ss: ss))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (!api && ss.GetNoDisplayIfReadOnly(context: context))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return context.HasPermission(ss: ss)
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
            ResultModel resultModel,
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
            switch (resultModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && resultModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            ResultModel resultModel,
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (!context.CanCreate(ss: ss) || resultModel.ReadOnly)
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
                    mine: resultModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated(
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
                        if (resultModel.Body_Updated(
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
                    case "Status":
                        if (resultModel.Status_Updated(
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
                    case "Manager":
                        if (resultModel.Manager_Updated(
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
                    case "Owner":
                        if (resultModel.Owner_Updated(
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
                    case "Locked":
                        if (resultModel.Locked_Updated(
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
                        if (resultModel.Comments_Updated(context: context))
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
                                if (resultModel.Class_Updated(
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
                                if (resultModel.Num_Updated(
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
                                if (resultModel.Date_Updated(
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
                                if (resultModel.Description_Updated(
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
                                if (resultModel.Check_Updated(
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
                                if (resultModel.Attachments_Updated(
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
            var errorData = OnAttaching(
                context: context,
                ss: ss,
                resultModel: resultModel);
            if (errorData.Type != Error.Types.None)
            {
                return errorData;
            }
            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                resultModel: resultModel).FirstOrDefault();
            if (inputErrorData.Type != Error.Types.None)
            {
                return inputErrorData;
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
            ResultModel resultModel,
            bool api = false,
            bool serverScript = false)
        {
            if (resultModel.RecordPermissions != null && !context.CanManagePermission(ss: ss))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        resultModel.ResultId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (!context.CanUpdate(ss: ss) || resultModel.ReadOnly)
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
                    mine: resultModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (resultModel.Title_Updated(context: context))
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
                        if (resultModel.Body_Updated(context: context))
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
                    case "Status":
                        if (resultModel.Status_Updated(context: context))
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
                    case "Manager":
                        if (resultModel.Manager_Updated(context: context))
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
                    case "Owner":
                        if (resultModel.Owner_Updated(context: context))
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
                    case "Locked":
                        if (resultModel.Locked_Updated(context: context))
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
                        if (resultModel.Comments_Updated(context: context))
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
                                if (resultModel.Class_Updated(
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
                                if (resultModel.Num_Updated(
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
                                if (resultModel.Date_Updated(
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
                                if (resultModel.Description_Updated(
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
                                if (resultModel.Check_Updated(
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
                                if (resultModel.Attachments_Updated(
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
            var errorData = OnAttaching(
                context: context,
                ss: ss,
                resultModel: resultModel);
            if (errorData.Type != Error.Types.None)
            {
                return errorData;
            }
            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                resultModel: resultModel).FirstOrDefault();
            if (inputErrorData.Type != Error.Types.None)
            {
                return inputErrorData;
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        public static ErrorData OnMoving(
            Context context,
            SiteSettings ss,
            SiteSettings destinationSs,
            ResultModel resultModel)
        {
            if (ss.LockedTable())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        resultModel.ResultId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (!Permissions.CanMove(
                context: context,
                source: ss,
                destination: destinationSs)
                    || resultModel.ReadOnly)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        public static ErrorData OnDeleting(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        resultModel.ResultId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return context.CanDelete(ss: ss) && !resultModel.ReadOnly
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
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
            if (ss.LockedTable())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedTable,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
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

        public static ErrorData OnDeleteHistory(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            bool api = false,
            bool serverScript = false)
        {
            if (!Parameters.History.PhysicalDelete
                || ss.AllowPhysicalDeleteHistories == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.InvalidRequest,
                    api: api,
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (!context.CanManageSite(ss: ss) || resultModel.ReadOnly)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
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
            if (ss.LockedRecord())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.LockedRecord,
                    data: new string[]
                    {
                        resultModel.ResultId.ToString(),
                        ss.LockedRecordUser.Name,
                        ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                    },
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        public static ErrorData OnUnlockRecord(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
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
            if (!ss.LockedRecord())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotLockedRecord,
                    api: api,
                    sysLogsStatus: 400,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (!context.CanUpdate(
                ss: ss,
                checkLocked: false)
                    || resultModel.ReadOnly)
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
            if (!context.HasPrivilege && ss.LockedRecordUser.Id != context.UserId)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        private static ErrorData OnAttaching(
            Context context, SiteSettings ss, ResultModel resultModel)
        {
            foreach (var column in ss.Columns.Where(o => o.TypeCs == "Attachments"))
            {
                if (resultModel.Attachments_Updated(
                    columnName: column.Name,
                    context: context,
                    column: column))
                {
                    var invalid = BinaryValidators.OnUploading(
                        context: context,
                        ss: ss,
                        attachmentsHash: resultModel.AttachmentsHash);
                    switch (invalid)
                    {
                        case Error.Types.OverLimitQuantity:
                            return new ErrorData(
                                context: context,
                                type: Error.Types.OverLimitQuantity,
                                data: column.LimitQuantity.ToInt().ToString(),
                                sysLogsStatus: 400,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        case Error.Types.OverLimitSize:
                            return new ErrorData(
                                context: context,
                                type: Error.Types.OverLimitSize,
                                data: column.LimitSize.ToString(),
                                sysLogsStatus: 400,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        case Error.Types.OverTotalLimitSize:
                            return new ErrorData(
                                context: context,
                                type: Error.Types.OverTotalLimitSize,
                                data: column.TotalLimitSize.ToString(),
                                sysLogsStatus: 400,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        case Error.Types.OverLocalFolderLimitSize:
                            return new ErrorData(
                                context: context,
                                type: Error.Types.OverLocalFolderLimitSize,
                                data: column.LocalFolderLimitSize.ToString(),
                                sysLogsStatus: 400,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        case Error.Types.OverLocalFolderTotalLimitSize:
                            return new ErrorData(
                                context: context,
                                type: Error.Types.OverLocalFolderTotalLimitSize,
                                data: column.LocalFolderTotalLimitSize.ToString(),
                                sysLogsStatus: 400,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                        case Error.Types.OverTenantStorageSize:
                            return new ErrorData(
                                context: context,
                                type: Error.Types.OverTenantStorageSize,
                                data: context.ContractSettings.StorageSize.ToString(),
                                sysLogsStatus: 400,
                                sysLogsDescription: Debugs.GetSysLogsDescription());
                    }
                }
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        public static List<ErrorData> OnInputValidating(
            Context context,
            SiteSettings ss,
            Dictionary<int, ResultModel> resultHash,
            bool api = false)
        {
            var errors = resultHash
                ?.OrderBy(data => data.Key)
                .SelectMany((data, index) => OnInputValidating(
                    context: context,
                    ss: ss,
                    resultModel: data.Value,
                    rowNo: index + 1))
                .Where(data => data.Type != Error.Types.None).ToList()
                    ?? new List<ErrorData>();
            if (errors.Count == 0)
            {
                errors.Add(new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    api: api,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
            return errors;
        }

        private static List<ErrorData> OnInputValidating(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            int rowNo = 0)
        {
            var errors = new List<ErrorData>();
            var editorColumns = ss.GetEditorColumns(context: context);
            editorColumns
                ?.Concat(ss
                    .Columns
                    ?.Where(o => !o.NotEditorSettings)
                    .Where(column => !editorColumns
                        .Any(editorColumn => editorColumn.ColumnName == column.ColumnName)))
                .ForEach(column =>
                {
                    var value = resultModel.PropertyValue(
                        context: context,
                        column: column);
                    if (column.TypeCs == "Comments")
                    {
                        var savedCommentId = resultModel
                            .SavedComments
                            ?.Deserialize<Comments>()
                            ?.Max(savedComment => (int?)savedComment.CommentId) ?? default(int);
                        var comment = value
                            ?.Deserialize<Comments>()
                            ?.FirstOrDefault();
                        value = comment?.CommentId > savedCommentId ? comment?.Body : null;
                    }
                    if (!value.IsNullOrEmpty())
                    {
                        Validators.ValidateMaxLength(
                            columnName: column.ColumnName,
                            maxLength: column.MaxLength,
                            errors: errors,
                            value: value);
                        var validationType = ss.Processes
                            ?.FirstOrDefault(o => $"Process_{o.Id}" == context.Forms.ControlId())
                            ?.ValidationType;
                        if (validationType == Process.ValidationTypes.Merge || validationType == null)
                        {
                            Validators.ValidateRegex(
                            columnName: column.ColumnName,
                            serverRegexValidation: column.ServerRegexValidation,
                            regexValidationMessage: column.RegexValidationMessage,
                            errors: errors,
                            value: value);
                        }
                        ss.Processes
                            ?.Where(o => o.ValidationType != Process.ValidationTypes.None)
                            ?.FirstOrDefault(o => $"Process_{o.Id}" == context.Forms.ControlId())
                            ?.ValidateInputs
                            ?.Where(validateInputs => validateInputs.ColumnName == column.ColumnName)
                            ?.ForEach(validateInputs =>
                                Validators.ValidateRegex(
                                    columnName: validateInputs.ColumnName,
                                    serverRegexValidation: validateInputs.ServerRegexValidation,
                                    regexValidationMessage: validateInputs.RegexValidationMessage,
                                    errors: errors,
                                    value: value));
                    }
                });
            if (errors.Count == 0)
            {
                errors.Add(new ErrorData(
                    context: context,
                    type: Error.Types.None,
                    sysLogsStatus: 200,
                    sysLogsDescription: Debugs.GetSysLogsDescription()));
            }
            return errors;
        }
    }
}
