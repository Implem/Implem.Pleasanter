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
    public class setTemplate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsTemplates(id: id),
                httpMethod: "POST");
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var jsonTests = JsonData.Tests(
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#SiteMenu"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#MainCommandsContainer"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setTemplate"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: string.Empty,
                    jsonTests: jsonTests),
                new TestPart(
                    title: "プロジェクト管理の例",
                    jsonTests: jsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談管理の例",
                    jsonTests: jsonTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                userModel,
                jsonTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.Templates(context: context);
        }
    }
}
