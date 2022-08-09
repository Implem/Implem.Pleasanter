using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    public class ItemsOpenBulkUpdateSelectorDialog
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
                routeData: RouteData.ItemsOpenBulkUpdateSelectorDialog(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "WBS",
                    gridCheckedItems: new List<string>()
                    {
                        "サーバの構築"
                    })
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "OpenBulkUpdateSelectorDialog"),
                        new KeyValue("GridCheckedItems", testPart.GridCheckedItems)),
                    userModel: testPart.UserModel,
                    jsonTests: JsonData.Tests(JsonData.ExistsOne(
                        method: "Html",
                        target: "#BulkUpdateSelectorDialog")));
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

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.OpenBulkUpdateSelectorDialog(context: context);
        }

        private class MyTestPart : TestPart
        {
            public string GridCheckedItems { get; set; }

            public MyTestPart(
                string title,
                List<string> gridCheckedItems,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                GridCheckedItems = gridCheckedItems
                    .Select(o => Initializer.Titles.Get(o))
                    .Join();
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
