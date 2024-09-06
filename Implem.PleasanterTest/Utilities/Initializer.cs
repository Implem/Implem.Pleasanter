using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
namespace Implem.PleasanterTest.Utilities
{
    /// <summary>
    /// 新しいテナントを作成し、デモデータからテスト用のデータをデータベース内に作成します。
    /// </summary>
    public static partial class Initializer
    {
        public static Context Context;
        public static int TenantId;
        public static TenantModel Tenant;
        public static Dictionary<int, DeptModel> Depts;
        public static Dictionary<int, GroupModel> Groups;
        public static Dictionary<int, UserModel> Users;
        public static List<GroupMemberModel> GroupMembers;
        public static Dictionary<string, ItemModel> Items;
        public static Dictionary<string, SiteModel> Sites;
        public static Dictionary<string, Dictionary<string, IssueModel>> Issues;
        public static Dictionary<string, Dictionary<string, ResultModel>> Results;
        public static Dictionary<string, Dictionary<string, WikiModel>> Wikis;
        public static Dictionary<string, long> Titles = new Dictionary<string, long>();
        public static Dictionary<long, ItemModel> ItemIds = new Dictionary<long, ItemModel>();
        //実行結果を保存する場合に保存先のフォルダ名を指定
        private static readonly string resultsSaveDir = string.Empty;

        //実行結果の日付、GUID、トークンをマスクする正規表現
        [System.Text.RegularExpressions.GeneratedRegex(@"\d{4}-\d{1,2}-\d{1,2}")]
        private static partial System.Text.RegularExpressions.Regex Date1Regex();

        [System.Text.RegularExpressions.GeneratedRegex(@"\d{4}/\d{1,2}/\d{1,2}")]
        private static partial System.Text.RegularExpressions.Regex Date2Regex();

        [System.Text.RegularExpressions.GeneratedRegex(@"\d{1,2}:\d{1,2}(:\d{1,2})?(\.\d{3})?")]
        private static partial System.Text.RegularExpressions.Regex TimeRegex();

        [System.Text.RegularExpressions.GeneratedRegex(@"\d{1,3} [分秒日](前|超過)")]
        private static partial System.Text.RegularExpressions.Regex BeforeRegex();

        [System.Text.RegularExpressions.GeneratedRegex(@"data-id=\\""(\w+)\\""")]
        private static partial System.Text.RegularExpressions.Regex DataIdRegex();

        [System.Text.RegularExpressions.GeneratedRegex(@"\/\?(\d*)(\\)?""")]
        private static partial System.Text.RegularExpressions.Regex Token1Regex();

        [System.Text.RegularExpressions.GeneratedRegex(@"binaries\/(\w+)\/")]
        private static partial System.Text.RegularExpressions.Regex BinariesRegex();

        public static void SaveResults(string result, [CallerFilePath] string callerFilePath = "")
        {
            if (resultsSaveDir.IsNullOrEmpty())
            {
                return;
            }
            Directory.CreateDirectory(resultsSaveDir);
            var fileName = Path.Combine(resultsSaveDir, Path.GetFileName(callerFilePath));

            result = Date1Regex().Replace(result, "****-**-**");
            result = Date2Regex().Replace(result, "****/**/**");
            result = TimeRegex().Replace(result, "**:**:**");
            result = DataIdRegex().Replace(result, "data-id=\"****\"");
            result = Token1Regex().Replace(result, "/?******\"");
            result = BinariesRegex().Replace(result, "binaries/******/");
            result = BeforeRegex().Replace(result, "** **");
            result = result.Replace(">", ">" + System.Environment.NewLine);
            System.IO.File.WriteAllText(fileName, result);
        }

