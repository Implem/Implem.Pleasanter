using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;
using Initializer = Implem.PleasanterTest.Utilities.Initializer;

namespace Implem.PleasanterTest.Tests.Items
{
    [Collection(nameof(CircleReference))]
    public class CircleReference
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsEdit(id: id));
            // 画面から取得したサイトパッケージで登録したサイト、テーブルのアクセス権を付与
            InsertPermissions(context: context);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "循環参照1", userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                title: testPart.Title,
                userModel: testPart.UserModel,
                baseTests: BaseData.Tests(HtmlData.ExistsOne(selector: "#Editor")));
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Editor(context: context);
        }

        private static void InsertPermissions(Context context)
        {
            // 共通のInitializerにおいてサイトパッケージから追加したサイトにアクセス権を付与。
            // ※サイトパッケージから追加したテストデータでは、このように、アクセス権はテストの直前に付与します。
            // →テナント管理者に511のアクセス権を付与、全組織に31のアクセス権を付与。
            // →標準的なデモ用のサイト(Defで管理しているもの)と同様とする。
            // ※フォルダ内の個別テーブルはサイトパッケージからインポートした時点でアクセス権継承設定があるものとする。
            var demoDefinition = Def.DemoDefinitionCollection;
            var tenantId = Initializer.TenantId;
            var siteId = Repository.ExecuteScalar_long(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SiteId(),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .Title("循環"),
                        orderBy: Rds.SitesOrderBy().SiteId(),
                        top: 1));
            // テナント管理者にアクセス権を付与
            var privilegeUserId = Initializer.Users.Values
                .Where(user => user.Name == "テナント管理者")
                .Select(user => user.UserId)
                .FirstOrDefault();
            Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceId(siteId)
                            .DeptId(0)
                            .GroupId(0)
                            .UserId(privilegeUserId)
                            .PermissionType(511)));
            // 各組織にアクセス権を付与
            var depts = Initializer.Depts;
            var items = Initializer.ItemIds;
        }
    }
}
