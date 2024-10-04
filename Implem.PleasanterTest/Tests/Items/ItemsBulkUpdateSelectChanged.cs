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
    [Collection(nameof(ItemsBulkUpdateSelectChanged))]
    public class ItemsBulkUpdateSelectChanged
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
                routeData: RouteData.ItemsOpenBulkUpdateSelectorDialog(id: id),
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
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#BulkUpdateSelectedField")))
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
                List<BaseTest> baseTests,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                Forms = forms;
                BulkUpdateColumnName = bulkUpdateColumnName;
                BaseTests = baseTests;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
