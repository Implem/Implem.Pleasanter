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
    public class ItemsLinkTable
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
                routeData: RouteData.ItemsLinkTable(id: id),
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "AI技術の検証",
                    linkedSitetitle: "顧客マスタ",
                    sortColumnName: "Title")
            };
            foreach (var testPart in testParts)
            {
                var tableId = $"{testPart.ReferenceType}_{testPart.Direction}{testPart.SiteId}";
                var forms = new Forms();
                forms.Add(
                    $"ViewSorters__{testPart.SortColumnName}",
                    testPart.SortType);
                forms.Add(
                    "Direction",
                    testPart.Direction);
                forms.Add(
                    "TableId",
                    tableId);
                forms.Add(
                    "ControlId",
                    tableId);
                yield return TestData(
                    title: testPart.Title,
                    forms: forms,
                    userModel: UserData.Get(userType: testPart.UserType),
                    jsonTests: JsonData.ExistsOne(
                        method: "ReplaceAll",
                        target: $"#{tableId}").ToSingleList());
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
            return itemModel.LinkTable(context: context);
        }

        private class TestPart
        {
            public string Title { get; }
            public string SortColumnName { get; }
            public string SortType { get; }
            public string Direction { get; }
            public string ReferenceType { get; }
            public long SiteId { get; }
            public UserData.UserTypes UserType { get; set; }

            public TestPart(
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
                UserType = userType;
            }
        }
    }
}
