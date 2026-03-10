using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.MCP.McpContext;
using Implem.Pleasanter.MCP.Models;
using Implem.Pleasanter.MCP.Utilities;
using Implem.Pleasanter.Models;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using static Implem.Pleasanter.MCP.Utilities.CommonUtilities;
using static Implem.Pleasanter.MCP.Utilities.SearchUtilities;

namespace Implem.Pleasanter.MCP.Tools
{
    [McpServerToolType]
    [Description(@"
Pleasanter のサイト（テーブル定義）を管理するツール群です。

【検索】GetSiteIdByTitle でサイト名からサイト ID を特定
【取得】GetSite でサイトの設定情報を取得
【更新】UpdateSite でサイトの設定を更新")]
    public class SitesTool
    {
        private const string ClassName = nameof(SitesTool);

        [McpServerTool(Name = "GetSite")]
        [Description(@"
指定サイトの設定情報（SiteSettings）を取得します。
タイトル、参照タイプ、権限情報、作成者・更新者情報等を返します。")]
        public static async Task<CallToolResult> GetSite(
            [Description(@"取得対象のサイト ID。")]
                long siteId)
        {
            var toolPermission = new ToolPermission(nameof(GetSite));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetSite));
            return await Execute(
                actionType: ActionType.Get,
                siteId: siteId);
        }

        [McpServerTool(Name = "GetSiteIdByTitle")]
        [Description(@"
サイト名（タイトル）からサイト ID を検索します。
取得した ID は GetItems や CreateViewJson 等のパラメータに使用できます。")]
        public static async Task<CallToolResult> GetSiteIdByTitle(
            [Description(@"検索するサイトのタイトル。大文字・小文字区別なし。")]
                string title,
            [Description(@"検索モード。""ExactMatch""(完全一致/デフォルト), ""PartialMatch""(部分一致), ""ForwardMatch""(前方一致)")]
                string searchType = DefaultSearchTypeName)
        {
            var toolPermission = new ToolPermission(nameof(GetSiteIdByTitle));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetSiteIdByTitle));
            return await Execute(
                actionType: ActionType.GetByName,
                siteTitle: title,
                searchType: ParseSearchType(searchType));
        }

        [Description(@"
指定サイトの設定を更新します。
Title, ReferenceType, ParentId, InheritPermission, SiteSettings 等を更新可能。")]
        public static async Task<CallToolResult> UpdateSite(
            [Description(@"更新対象のサイト ID。")]
                long siteId,
            [Description(@"更新データの JSON 文字列。更新したい項目のみ指定。項目名は resource://pleasanter/specs/site-settings を参照。")]
                string siteDataJson)
        {
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(UpdateSite));
            return await Execute(
                actionType: ActionType.Update,
                siteId: siteId,
                apiRequestJson: siteDataJson);
        }

        private static async Task<CallToolResult> Execute(
            ActionType actionType,
            long siteId = 0,
            string siteTitle = "",
            SearchTypes searchType = SearchTypes.ExactMatch,
            string apiRequestJson = "")
        {
            var context = CreateContext(
                siteId: siteId,
                apiRequestJson: apiRequestJson);

            try
            {
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
                    referenceId: siteId);

                const string referenceType = "Sites";
                var result = actionType switch
                {
                    ActionType.Get =>
                        itemModel.GetByApi(
                            context: context,
                            referenceType: referenceType),

                    ActionType.GetByName =>
                        SearchAndCreateResultForSites(
                            context: context,
                            searchType: searchType,
                            searchValue: siteTitle),

                    ActionType.Update =>
                        itemModel.UpdateByApi(
                            context: context,
                            referenceType: referenceType),

                    _ => ApiResults.BadRequest(context: context)
                };

                return CallToolResultUtilities.ToCallToolResult(
                    context: context,
                    result: result);
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
