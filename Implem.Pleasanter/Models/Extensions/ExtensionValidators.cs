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
    public static class ExtensionValidators
    {
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

            //TODO: チェックが適切かを確認する。 context.HasPermission(ss: ss) は False になる
            //if (!context.HasPermission(ss: ss))
            //    return new ErrorData(
            //        context: context,
            //        type: context.CanRead(ss: ss) ? Error.Types.HasNotPermission : Error.Types.None,
            //        api: api,
            //        sysLogsStatus: 403,
            //        sysLogsDescription: Debugs.GetSysLogsDescription());

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
                //TODO: ServerScriptの関係あるか確認する
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None) return apiErrorData;
            }

            //TODO: 権限のチェックが適切かを確認する
            if (!context.CanCreate(ss: ss))
                return new ErrorData(
                    context: context,
                    type: context.CanRead(ss: ss) ? Error.Types.HasNotPermission : Error.Types.NotFound,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            var checkColumns = ss.Columns.Where(
                o => !o.CanCreate(context: context, ss: ss, mine: extensionModel.Mine(context: context)) &&
                     !ss.FormulaTarget(o.ColumnName));
            foreach (var column in checkColumns)
            {
                if (IsColumnUpdated(extensionModel, column.ColumnName, context))
                    return GetErrorDataOfHasNotChangeColumnPermission(context, column.LabelText, api);
            }

            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                extensionModel: extensionModel).FirstOrDefault();
            if (inputErrorData.Type != Error.Types.None) return inputErrorData;

            return SuccessData(context: context, api: api);

        }

        public static List<ErrorData> OnInputValidating(
            Context context,
            SiteSettings ss,
            Dictionary<int, ExtensionModel> extensionHash,
            bool api = false)
        {
            var errors = extensionHash
                             ?.OrderBy(data => data.Key)
                             .SelectMany((data, index) => OnInputValidating(
                                 context: context,
                                 ss: ss,
                                 extensionModel: data.Value,
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
            ExtensionModel extensionModel,
            int rowNo = 0)
        {
            //TODO: 処理の内容を確認する。Extensionsでは editorColumns をどうみるべきか？
            var errors = new List<ErrorData>();
            var editorColumns = ss.GetEditorColumns(context: context); //context使われてないので不要。ssの内容によって設定されれる。
            editorColumns
                ?.Concat(ss.Columns
                    ?.Where(o => !o.NotEditorSettings)
                    .Where(column => !editorColumns
                        .Any(editorColumn => editorColumn.ColumnName == column.ColumnName)))
                .ForEach(column =>
                {　
                    var value = extensionModel.PropertyValue(
                        context: context,
                        column: column);
                    if (column.TypeCs == "Comments")
                    {
                        var savedCommentId = extensionModel.SavedComments?.Deserialize<Comments>()?.Max(savedComment => (int?)savedComment.CommentId) ?? default(int);
                        var comment = value?.Deserialize<Comments>()?.FirstOrDefault();
                        value = comment?.CommentId > savedCommentId ? comment?.Body : null;
                    }
                    if (!value.IsNullOrEmpty())
                    {
                        //TODO: これらの関数の中でErrosにAddしてるけど、わかりにくいので修正したいところ…
                        Validators.ValidateMaxLength(
                            columnName: column.ColumnName,
                            maxLength: column.MaxLength,
                            errors: errors,
                            value: value);
                        Validators.ValidateRegex(
                            columnName: column.ColumnName,
                            serverRegexValidation: column.ServerRegexValidation,
                            regexValidationMessage: column.RegexValidationMessage,
                            errors: errors,
                            value: value);
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


        public static ErrorData OnUpdating(
            Context context,
            SiteSettings ss,
            ExtensionModel extensionModel,
            bool api = false,
            bool serverScript = false)
        {
            // TODO: こちらのチェックと、後のCanCreqateのチェックとの兼ね合いを確認して調整する。またOnCreattingにはないけどよいのか？
            //if (extensionModel.RecordPermissions != null && !context.CanManagePermission(ss: ss))
            //{
            //    return new ErrorData(
            //        context: context,
            //        type: Error.Types.HasNotPermission,
            //        api: api,
            //        sysLogsStatus: 403,
            //        sysLogsDescription: Debugs.GetSysLogsDescription());
            //}

            if (api)
            {
                var apiErrorData = Validators.ValidateApi(context: context, serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None) return apiErrorData;
            }

            var checkColumns = ss.Columns.Where(
                o => !o.CanUpdate(context: context, ss: ss, mine: extensionModel.Mine(context: context)) &&
                     !ss.FormulaTarget(o.ColumnName));
            foreach (var column in checkColumns)
            {
                if (IsColumnUpdated(extensionModel, column.ColumnName, context))
                    return GetErrorDataOfHasNotChangeColumnPermission(context, column.LabelText, api);
            }

            //TODO: 権限のチェックが適切かを確認する
            //TODO:  Extensionのモデルベースのモデルを別にしたい
            if (!context.CanUpdate(ss: ss))
                return new ErrorData(
                    context: context,
                    type: !context.CanRead(ss: ss) ? Error.Types.NotFound : Error.Types.HasNotPermission,
                    api: api,
                    sysLogsStatus: 403,
                    sysLogsDescription: Debugs.GetSysLogsDescription());

            //TODO: 必要かどうか確認する
            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                extensionModel: extensionModel).FirstOrDefault();
            if (inputErrorData?.Type != Error.Types.None) return inputErrorData;

            return SuccessData(context: context, api: api);
        }

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
