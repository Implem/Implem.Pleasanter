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
    [Collection(nameof(ItemsAnalyJson))]
    public class ItemsAnalyJson
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsAnaly(id: siteId),
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
            var baseKeyValues = new List<KeyValue>
            {
                 new KeyValue("AnalyPartTimePeriodValue", "1"),
                 new KeyValue("AnalyPartAggregationTarget", ""),
                 new KeyValue("ControlId", "AddAnalyPart"),
                 new KeyValue("AnalyPartId", "0"),
                 new KeyValue("AnalyPartGroupBy", "Status"),
                 new KeyValue("AnalyPartTimePeriod", "DaysAgoNoArgs")
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    forms: FormsUtilities.Get(
                         baseKeyValues.Concat(new[] {
                            new KeyValue("AnalyPartAggregationType", "Count")
                         }).ToArray()
                     )),
                new TestPart(
                    title: "WBS",
                    forms: FormsUtilities.Get(
                         baseKeyValues.Concat(new[] {
                            new KeyValue("AnalyPartAggregationType", "Total")
                         }).ToArray()
                     )),
                new TestPart(
                    title: "WBS",
                    forms: FormsUtilities.Get(
                         baseKeyValues.Concat(new[] {
                            new KeyValue("AnalyPartAggregationType", "Average")
                         }).ToArray()
                     )),
                new TestPart(
                    title: "WBS",
                    forms: FormsUtilities.Get(
                         baseKeyValues.Concat(new[] {
                            new KeyValue("AnalyPartAggregationType", "Min")
                         }).ToArray()
                     )),
                new TestPart(
                    title: "WBS",
                    forms: FormsUtilities.Get(
                         baseKeyValues.Concat(new[] {
                            new KeyValue("AnalyPartAggregationType", "Max")
                         }).ToArray()
                     )),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: BaseData.Tests(
                        JsonData.Html(
                            target: "#ViewModeContainer",
                            selector: "#AnalyJson")));
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
            return itemModel.AnalyJson(context: context);
        }
    }
}
