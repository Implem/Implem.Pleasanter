using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class IssueValidators
    {
        public static ErrorData OnEntry(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.HasPermission(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnReading(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanRead(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnEditing(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            switch (issueModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss) &&
                        issueModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? new ErrorData(type: Error.Types.None)
                            : new ErrorData(type: Error.Types.NotFound);
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? new ErrorData(type: Error.Types.None)
                        : new ErrorData(type: Error.Types.HasNotPermission);
                default:
                    return new ErrorData(type: Error.Types.NotFound);
            }
        }

        public static ErrorData OnCreating(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!context.CanCreate(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            ss.SetColumnAccessControls(context: context, mine: issueModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                if (issueModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (issueModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (issueModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (issueModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (issueModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (issueModel.Attachments_Updated(
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
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!context.CanUpdate(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            ss.SetColumnAccessControls(context: context, mine: issueModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "Title":
                        if (issueModel.Title_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (issueModel.Body_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "StartTime":
                        if (issueModel.StartTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "CompletionTime":
                        if (issueModel.CompletionTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "WorkValue":
                        if (issueModel.WorkValue_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ProgressRate":
                        if (issueModel.ProgressRate_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Status":
                        if (issueModel.Status_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Manager":
                        if (issueModel.Manager_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Owner":
                        if (issueModel.Owner_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                if (issueModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (issueModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (issueModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (issueModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (issueModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (issueModel.Attachments_Updated(
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

        public static ErrorData OnMoving(
            Context context, SiteSettings source, SiteSettings destination)
        {
            if (!Permissions.CanMove(context: context, source: source, destination: destination))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(
            Context context, SiteSettings ss, IssueModel issueModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanDelete(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnRestoring(Context context, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnExporting(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanExport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }
    }
}
