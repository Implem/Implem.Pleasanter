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
    public class PublishesSelectSearchDropDown
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishesSelectSearchDropDown(id: id),
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
            var forms = FormsUtilities.Get(
                new KeyValue("DropDownSearchReferenceId", Initializer.Titles.Get("商談").ToString()),
                new KeyValue("DropDownSearchSelectedValues", "[]"),
                new KeyValue("DropDownSearchTarget", "ViewFilters__ClassB"),
                new KeyValue("DropDownSearchMultiple", "true"),
                new KeyValue("DropDownSearchResultsOffset", "-1"),
                new KeyValue("DropDownSearchParentClass", string.Empty),
                new KeyValue("DropDownSearchParentDataId", string.Empty),
                new KeyValue("DropDownSearchResults", "[\"システム開発\"]"),
                new KeyValue("DropDownSearchResultsAll", "[\"システム開発\"]"),
                new KeyValue("DropDownSearchText", string.Empty));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(
                    method: "CloseDialog",
                    target: "#DropDownSearchDialog"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "[id=\"ViewFilters__ClassB\"]"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setDropDownSearch"),
                JsonData.ExistsOne(
                    method: "Trigger",
                    target: "#ViewFilters__ClassB"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "商談",
                    forms: forms,
                    baseTests: baseTests)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.SelectSearchDropDown(context: context);
        }
    }
}
