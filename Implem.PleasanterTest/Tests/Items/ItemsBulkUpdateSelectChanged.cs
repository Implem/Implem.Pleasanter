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
    public class ItemsBulkUpdateSelectChanged
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
            var json = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                json: json,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var gridCheckedItems = new List<string>()
            {
                "サーバの構築"
            }
                .Select(o => Initializer.Titles.Get(o))
                .Join();
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "WBS",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "OpenBulkUpdateSelectorDialog"),
                        new KeyValue("GridCheckedItems", gridCheckedItems),
                        new KeyValue("BulkUpdateColumnName", "ClassA")),
                    bulkUpdateColumnName: "ClassA",
                    jsonTests: JsonData.Tests(JsonData.ExistsOne(
                        method: "Html",
                        target: "#BulkUpdateSelectedField")))
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
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
            return itemModel.BulkUpdateSelectChanged(context: context);
        }

        private class MyTestPart : TestPart
        {
            public string GridCheckedItems { get; set; }
            public string BulkUpdateColumnName { get; set; }

            public MyTestPart(
                string title,
                Forms forms,
                string bulkUpdateColumnName,
                List<JsonTest> jsonTests,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                Forms = forms;
                BulkUpdateColumnName = bulkUpdateColumnName;
                JsonTests = jsonTests;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
