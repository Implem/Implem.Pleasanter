using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Items
{
    public class ItemsOpenExportSelectorDialog
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsOpenExportSelectorDialog(id: id),
                httpMethod: "POST",
                forms: forms);
            var json = GetJson(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<string>()
            {
                "WBS"
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart,
                    forms: new Forms()
                    {
                        {
                            "ControlId",
                            "OpenExportSelectorDialog"
                        }
                    },
                    userModel: UserData.Get(userType: UserData.UserTypes.TenantManager1),
                    jsonTests: JsonData.ExistsOne(
                        method: "Html",
                        target: $"#ExportSelectorDialog").ToSingleList());
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                jsonTests
            };
        }

        private static string GetJson(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.OpenExportSelectorDialog(context: context);
        }
    }
}
