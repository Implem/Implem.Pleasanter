using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsMoveSiteMenu
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsMoveSiteMenu(id: id),
                httpMethod: "POST",
                forms: forms);
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
                // フォルダ内からトップへの移動
                new TestPart(
                    title: "プロジェクト管理の例",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveSiteMenu"),
                        new KeyValue("SiteId", Initializer.Sites.Get("サイト移動のテスト").ToString()),
                        new KeyValue("DestinationId", "0")),
                    baseTests: BaseData.Tests(TextData.Equals(value: "[]")),
                    userType: UserData.UserTypes.TenantManager1),
                // トップからフォルダ内への移動
                new TestPart(
                    title: string.Empty,
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "MoveSiteMenu"),
                        new KeyValue("SiteId", Initializer.Titles.Get("サイト移動のテスト").ToString()),
                        new KeyValue("DestinationId", Initializer.Titles.Get("プロジェクト管理の例").ToString())),
                    baseTests: BaseData.Tests(JsonData.ExistsOne(
                        method: "ReplaceAll",
                        target: $"[data-value=\"{Initializer.Titles.Get("プロジェクト管理の例")}\"]")),
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return SiteUtilities.MoveSiteMenu(
                context: context,
                id: itemModel.ReferenceId);
        }
    }
}
