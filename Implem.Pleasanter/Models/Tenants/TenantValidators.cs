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
    public static class TenantValidators
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
            TenantModel tenantModel,
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
            switch (tenantModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss)
                        && tenantModel.AccessStatus != Databases.AccessStatuses.NotFound
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
            TenantModel tenantModel,
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
            if (!context.CanCreate(ss: ss) || tenantModel.ReadOnly)
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
                    mine: tenantModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "TenantName":
                        if (tenantModel.TenantName_Updated(
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
                    case "Title":
                        if (tenantModel.Title_Updated(
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
                        if (tenantModel.Body_Updated(
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
                    case "DisableAllUsersPermission":
                        if (tenantModel.DisableAllUsersPermission_Updated(
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
                    case "DisableApi":
                        if (tenantModel.DisableApi_Updated(
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
                    case "DisableStartGuide":
                        if (tenantModel.DisableStartGuide_Updated(
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
                    case "LogoType":
                        if (tenantModel.LogoType_Updated(
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
                    case "HtmlTitleTop":
                        if (tenantModel.HtmlTitleTop_Updated(
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
                    case "HtmlTitleSite":
                        if (tenantModel.HtmlTitleSite_Updated(
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
                    case "HtmlTitleRecord":
                        if (tenantModel.HtmlTitleRecord_Updated(
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
                    case "TopStyle":
                        if (tenantModel.TopStyle_Updated(
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
                    case "TopScript":
                        if (tenantModel.TopScript_Updated(
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
                    case "TopDashboards":
                        if (tenantModel.TopDashboards_Updated(
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
                    case "Theme":
                        if (tenantModel.Theme_Updated(
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
                    case "ContractDeadline":
                        if (tenantModel.ContractDeadline_Updated(
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
                        if (tenantModel.Comments_Updated(context: context))
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
                                if (tenantModel.Class_Updated(
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
                                if (tenantModel.Num_Updated(
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
                                if (tenantModel.Date_Updated(
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
                                if (tenantModel.Description_Updated(
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
                                if (tenantModel.Check_Updated(
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
                                if (tenantModel.Attachments_Updated(
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
            TenantModel tenantModel,
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
            if (!context.CanUpdate(ss: ss) || tenantModel.ReadOnly)
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
                    mine: tenantModel.Mine(context: context)))
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "TenantName":
                        if (tenantModel.TenantName_Updated(context: context))
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
                    case "Title":
                        if (tenantModel.Title_Updated(context: context))
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
                        if (tenantModel.Body_Updated(context: context))
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
                    case "ContractDeadline":
                        if (tenantModel.ContractDeadline_Updated(context: context))
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
                    case "DisableAllUsersPermission":
                        if (tenantModel.DisableAllUsersPermission_Updated(context: context))
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
                    case "DisableApi":
                        if (tenantModel.DisableApi_Updated(context: context))
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
                    case "DisableStartGuide":
                        if (tenantModel.DisableStartGuide_Updated(context: context))
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
                    case "LogoType":
                        if (tenantModel.LogoType_Updated(context: context))
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
                    case "HtmlTitleTop":
                        if (tenantModel.HtmlTitleTop_Updated(context: context))
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
                    case "HtmlTitleSite":
                        if (tenantModel.HtmlTitleSite_Updated(context: context))
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
                    case "HtmlTitleRecord":
                        if (tenantModel.HtmlTitleRecord_Updated(context: context))
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
                    case "TopStyle":
                        if (tenantModel.TopStyle_Updated(context: context))
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
                    case "TopScript":
                        if (tenantModel.TopScript_Updated(context: context))
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
                    case "TopDashboards":
                        if (tenantModel.TopDashboards_Updated(context: context))
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
                    case "Theme":
                        if (tenantModel.Theme_Updated(context: context))
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
                        if (tenantModel.Comments_Updated(context: context))
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
                                if (tenantModel.Class_Updated(
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
                                if (tenantModel.Num_Updated(
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
                                if (tenantModel.Date_Updated(
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
                                if (tenantModel.Description_Updated(
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
                                if (tenantModel.Check_Updated(
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
                                if (tenantModel.Attachments_Updated(
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
            TenantModel tenantModel,
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
            return context.CanDelete(ss: ss) && !tenantModel.ReadOnly
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnSyncByLdap(
            Context context, SiteSettings ss)
        {
            if (!context.CanUpdate(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
