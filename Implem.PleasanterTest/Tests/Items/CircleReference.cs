using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
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
            // 操作するテーブルのIDをDB検索で取得
            // (サイトパッケージからテストデータを追加すると、Initializerのフィールドに登録されないので考慮してテストを記述)
            var id = Initializer.GetSiteIdByPackageTitle(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsEdit(id: id));
            // テーブル設定画面の初期表示確認テストを実施
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            // サイトパッケージから追加したテストデータ(フォルダ)に標準的なアクセス権を付与
            Initializer.InsertCommonPermissions("循環");
            // テスト対象はフォルダ内のテーブル3件
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "循環参照1", userType: UserData.UserTypes.TenantManager1),
                new TestPart(title: "循環参照2", userType: UserData.UserTypes.TenantManager1),
                new TestPart(title: "循環参照3", userType: UserData.UserTypes.TenantManager1)
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
            var itemModel = new ItemModel(
                context: context,
                referenceId: context.Id);
            return itemModel.Editor(context: context);
        }
    }
}