        public static void Initialize()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            DefinitionAccessor.Initializer.Initialize(
                path: null,
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                pleasanterTest: true);
            Parameters.Service.DefaultLanguage = "ja";
            Context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                httpMethod: "post",
                absolutePath: "/demos/register",
                forms: FormsUtilities.Get(
                    new KeyValue("Users_DemoMailAddress", "DemoUser@example.com")));
            DemoUtilities.Register(
                context: Context,
                async: false,
                sendMail: false);
            TenantId = Rds.ExecuteScalar_int(
                context: Context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn().TenantId(function: Sqls.Functions.Max)));
            Rds.ExecuteNonQuery(
                context: Context,
                statements: Rds.UpdateTenants(
                    param: Rds.TenantsParam().ContractSettings("{\"Extensions\":{\"Publish\":true}}"),
                    where: Rds.TenantsWhere().TenantId(TenantId)));
            Parameters.Security.PrivilegedUsers = new List<string>()
            {
                $"Tenant{TenantId}_User20"
            };
            Context.TenantId = TenantId;
            Tenant = new TenantModel(
                context: Context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: Context),
                tenantId: TenantId);
            Depts = new DeptCollection(
                context: Context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: Context),
                where: Rds.DeptsWhere().TenantId(TenantId))
                    .ToDictionary(o => o.DeptId, o => o);
            Groups = new GroupCollection(
                context: Context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: Context),
                where: Rds.GroupsWhere().TenantId(TenantId))
                    .ToDictionary(o => o.GroupId, o => o);
            Users = new UserCollection(
                context: Context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: Context),
                where: Rds.UsersWhere().TenantId(TenantId))
                    .ToDictionary(o => o.UserId, o => o);
            GroupMembers = new GroupMemberCollection(
                context: Context,
                where: Rds.GroupMembersWhere().GroupId_In(Groups.Keys.ToList()));
            Sites = new SiteCollection(
                context: Context,
                where: Rds.SitesWhere().TenantId(TenantId))
                    .ToDictionary(o => o.Title.Value, o => o);
            Items = new ItemCollection(
                context: Context,
                where: Rds.ItemsWhere().SiteId_In(Sites.Values.Select(o => o.SiteId)))
                    .ToDictionary(o => o.Title, o => o);
            Issues = new Dictionary<string, Dictionary<string, IssueModel>>();
            Results = new Dictionary<string, Dictionary<string, ResultModel>>();
            Wikis = new Dictionary<string, Dictionary<string, WikiModel>>();
            Sites.Values.ForEach(siteModel =>
            {
                switch (siteModel.ReferenceType)
                {
                    case "Issues":
                        Issues.Add(siteModel.Title.Value, new IssueCollection(
                            context: Context,
                            ss: SiteSettingsUtilities.Get(
                                context: Context,
                                siteId: siteModel.SiteId),
                            where: Rds.IssuesWhere().SiteId(siteModel.SiteId))
                                .ToDictionary(o => o.Title.Value, o => o));
                        break;
                    case "Results":
                        Results.Add(siteModel.Title.Value, new ResultCollection(
                            context: Context,
                            ss: SiteSettingsUtilities.Get(
                                context: Context,
                                siteId: siteModel.SiteId),
                            where: Rds.ResultsWhere().SiteId(siteModel.SiteId))
                                .ToDictionary(o => o.Title.Value, o => o));
                        break;
                    case "Wikis":
                        Wikis.Add(siteModel.Title.Value, new WikiCollection(
                            context: Context,
                            ss: SiteSettingsUtilities.Get(
                                context: Context,
                                siteId: siteModel.SiteId),
                            where: Rds.WikisWhere().SiteId(siteModel.SiteId))
                                .ToDictionary(o => o.Title.Value, o => o));
                        break;
                }
            });
            Sites.ForEach(data => Titles.Add(data.Key, data.Value.SiteId));
            Issues.SelectMany(o => o.Value).ForEach(data => Titles.Add(data.Key, data.Value.IssueId));
            Results.SelectMany(o => o.Value).ForEach(data => Titles.Add(data.Key, data.Value.ResultId));
            Wikis.SelectMany(o => o.Value).ForEach(data => Titles.Add(data.Key, data.Value.WikiId));
            Items.ForEach(data => ItemIds.Add(data.Value.ReferenceId, data.Value));
            // ユーザのパスワードを初期化します。
            Rds.ExecuteNonQuery(
                context: Context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhere()
                        .TenantId(TenantId),
                    param: Rds.UsersParam()
                        .Password("ABCDEF".Sha512Cng())));
        }


    }
}