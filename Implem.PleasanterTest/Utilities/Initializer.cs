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
        public static List<DeptModel> Depts;
        public static List<GroupModel> Groups;
        public static List<UserModel> Users;
        public static List<ItemModel> Items;
        public static List<SiteModel> Sites;
        public static Dictionary<long, List<IssueModel>> IssueHash;
        public static Dictionary<long, List<ResultModel>> ResultHash;
        public static Dictionary<long, List<WikiModel>> WikiHash;

        public static void Initialize()
        {
            DefinitionAccessor.Initializer.Initialize(
                path: null,
                assemblyVersion: Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Context context = ContextData.Get(
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
                where: Rds.DeptsWhere().TenantId(TenantId));
            Groups = new GroupCollection(
                context: context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                where: Rds.GroupsWhere().TenantId(TenantId));
            Users = new UserCollection(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                where: Rds.UsersWhere().TenantId(TenantId));
            Sites = new SiteCollection(
                context: context,
                where: Rds.SitesWhere().TenantId(TenantId));
            Items = new ItemCollection(
                context: context,
                where: Rds.ItemsWhere().SiteId_In(Sites.Select(o => o.SiteId)));
            IssueHash = new Dictionary<long, List<IssueModel>>();
            ResultHash = new Dictionary<long, List<ResultModel>>();
            WikiHash = new Dictionary<long, List<WikiModel>>();
            Sites.ForEach(siteModel =>
            {
                switch (siteModel.ReferenceType)
                {
                    case "Issues":
                        IssueHash.Add(siteModel.SiteId, new IssueCollection(
                            context: context,
                            ss: SiteSettingsUtilities.Get(context: context, siteId: siteModel.SiteId),
                            where: Rds.IssuesWhere().SiteId_In(Sites.Select(o => o.SiteId))));
                        break;
                    case "Results":
                        ResultHash.Add(siteModel.SiteId, new ResultCollection(
                            context: context,
                            ss: SiteSettingsUtilities.Get(context: context, siteId: siteModel.SiteId),
                            where: Rds.ResultsWhere().SiteId_In(Sites.Select(o => o.SiteId))));
                        break;
                    case "Wikis":
                        WikiHash.Add(siteModel.SiteId, new WikiCollection(
                            context: context,
                            ss: SiteSettingsUtilities.Get(context: context, siteId: siteModel.SiteId),
                            where: Rds.WikisWhere().SiteId_In(Sites.Select(o => o.SiteId))));
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