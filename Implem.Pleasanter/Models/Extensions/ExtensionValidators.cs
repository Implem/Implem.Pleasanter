using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;

namespace Implem.Pleasanter.Models
{
    public static class ExtensionValidators
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
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                    return apiErrorData;
            }
            
            if (!context.CanRead(ss: ss))
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnGet(
            Context context,
            SiteSettings ss,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                    return apiErrorData;
            }

            if (!context.CanRead(ss: ss))
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            return SuccessData(context: context, api: api);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnCreating(
            Context context,
            SiteSettings ss,
            ExtensionModel extensionModel,
            bool copy = false,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None) return apiErrorData;
            }

            if (!context.CanCreate(ss: ss))
                return new ErrorData(
                    context: context,
                    type: context.CanRead(ss: ss) ? Error.Types.HasNotPermission : Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            return SuccessData(context: context, api: api);

        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnUpdating(
            Context context,
            SiteSettings ss,
            ExtensionModel extensionModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None) return apiErrorData;
            }

            if (!context.CanUpdate(ss: ss))
                return new ErrorData(
                    context: context,
                    type: !context.CanRead(ss: ss) ? Error.Types.NotFound : Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            return SuccessData(context: context, api: api);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnDeleting(
            Context context,
            SiteSettings ss,
            ExtensionModel extensionModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                    return apiErrorData;
            }

            if(!context.CanDelete(ss: ss))
                return new ErrorData(
                    context: context,
                    type: context.CanRead(ss: ss) ? Error.Types.HasNotPermission : Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            return SuccessData(context: context, api: api);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static bool IsColumnUpdated(ExtensionModel extensionModel, string columnName, Context context)
        {
            return columnName switch
            {
                "TenantId" => extensionModel.TenantId_Updated(context: context),
                "Ver" => extensionModel.Ver_Updated(context: context),
                "ExtensionType" => extensionModel.ExtensionType_Updated(context: context),
                "ExtensionName" => extensionModel.ExtensionName_Updated(context: context),
                "ExtensionSetting" => extensionModel.ExtensionSettings_Updated(context: context),
                "Body" => extensionModel.Body_Updated(context: context),
                "Description" => extensionModel.Description_Updated(context: context),
                "Disabled" => extensionModel.Disabled_Updated(context: context),
                "Comments" => extensionModel.Comments_Updated(context: context),
                _ => false,
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ErrorData GetErrorDataOfHasNotChangeColumnPermission(Context context, string columnLabelText, bool api)
        {
            return new ErrorData(
                context: context,
                type: Error.Types.HasNotChangeColumnPermission,
                data: columnLabelText,
                api: api,
                sysLogsStatus: 403,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ErrorData SuccessData(Context context, bool api)
        {
            return new ErrorData(
                context: context,
                type: Error.Types.None,
                api: api,
                sysLogsStatus: 200,
                sysLogsDescription: Debugs.GetSysLogsDescription());
        }
    }
}
