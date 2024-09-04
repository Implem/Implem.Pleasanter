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
    public class ItemsSearchDropDown
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
                routeData: RouteData.ItemsSearchDropDown(id: id),
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
                new KeyValue("ControlId", "DropDownSearchTarget"),
                new KeyValue("DropDownSearchReferenceId", Initializer.Titles.Get("顧客マスタ").ToString()),
                new KeyValue("DropDownSearchSelectedValues", $"[{Initializer.Titles.Get("働き方改革推進団体")}]"),
                new KeyValue("DropDownSearchTarget", "Issues_ClassA"),
                new KeyValue("DropDownSearchMultiple", "false"),
                new KeyValue("DropDownSearchResultsOffset", "0"),
                new KeyValue("DropDownSearchParentClass", string.Empty),
                new KeyValue("DropDownSearchParentDataId", string.Empty));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "業務改善コンサルティング",
                    forms: forms,
                    baseTests: BaseData.Tests(JsonData.ExistsOne(
                        method: "Html",
                        target: "#DropDownSearchDialogBody")))
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
            return itemModel.SearchDropDown(context: context);
        }
    }
}
