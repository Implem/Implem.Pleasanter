using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
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
        public static int TenantId;
        public static TenantModel Tenant;
        public static Dictionary<string, DeptModel> Depts;
        public static Dictionary<string, GroupModel> Groups;
        public static Dictionary<string, UserModel> Users;
        public static Dictionary<string, ItemModel> Items;
        public static Dictionary<string, SiteModel> Sites;
        public static Dictionary<string, Dictionary<string, IssueModel>> Issues;
        public static Dictionary<string, Dictionary<string, ResultModel>> Results;
        public static Dictionary<string, Dictionary<string, WikiModel>> Wikis;

        public static void Initialize()
        {
            DefinitionAccessor.Initializer.Initialize(
                path: null,
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                pleasanterTest: true);
            Context context = ContextData.Get(
                userType: ContextData.UserTypes.Anonymous,
                httpMethod: "post",
                absolutePath: "/demos/register",
                forms: new Forms()
                {
                    {
                        "Users_DemoMailAddress",
                        "DemoUser@example.com"
                    }
                });
            DemoUtilities.Register(
                context: context,
                async: false);
            context.TenantId = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn().TenantId(function: Sqls.Functions.Max)));
            TenantId = context.TenantId;
            Tenant = new TenantModel(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: TenantId);
            Depts = new DeptCollection(
                context: context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
                where: Rds.DeptsWhere().TenantId(TenantId))
                    .ToDictionary(o => o.DeptName, o => o);
            Groups = new GroupCollection(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                where: Rds.GroupsWhere().TenantId(TenantId))
                    .ToDictionary(o => o.GroupName, o => o);
            Users = new UserCollection(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                where: Rds.UsersWhere().TenantId(TenantId))
                    .ToDictionary(o => o.Name, o => o);
            Sites = new SiteCollection(
                context: context,
                where: Rds.SitesWhere().TenantId(TenantId))
                    .ToDictionary(o => o.Title.Value, o => o);
            Items = new ItemCollection(
                context: context,
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
                            context: context,
                            ss: SiteSettingsUtilities.Get(
                                context: context,
                                siteId: siteModel.SiteId),
                            where: Rds.IssuesWhere().SiteId_In(Sites.Values.Select(o => o.SiteId)))
                                .ToDictionary(o => o.Title.Value, o => o));
                        break;
                    case "Results":
                        Results.Add(siteModel.Title.Value, new ResultCollection(
                            context: context,
                            ss: SiteSettingsUtilities.Get(
                                context: context,
                                siteId: siteModel.SiteId),
                            where: Rds.ResultsWhere().SiteId_In(Sites.Values.Select(o => o.SiteId)))
                                .ToDictionary(o => o.Title.Value, o => o));
                        break;
                    case "Wikis":
                        Wikis.Add(siteModel.Title.Value, new WikiCollection(
                            context: context,
                            ss: SiteSettingsUtilities.Get(
                                context: context,
                                siteId: siteModel.SiteId),
                            where: Rds.WikisWhere().SiteId_In(Sites.Values.Select(o => o.SiteId)))
                                .ToDictionary(o => o.Title.Value, o => o));
                        break;
                }
            });
            // 管理者ユーザのパスワードを初期化します。
            Rds.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhere()
                        .TenantId(TenantId)
                        .TenantManager(true),
                    param: Rds.UsersParam()
                        .Password("ABCDEF".Sha512Cng())));
        }
    }
}