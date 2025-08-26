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
    [Collection(nameof(ItemsLinkTable))]
    public class ItemsLinkTable
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
                routeData: RouteData.ItemsLinkTable(id: id),
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
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "AI技術の検証",
                    linkedSitetitle: "顧客マスタ",
                    sortColumnName: "Title")
            };
            foreach (var testPart in testParts)
            {
                var tableId = $"{testPart.ReferenceType}_{testPart.Direction}{testPart.SiteId}";
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue($"ViewSorters__{testPart.SortColumnName}", testPart.SortType),
                        new KeyValue("Direction", testPart.Direction),
                        new KeyValue("TableId", tableId),
                        new KeyValue("ControlId", tableId)),
                    userModel: testPart.UserModel,
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: $"#{tableId}Wrap")));
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
            return itemModel.LinkTable(context: context);
        }

        private class MyTestPart : TestPart
        {
            public string SortColumnName { get; }
            public string SortType { get; }
            public string Direction { get; }
            public string ReferenceType { get; }
            public long SiteId { get; }

            public MyTestPart(
                string title,
                string linkedSitetitle,
                string sortColumnName,
                string sortType = "asc",
                string direction = "Destination",
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                var siteModel = Initializer.Sites.Get(linkedSitetitle);
                Title = title;
                SortColumnName = sortColumnName;
                SortType = sortType;
                Direction = direction;
                ReferenceType = siteModel.ReferenceType;
                SiteId = siteModel.SiteId;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
