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
【更新】CreateUpdateItemJson → UpdateItem の itemDataJson に渡す
【単一取得】GetItem にレコード ID を指定")]
    public class ItemsTool
    {
        private const string ClassName = nameof(ItemsTool);

        [McpServerTool(Name = "CreateUpdateItemJson")]
        [Description(@"
レコード更新用の JSON 文字列を生成します。
このツールは JSON を生成するだけで、実際の更新は行いません。
生成した JSON を UpdateItem の itemDataJson パラメータに渡して更新を実行してください。")]
        public static async Task<CallToolResult> CreateUpdateItemJson(
        [Description(@"更新対象のレコード ID。")]
            long referenceId,
        [Description(@"更新データ。キー:項目名（日本語表示名可）、値:設定する値。分類項目は日本語表示値から内部コードに自動変換。")]
            Dictionary<string, object> updateItemData)
        {
            var toolPermission = new ToolPermission(nameof(CreateUpdateItemJson));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(CreateUpdateItemJson));
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
                var itemModel = new ItemModel(
                    context: context,
                    referenceId: referenceId);

                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: itemModel.SiteId,
                    referenceId: referenceId);

                var translator = new CodeTranslator(
                    context: context,
                    ss: ss);

                var updateItemJson = translator.TranslateToCodeString(data: updateItemData);

                return CallToolResultUtilities.CreateCallToolResult(text: updateItemJson);
            }
            catch (Exception ex)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: Error.Types.InternalServerError,
                    data: ex.Message);
            }
        }

        [McpServerTool(Name = "GetItem")]
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

        [McpServerTool(Name = "GetItems")]
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

        [McpServerTool(Name = "UpdateItem")]
        [Description(@"
指定したレコードを更新します。
itemDataJson は CreateUpdateItemJson で生成するか、直接 JSON 文字列を指定します。")]
        public static async Task<CallToolResult> UpdateItem(
        [Description(@"更新対象のレコード ID。")]
            long referenceId,
        [Description(@"更新データの JSON 文字列。CreateUpdateItemJson で生成可能。項目名は resource://pleasanter/specs/item-fields を参照。")]
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
