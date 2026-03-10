using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.MCP.McpContext;
using Implem.Pleasanter.MCP.Models;
using Implem.Pleasanter.MCP.Utilities;
using Implem.Pleasanter.Models;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static Implem.Pleasanter.MCP.Utilities.CommonUtilities;

namespace Implem.Pleasanter.MCP.Tools
{
    [McpServerToolType]
    [Description(@"
Pleasanter の View JSON 作成とビュー管理のツール群です。

【検索】CreateViewJson → GetItems/GetUsers の viewJson に渡す
【ビュー保存】CreateViewJson → AddView でサイトに登録
【ビュー操作】GetView / GetViewIdByViewName / UpdateView / CopyView / DeleteView")]
    public class ViewsTool
    {
        private const string ClassName = nameof(ViewsTool);
        private const string Id = "Id";
        private const string Name = "Name";
        private const string ViewJSON = "ViewJSON";
        private const string ViewLatestId = "ViewLatestId";
        private const string Views = "Views";

        [McpServerTool(Name = "AddView")]
        [Description(@"
指定サイトに新しいビューを追加します。
CreateViewJson で作成した View JSON を登録できます。")]
        public static async Task<CallToolResult> AddView(
            [Description(@"ビューを追加するサイト ID。")]
                long siteId,
            [Description(@"ビューの表示名。一覧画面のビュー切り替えで表示される名前。")]
                string viewName,
            [Description(@"ビュー設定の JSON 文字列。CreateViewJson で作成。")]
                string viewJson)
        {
            var toolPermission = new ToolPermission(nameof(AddView));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(AddView));
            return await Execute(
                actionType: ActionType.Create,
                siteId: siteId,
                viewName: viewName,
                viewJson: viewJson);
        }

        [McpServerTool(Name = "CopyView")]
        [Description(@"
指定サイトのビューをコピーして新しいビューを作成します。
コピー後のビュー名はコピー元と同じで、新しいビュー ID が自動割当されます。")]
        public static async Task<CallToolResult> CopyView(
            [Description(@"対象サイト ID。")]
                long siteId,
            [Description(@"コピー元のビュー ID。GetView で取得した Id を指定。")]
                int viewId)
        {
            var toolPermission = new ToolPermission(nameof(CopyView));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(CopyView));
            return await Execute(
                actionType: ActionType.Copy,
                siteId: siteId,
                viewId: viewId);
        }

        [McpServerTool(Name = "CreateViewJson")]
        [Description(@"
レコード検索用の View JSON を生成します。
このツールは JSON 文字列を生成するだけで、検索の実行やサイトへのビュー登録は行いません。
生成した JSON は GetItems/GetUsers の viewJson パラメータに渡して検索を実行してください。
columnFilterHash の詳細な値指定方法は resource://pleasanter/specs/view-json を参照。")]
        public static async Task<CallToolResult> CreateViewJson(
            [Description(@"検索対象のサイト ID。")]
        int siteId = 0,
            [Description(@"未完了レコードのみ対象にする場合は true。")]
        bool incomplete = false,
            [Description(@"自分が作成者のレコードのみ対象にする場合は true。")]
        bool own = false,
            [Description(@"期限が近いレコードのみ対象にする場合は true。")]
        bool nearCompletionTime = false,
            [Description(@"遅延レコードのみ対象にする場合は true。")]
        bool delay = false,
            [Description(@"期限超過レコードのみ対象にする場合は true。")]
        bool overdue = false,
            [Description(@"全文検索キーワード。")]
        string search = "",
            [Description(@"列ごとの検索条件。キー:列名（日本語表示名可）、値:検索値の配列。例: {""状況"": [""実施中""]}。詳細は resource://pleasanter/specs/view-json を参照。")]
        Dictionary<string, List<string>> columnFilterHash = null,
            [Description(@"列ごとの検索方法。キー:列名、値:""ExactMatch""|""PartialMatch""|""ForwardMatch""。複数値OR: ""ExactMatchMultiple""等。")]
        Dictionary<string, string> columnFilterSearchTypes = null,
            [Description(@"除外検索の対象列名リスト。プリセットフィルタ否定: ""ViewFilters_Incomplete""等。")]
        List<string> columnFilterNegatives = null,
            [Description(@"並び順。キー:列名（日本語表示名可）、値:""asc""|""desc""。例: {""期限"": ""desc""}")]
        Dictionary<string, string> columnSorterHash = null,
            [Description(@"返却項目の配列。日本語表示名可。親テーブル: ""リンク列名~親サイトID,取得項目名""")]
        List<string> gridColumns = null,
            [Description(@"GetUsers 使用時にメールアドレスを取得する場合は true。")]
        bool apiGetMailAddresses = false)
        {
            var toolPermission = new ToolPermission(nameof(CreateViewJson));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(CreateViewJson));
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
                var viewJson = ViewJsonUtilities.CreateViewJson(
                    siteId: siteId,
                    incomplete: incomplete,
                    own: own,
                    nearCompletionTime: nearCompletionTime,
                    delay: delay,
                    overdue: overdue,
                    search: search,
                    columnFilterHash: columnFilterHash,
                    columnFilterSearchTypes: columnFilterSearchTypes,
                    columnFilterNegatives: columnFilterNegatives,
                    columnSorterHash: columnSorterHash,
                    gridColumns: gridColumns,
                    apiGetMailAddresses: apiGetMailAddresses);

                var responseMessage = string.Format(
                    format: Generated,
                    arg0: ViewJSON);

                var response = new JObject
                {
                    ["View"] = viewJson,
                    ["Message"] = responseMessage,
                };

                return CallToolResultUtilities.CreateCallToolResult(text: response.ToString());
            }
            catch (Exception ex)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: Error.Types.InternalServerError,
                    data: ex.Message);
            }
        }

        [McpServerTool(Name = "DeleteView")]
        [Description(@"
指定サイトからビューを削除します。削除は元に戻せません。
viewId=0 で全ビュー削除。実行前に必ずユーザーに確認してください。")]
        public static async Task<CallToolResult> DeleteView(
            [Description(@"対象サイト ID。")]
                long siteId,
            [Description(@"削除するビュー ID。0 で全ビュー削除。")]
                int viewId = 0)
        {
            var toolPermission = new ToolPermission(nameof(DeleteView));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(DeleteView));
            return await Execute(
                actionType: ActionType.Delete,
                siteId: siteId,
                viewId: viewId);
        }

        [McpServerTool(Name = "GetView")]
        [Description(@"指定サイトのビュー設定を取得します。")]
        public static async Task<CallToolResult> GetView(
            [Description(@"対象サイト ID。")]
                long siteId,
            [Description(@"取得するビュー ID。")]
                int viewId)
        {
            var toolPermission = new ToolPermission(nameof(GetView));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetView));
            return await Execute(
                actionType: ActionType.Get,
                siteId: siteId,
                viewId: viewId);
        }

        [McpServerTool(Name = "GetViewIdByViewName")]
        [Description(@"
ビュー名からビュー ID を検索します（完全一致）。
取得した ID は UpdateView / DeleteView 等のパラメータに使用できます。")]
        public static async Task<CallToolResult> GetViewIdByViewName(
            [Description(@"対象サイト ID。")]
                long siteId,
            [Description(@"検索するビュー名。完全一致で検索。")]
                string viewName)
        {
            var toolPermission = new ToolPermission(nameof(GetViewIdByViewName));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(GetViewIdByViewName));
            return await Execute(
                actionType: ActionType.GetByName,
                siteId: siteId,
                viewName: viewName);
        }

        [McpServerTool(Name = "UpdateView")]
        [Description(@"
指定サイトの既存ビューを更新します。
フィルタ条件・ソート順の変更やビュー名の変更が可能です。")]
        public static async Task<CallToolResult> UpdateView(
            [Description(@"対象サイト ID。")]
                long siteId,
            [Description(@"更新するビュー ID。GetView で取得した Id を指定。")]
                int viewId,
            [Description(@"更新するビュー設定の JSON 文字列。CreateViewJson で作成。指定プロパティのみ上書き。")]
                string viewJson,
            [Description(@"ビュー表示名。省略時は既存名を維持。")]
                string viewName = "")
        {
            var toolPermission = new ToolPermission(nameof(UpdateView));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(UpdateView));
            return await Execute(
                actionType: ActionType.Update,
                siteId: siteId,
                viewId: viewId,
                viewName: viewName,
                viewJson: viewJson);
        }

        private static string BuildSiteSettingsJson(JObject ssJObject)
        {
            var settingsJson = new JObject
            {
                ["SiteSettings"] = ssJObject
            };

            return settingsJson.ToString();
        }

        private static (string json, ErrorData errorData) BuildSiteSettingsJsonForAddView(
            Context context,
            JObject ssJObject,
            int viewLatestId,
            string viewName,
            string viewJson)
        {
            var newViewLatestId = viewLatestId + 1;

            JObject addView;
            try
            {
                addView = CreateViewObject(
                    viewJson: viewJson,
                    id: newViewLatestId,
                    viewName: viewName);
            }
            catch (JsonReaderException)
            {
                return (null, new ErrorData(type: Error.Types.InvalidJsonData));
            }

            ssJObject[ViewLatestId] = newViewLatestId;

            var viewsArray = ssJObject[Views] as JArray ?? new JArray();
            viewsArray.Add(addView);
            ssJObject[Views] = viewsArray;

            return (BuildSiteSettingsJson(ssJObject), null);
        }

        private static (string json, ErrorData errorData) BuildSiteSettingsJsonForCopyView(
            Context context,
            JObject ssJObject,
            int viewLatestId,
            int viewId)
        {
            var (targetView, errorData) = FindViewById(
                context: context,
                ssJObject: ssJObject,
                viewId: viewId);

            if (errorData != null)
            {
                return (null, errorData);
            }

            var newViewLatestId = viewLatestId + 1;

            var copiedView = (JObject)targetView.DeepClone();
            copiedView[Id] = newViewLatestId;

            ssJObject[ViewLatestId] = newViewLatestId;

            var viewsArray = ssJObject[Views] as JArray ?? new JArray();
            viewsArray.Add(copiedView);
            ssJObject[Views] = viewsArray;

            return (BuildSiteSettingsJson(ssJObject), null);
        }

        private static (string json, ErrorData errorData) BuildSiteSettingsJsonForDeleteView(
            Context context,
            JObject ssJObject,
            int viewId)
        {
            if (viewId == 0)
            {
                var viewsArray = ssJObject[Views] as JArray ?? new JArray();
                viewsArray.Clear();
            }
            else
            {
                var (targetView, errorData) = FindViewById(
                    context: context,
                    ssJObject: ssJObject,
                    viewId: viewId);
                if (errorData != null)
                {
                    return (null, errorData);
                }

                targetView.Remove();
            }

            return (BuildSiteSettingsJson(ssJObject), null);
        }

        private static (string json, ErrorData errorData) BuildSiteSettingsJsonForUpdateView(
            Context context,
            JObject ssJObject,
            int viewId,
            string viewName,
            string viewJson)
        {
            JObject updateView;
            try
            {
                updateView = JObject.Parse(viewJson);
            }
            catch (JsonReaderException)
            {
                return (null, new ErrorData(type: Error.Types.InvalidJsonData));
            }

            var (targetView, errorData) = FindViewById(
                context: context,
                ssJObject: ssJObject,
                viewId: viewId);
            if (errorData != null)
            {
                return (null, errorData);
            }

            foreach (var property in updateView.Properties())
            {
                targetView[property.Name] = property.Value;
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                targetView[Name] = viewName;
            }

            targetView[Id] = viewId;

            return (BuildSiteSettingsJson(ssJObject), null);
        }

        private static JObject ConvertSiteSettingsToJObject(SiteSettings ss)
        {
            if (ss == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            var ssJson = JsonConvert.SerializeObject(
                value: ss,
                settings: settings);

            return JObject.Parse(json: ssJson);
        }

        private static CallToolResult CreateResultForGet(
            Context context,
            JObject ssJObject,
            int viewId)
        {
            var (targetView, errorData) = FindViewById(
                context: context,
                ssJObject: ssJObject,
                viewId: viewId);

            if (errorData != null)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    errorData: errorData);
            }

            return CallToolResultUtilities.CreateCallToolResult(
                text: targetView.ToString(Formatting.Indented),
                isError: false);
        }

        private static CallToolResult CreateResultForGetByName(
            JObject ssJObject,
            string viewName)
        {
            var viewsArray = ssJObject?[Views] as JArray ?? new JArray();

            var viewIds = viewsArray
                .Where(v => (string)v[Name] == viewName)
                .Select(v => (int?)v[Id])
                .Where(id => id.HasValue && id.Value != 0)
                .Select(id => id.Value)
                .ToList();

            var json = new JObject
            {
                ["Count"] = viewIds.Count,
                ["ViewIds"] = new JArray(viewIds)
            };

            return CallToolResultUtilities.CreateCallToolResult(text: json.ToString());
        }

        private static JObject CreateViewObject(
            string viewJson,
            int id,
            string viewName)
        {
            var view = JObject.Parse(json: viewJson);
            view[Id] = id;
            view[Name] = viewName;
            return view;
        }

        private static async Task<CallToolResult> Execute(
            ActionType actionType,
            long siteId,
            int viewId = 0,
            string viewName = "",
            string viewJson = "")
        {
            var context = CreateContext(siteId: siteId);

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

                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId);

                var ssJObject = ConvertSiteSettingsToJObject(ss);
                if (ssJObject == null)
                {
                    return CallToolResultUtilities.ToError(
                        context: context,
                        type: Error.Types.NotFound);
                }

                var viewLatestId = ss?.ViewLatestId ?? 1;

                async Task<CallToolResult> ProcessUpdateResult((string json, ErrorData errorData) result)
                {
                    return result.errorData != null
                        ? CallToolResultUtilities.ToError(
                            context: context,
                            errorData: result.errorData)
                        : await SitesTool.UpdateSite(
                            siteId: siteId,
                            siteDataJson: result.json);
                }

                return actionType switch
                {
                    ActionType.Get =>
                        CreateResultForGet(
                            context: context,
                            ssJObject: ssJObject,
                            viewId: viewId),

                    ActionType.GetByName =>
                        CreateResultForGetByName(
                            ssJObject: ssJObject,
                            viewName: viewName),

                    ActionType.Create =>
                        await ProcessUpdateResult(
                            BuildSiteSettingsJsonForAddView(
                                context: context,
                                ssJObject: ssJObject,
                                viewLatestId: viewLatestId,
                                viewName: viewName,
                                viewJson: viewJson)),

                    ActionType.Copy =>
                        await ProcessUpdateResult(
                            BuildSiteSettingsJsonForCopyView(
                                context: context,
                                ssJObject: ssJObject,
                                viewLatestId: viewLatestId,
                                viewId: viewId)),

                    ActionType.Delete =>
                        await ProcessUpdateResult(
                            BuildSiteSettingsJsonForDeleteView(
                                context: context,
                                ssJObject: ssJObject,
                                viewId: viewId)),

                    ActionType.Update =>
                        await ProcessUpdateResult(
                            BuildSiteSettingsJsonForUpdateView(
                                context: context,
                                ssJObject: ssJObject,
                                viewId: viewId,
                                viewName: viewName,
                                viewJson: viewJson)),

                    _ => CallToolResultUtilities.ToError(
                            context: context,
                            type: Error.Types.BadRequest)
                };
            }
            catch (Exception ex)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: Error.Types.InternalServerError,
                    data: ex.Message);
            }
        }

        private static (JToken view, ErrorData errorData) FindViewById(
            Context context,
            JObject ssJObject,
            int viewId)
        {
            var viewsArray = ssJObject?[Views] as JArray ?? new JArray();
            var targetView = viewsArray.FirstOrDefault(v => (int?)v[Id] == viewId);

            if (targetView == null)
            {
                return (null, new ErrorData(type: Error.Types.NotFound));
            }

            return (targetView, null);
        }
    }
}
