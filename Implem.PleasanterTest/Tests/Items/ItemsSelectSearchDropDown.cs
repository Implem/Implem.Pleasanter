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
    public class ItemsSelectSearchDropDown
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
                routeData: RouteData.ItemsSelectSearchDropDown(id: id),
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
                new KeyValue("DropDownSearchReferenceId", Initializer.Titles.Get("顧客マスタ").ToString()),
                new KeyValue("DropDownSearchSelectedValues", $"[{Initializer.Titles.Get("働き方改革推進団体")}]"),
                new KeyValue("DropDownSearchTarget", "Issues_ClassA"),
                new KeyValue("DropDownSearchMultiple", "false"),
                new KeyValue("DropDownSearchResultsOffset", "-1"),
                new KeyValue("DropDownSearchParentClass", string.Empty),
                new KeyValue("DropDownSearchParentDataId", string.Empty),
                new KeyValue("DropDownSearchResults", $"[{Initializer.Titles.Get("株式会社プリザンター")}]"),
                new KeyValue("DropDownSearchText", string.Empty));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(
                    method: "CloseDialog",
                    target: "#DropDownSearchDialog"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "[id=\"Issues_ClassA\"]"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setDropDownSearch"),
                JsonData.ExistsOne(
                    method: "Trigger",
                    target: "#Issues_ClassA"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "業務改善コンサルティング",
                    forms: forms,
                    baseTests: baseTests)
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
            return itemModel.SelectSearchDropDown(context: context);
        }
    }
}
