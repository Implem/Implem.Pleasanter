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
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                //TODO: ServerScriptの関係あるか確認する
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }

            //TODO: 適切なチェックを確認する（Returnの内容は、ResultValidators.OnGet のようにもう少し詳細に書く？）
            return Permissions.CanManageUser(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
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
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }

            //TODO: ss.LockedTable() を見る必要があるか確認する
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

            //TODO: HasNotChangeColumnPermission のチェック必要性を確認する

            // 添付ファイルに関するものなので不要なはず
            //var errorData = OnAttaching(
            //    context: context,
            //    ss: ss,
            //    extensionModel: extensionModel);
            //if (errorData.Type != Error.Types.None)
            //{
            //    return errorData;
            //}

            var inputErrorData = OnInputValidating(
                context: context,
                ss: ss,
                extensionModel: extensionModel).FirstOrDefault();
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
                ?.Concat(ss
                    .Columns
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
    }
}
