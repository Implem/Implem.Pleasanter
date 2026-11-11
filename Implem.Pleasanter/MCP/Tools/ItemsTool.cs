using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.MCP.McpContext;
using Implem.Pleasanter.MCP.Models;
using Implem.Pleasanter.MCP.Translator;
using Implem.Pleasanter.MCP.Utilities;
using Implem.Pleasanter.Models;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using static Implem.Pleasanter.MCP.Utilities.CommonUtilities;

namespace Implem.Pleasanter.MCP.Tools
{
    [McpServerToolType]
    [Description(@"
Pleasanter のレコード（データ）を操作するツール群です。

【検索】CreateViewJson → GetItems の viewJson に渡す
【作成】CreateItemJson → AddItem の itemDataJson に渡す
【更新】CreateItemJson → UpdateItem の itemDataJson に渡す
【削除】DeleteItem にレコード ID を指定
【単一取得】GetItem にレコード ID を指定")]
    public class ItemsTool
    {
        private const string ClassName = nameof(ItemsTool);

        [McpServerTool(Name = "AddItem", ReadOnly = false, Destructive = true, OpenWorld = true)]
        [Description(@"
指定サイトに新しいレコードを作成します。
itemDataJson は CreateItemJson で生成するか、直接 JSON 文字列を指定します。
添付ファイルを含める場合は、次のいずれかの方法で指定します。
1) CreateItemJson を利用する場合: itemData に添付ファイル項目（例: 添付ファイルA）をキーとして含めてください。
2) itemDataJson を直接指定する場合: 各添付ファイル項目に対応する `AttachmentsHash` フィールドを JSON 内に含めてください。")]
        public static async Task<CallToolResult> AddItem(
        [Description(@"レコードを追加するサイト ID。")]
            long siteId,
        [Description(@"作成データの JSON 文字列。CreateItemJson で生成可能。
項目名は resource://pleasanter/specs/item-fields を参照。")]
            string itemDataJson)
        {
            var toolPermission = new ToolPermission(nameof(AddItem));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(AddItem));
            return await Execute(
                actionType: ActionType.Create,
                siteId: siteId,
                referenceId: siteId,
                apiRequestJson: itemDataJson);
        }

        [McpServerTool(Name = "CreateItemJson", ReadOnly = true, Destructive = false, OpenWorld = true)]
        [Description(@"
レコード作成・更新用の JSON 文字列を生成します。
このツールは JSON を生成するだけで、実際の作成・更新は行いません。
生成した JSON を AddItem または UpdateItem の itemDataJson パラメータに渡してください。
添付ファイルを含める場合は itemData に添付ファイル項目（例: 添付ファイルA）をキーとして含めてください。
値はファイルオブジェクトの配列で、各オブジェクトに Name（ファイル名）、ContentType（MIMEタイプ）、Base64（ファイル内容のBase64エンコード文字列）を指定します。")]
        public static async Task<CallToolResult> CreateItemJson(
        [Description(@"対象のサイト ID。")]
            long siteId,
        [Description(@"作成・更新データ。キー:項目名（日本語表示名可）、値:設定する値。
分類項目は日本語表示値から内部コードに自動変換。
添付ファイル項目の値はオブジェクト配列: [{""Name"":""ファイル名"",""ContentType"":""MIMEタイプ"",""Base64"":""Base64文字列""}]")]
            Dictionary<string, object> itemData)
        {
            var toolPermission = new ToolPermission(nameof(CreateItemJson));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(CreateItemJson));
            var context = CreateContext();

            if (!TenantQuotaUsagesUtilities.TryWithinQuotaKeyLimit(
                context: context,
                quotaKey: QuotaKeys.McpRequests,
                errorType: out var errorType,
                errorData: out var errorData))
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: errorType,
                    data: errorData ?? Array.Empty<string>());
            }

            try
            {
                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId,
                    referenceId: siteId);

                var translator = new CodeTranslator(
                    context: context,
                    ss: ss);

                var itemJson = translator.TranslateToCodeString(data: itemData);

                return CallToolResultUtilities.CreateCallToolResult(text: itemJson);
            }
            catch (Exception ex)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: Error.Types.InternalServerError,
                    data: ex.Message);
            }
        }

        [McpServerTool(Name = "DeleteItem", ReadOnly = false, Destructive = true, OpenWorld = true)]
        [Description(@"
指定したレコードを削除します。削除は元に戻せません。
実行前に必ずユーザーに確認してください。")]
        public static async Task<CallToolResult> DeleteItem(
            [Description(@"削除対象のレコード ID。")]
                long referenceId)
        {
            var toolPermission = new ToolPermission(nameof(DeleteItem));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(DeleteItem));
            return await Execute(
                actionType: ActionType.Delete,
                referenceId: referenceId);
        }

        [McpServerTool(Name = "GetItem", ReadOnly = true, Destructive = false, OpenWorld = true)]
        [Description(@"指定したレコード ID の詳細情報を取得します。")]
        public static async Task<CallToolResult> GetItem(
            [Description(@"取得対象のレコード ID。")]
                long referenceId)
        {
            var toolPermission = new ToolPermission(nameof(GetItem));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetItem));
            return await Execute(
                actionType: ActionType.Get,
                referenceId: referenceId);
        }

        [McpServerTool(Name = "GetItems", ReadOnly = true, Destructive = false, OpenWorld = true)]
        [Description(@"
指定サイトのレコード一覧を取得します。
viewJson で検索条件・ソート順を指定可能（CreateViewJson で作成）。
デフォルトの PageSize は 200 件です。")]
        public static async Task<CallToolResult> GetItems(
            [Description(@"取得対象のサイト ID。")]
                long siteId,
            [Description(@"検索条件の View JSON 文字列。CreateViewJson で作成。空文字の場合はデフォルトビューで取得。")]
                string viewJson = "",
            [Description(@"取得開始位置。PageSize 200 のため、2ページ目は 200、3ページ目は 400 を指定。")]
                int offset = 0)
        {
            var toolPermission = new ToolPermission(nameof(GetItems));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetItems));

            var apiRequestJson = (offset > 0)
                ? JsonConvert.SerializeObject(new
                {
                    Offset = offset
                })
                : string.Empty;

            return await Execute(
                actionType: ActionType.Get,
                siteId: siteId,
                referenceId: siteId,
                apiRequestJson: apiRequestJson,
                viewJson: viewJson);
        }

        [McpServerTool(Name = "UpdateItem", ReadOnly = false, Destructive = true, OpenWorld = true)]
        [Description(@"
指定したレコードを更新します。
itemDataJson は CreateItemJson で生成するか、直接 JSON 文字列を指定します。
添付ファイルを含める場合は、次のいずれかの方法で指定します。
・CreateItemJson を使用する場合は、itemData に添付ファイル項目を含めてください。
・itemDataJson に JSON を直接指定する場合は、添付ファイル列に対応する AttachmentsHash を itemDataJson に含めてください。")]
        public static async Task<CallToolResult> UpdateItem(
        [Description(@"更新対象のレコード ID。")]
            long referenceId,
        [Description(@"更新データの JSON 文字列。CreateItemJson で生成可能。項目名は resource://pleasanter/specs/item-fields を参照。")]
            string itemDataJson)
        {
            var toolPermission = new ToolPermission(nameof(UpdateItem));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(UpdateItem));
            return await Execute(
                actionType: ActionType.Update,
                referenceId: referenceId,
                apiRequestJson: itemDataJson);
        }

        private static async Task<CallToolResult> Execute(
            ActionType actionType,
            long siteId = 0,
            long referenceId = 0,
            string apiRequestJson = "",
            string viewJson = "")
        {
            var context = CreateContext(
                apiRequestJson: apiRequestJson,
                viewJson: viewJson,
                siteId: siteId,
                referenceId: referenceId);

            try
            {
                if (context.InvalidJsonData)
                {
                    return CallToolResultUtilities.ToError(
                        context: context,
                        type: Error.Types.InvalidJsonData);
                }

                if (!context.Authenticated)
                {
                    return CallToolResultUtilities.ToError(
                        context: context,
                        type: Error.Types.Unauthorized);
                }

                if (!TenantQuotaUsagesUtilities.TryWithinQuotaKeyLimit(
                        context: context,
                        quotaKey: QuotaKeys.McpRequests,
                        errorType: out var errorType,
                        errorData: out var errorData))
                {
                    return CallToolResultUtilities.ToError(
                        context: context,
                        type: errorType,
                        data: errorData ?? Array.Empty<string>());
                }

                var itemModel = new ItemModel(
                    context: context,
                    referenceId: referenceId);

                var result = actionType switch
                {
                    ActionType.Get =>
                        itemModel.GetByApi(context: context),

                    ActionType.Create =>
                        itemModel.CreateByApi(context: context),

                    ActionType.Delete =>
                        itemModel.DeleteByApi(context: context),

                    ActionType.Update =>
                        itemModel.UpdateByApi(context: context),

                    _ => ApiResults.BadRequest(context: context)
                };

                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: itemModel.SiteId);

                return CallToolResultUtilities.ToCallToolResult(
                    context: context,
                    result: result,
                    ss: ss);
            }
            catch (Exception ex)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: Error.Types.InternalServerError,
                    data: ex.Message);
            }
        }
    }
}
