using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Microsoft.AspNetCore.Http;

namespace Implem.Pleasanter.Models
{
    public enum BackgroundJobAccessScope
    {
        All,
        Tenant,
        Own
    }

    public static class BackgroundJobAccessValidator
    {
        public static BackgroundJobAccessScope GetAccessScope(Context context)
        {
            if (context.HasPrivilege)
            {
                return BackgroundJobAccessScope.All;
            }
            if (context.User?.TenantManager == true)
            {
                return BackgroundJobAccessScope.Tenant;
            }
            return BackgroundJobAccessScope.Own;
        }

        public static bool CanAccess(
            Context context,
            BackgroundJobModel model)
        {
            switch (GetAccessScope(context: context))
            {
                case BackgroundJobAccessScope.All:
                    return true;
                case BackgroundJobAccessScope.Tenant:
                    return model.TenantId == context.TenantId;
                default:
                    return model.TenantId == context.TenantId
                        && model.UserId == context.UserId;
            }
        }

        public static ErrorData OnCancelling(
            Context context,
            BackgroundJobModel model)
        {
            if (CanAccess(
                context: context,
                model: model) == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    sysLogsStatus: StatusCodes.Status403Forbidden,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (model.Status != BackgroundJobStatus.Pending)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.CanNotPerformed,
                    sysLogsStatus: StatusCodes.Status400BadRequest,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                sysLogsStatus: StatusCodes.Status200OK,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        public static ErrorData OnDeleting(
            Context context,
            BackgroundJobModel model)
        {
            if (CanAccess(
                context: context,
                model: model) == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    sysLogsStatus: StatusCodes.Status403Forbidden,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            switch (model.Status)
            {
                case BackgroundJobStatus.Pending:
                case BackgroundJobStatus.Completed:
                case BackgroundJobStatus.Failed:
                case BackgroundJobStatus.Cancelled:
                    return new ErrorData(
                        context: context,
                        type: Error.Types.None,
                        sysLogsStatus: StatusCodes.Status200OK,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
                case BackgroundJobStatus.Running:
                    return OnDeletingRunning(
                        context: context,
                        model: model);
                case BackgroundJobStatus.RunningOverdue:
                    return new ErrorData(
                        context: context,
                        type: Error.Types.CanNotPerformed,
                        sysLogsStatus: StatusCodes.Status400BadRequest,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
                default:
                    return new ErrorData(
                        context: context,
                        type: Error.Types.NotFound,
                        sysLogsStatus: StatusCodes.Status404NotFound,
                        sysLogsDescription: Debugs.GetSysLogsDescription());
            }
        }

        public static ErrorData OnEditing(
            Context context,
            BackgroundJobModel model)
        {
            if (model == null)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    sysLogsStatus: StatusCodes.Status404NotFound,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (CanAccess(
                context: context,
                model: model) == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    sysLogsStatus: StatusCodes.Status403Forbidden,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                sysLogsStatus: StatusCodes.Status200OK,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        public static ErrorData OnDownloading(
            Context context,
            BackgroundJobModel model)
        {
            if (CanAccess(
                context: context,
                model: model) == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    sysLogsStatus: StatusCodes.Status403Forbidden,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (model.Status != BackgroundJobStatus.Completed)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    sysLogsStatus: StatusCodes.Status404NotFound,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (model.File.IsNullOrEmpty())
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    sysLogsStatus: StatusCodes.Status404NotFound,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            var ss = GetExistingSiteSettingsOrNull(
                context: context,
                model: model);
            if (ss != null
                && context.CanExport(ss: ss) == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.NotFound,
                    sysLogsStatus: StatusCodes.Status403Forbidden,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                sysLogsStatus: StatusCodes.Status200OK,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        private static ErrorData OnDeletingRunning(
            Context context,
            BackgroundJobModel model)
        {
            if (BackgroundJobQueue.IsTimedOut(model: model))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.CustomError,
                    sysLogsStatus: StatusCodes.Status400BadRequest,
                    sysLogsDescription: Debugs.GetSysLogsDescription(),
                    data: Displays.Get(
                        context: context,
                        id: "BackgroundJobRunningTimedOutCanNotDelete"));
            }
            return new ErrorData(
                context: context,
                type: Error.Types.CanNotDelete,
                sysLogsStatus: StatusCodes.Status400BadRequest,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        private static SiteSettings GetExistingSiteSettingsOrNull(
            Context context,
            BackgroundJobModel model)
        {
            var dataTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesDefaultColumns(),
                    where: Rds.SitesWhere()
                        .TenantId(model.TenantId)
                        .SiteId(model.SiteId),
                    top: 1));
            if (dataTable.Rows.Count == 0)
            {
                return null;
            }
            return SiteSettingsUtilities.Get(
                context: context,
                dataRow: dataTable.Rows[0]);
        }
    }
}
