using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Implem.PleasanterTest.Utilities
{
    /// <summary>
    /// 新しいテナントを作成し、デモデータからテスト用のデータをデータベース内に作成します。
    /// </summary>
    public static class Initializer
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