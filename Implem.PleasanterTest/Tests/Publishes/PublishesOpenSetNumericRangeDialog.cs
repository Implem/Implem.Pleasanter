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
    [Collection(nameof(PublishesOpenSetNumericRangeDialog))]
    public class PublishesOpenSetNumericRangeDialog
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
                routeData: RouteData.PublishesOpenSetNumericRangeDialog(id: id),
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ViewFilters__NumA_NumericRange"),
                        new KeyValue("ViewFilters__NumA_NumericRange", string.Empty)),
                    baseTests: BaseData.Tests(JsonData.ExistsOne(
                        method: "Html",
                        target: "#SetNumericRangeDialog")))
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
            return itemModel.OpenSetNumericRangeDialog(context: context);
        }
    }
}
