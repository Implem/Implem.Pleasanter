using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
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
Pleasanter のユーザー情報を操作するツール群です。

【検索】GetUserIdByName でユーザー名からユーザー ID を特定
【一覧】CreateViewJson → GetUsers の viewJson に渡す
【メールアドレス取得】CreateViewJson で apiGetMailAddresses=true を指定 → GetUsers に渡す")]
    public class UsersTool
    {
        private const string ClassName = nameof(UsersTool);

        [McpServerTool(Name = "GetUserIdByName")]
        [Description(@"
ユーザー名からユーザー ID を検索します。
取得した ID は他のツールのパラメータに使用できます。")]
        public static async Task<CallToolResult> GetUserIdByName(
            [Description(@"検索するユーザー名。")]
                string userName,
            [Description(@"検索モード。""ExactMatch""(完全一致/デフォルト), ""PartialMatch""(部分一致), ""ForwardMatch""(前方一致)")]
                string searchType = DefaultSearchTypeName)
        {
            var toolPermission = new ToolPermission(nameof(GetUserIdByName));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetUserIdByName));
            return await Execute(
                actionType: ActionType.GetByName,
                userName: userName,
                searchType: ParseSearchType(searchType));
        }

        [McpServerTool(Name = "GetUsers")]
        [Description(@"
ユーザー一覧を取得します。
viewJson で検索条件を指定可能（CreateViewJson で作成）。
メールアドレスを含める場合は CreateViewJson で apiGetMailAddresses=true を指定。")]
        public static async Task<CallToolResult> GetUsers(
            [Description(@"検索条件の View JSON 文字列。CreateViewJson で作成。空文字の場合は全ユーザーを取得。")]
            string viewJson = "")
        {
            var toolPermission = new ToolPermission(nameof(GetUsers));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetUsers));
            return await Execute(
                actionType: ActionType.Get,
                viewJson: viewJson);
        }

        private static async Task<CallToolResult> Execute(
            ActionType actionType,
            int userId = 0,
            string userName = "",
            SearchTypes searchType = SearchTypes.ExactMatch,
            string viewJson = "")
        {
            var context = CreateContext(viewJson: viewJson);

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

                var ss = SiteSettingsUtilities.ApiUsersSiteSettings(context: context);

                var result = actionType switch
                {
                    ActionType.Get =>
                        UserUtilities.GetByApi(
                            context: context,
                            ss: ss,
                            userId: userId),

                    ActionType.GetByName =>
                        SearchAndCreateResultForUsers(
                            context: context,
                            searchType: searchType,
                            searchValue: userName,
                            ss: ss),

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
