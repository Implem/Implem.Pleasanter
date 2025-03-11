using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsSummaries))]
    public class SiteSettingsSummaries
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context, siteId: siteId);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var summarySiteId = Initializer.Sites.Get("商談").SiteId;
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","NewSummary")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#SummaryDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#SummaryDialog","#SummaryForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","AddSummary"),
                        new KeyValue("SummarySiteId", summarySiteId.ToString()),
                        new KeyValue("SummaryDestinationColumn", "WorkValue"),
                        new KeyValue("SummarySetZeroWhenOutOfCondition", "false"),
                        new KeyValue("SummaryLinkColumn", "ClassA"),
                        new KeyValue("SummaryType", "Count")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditSummary"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ReplaceAll("#EditSummary","#EditSummaryWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","EditSummary"),
                        new KeyValue("SummaryId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#SummaryDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#SummaryDialog","#SummaryForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","UpdateSummary"),
                        new KeyValue("SummaryId","1"),
                        new KeyValue("SummarySiteId", summarySiteId.ToString()),
                        new KeyValue("SummaryDestinationColumn", "WorkValue"),
                        new KeyValue("SummarySetZeroWhenOutOfCondition", "false"),
                        new KeyValue("SummaryLinkColumn", "ClassA"),
                        new KeyValue("SummaryType", "Sum")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditSummary"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditSummary","#EditSummaryWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","CopySummaries"),
                        new KeyValue("EditSummary","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditSummary"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ReplaceAll("#EditSummary","#EditSummaryWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","DeleteSummaries"),
                        new KeyValue("EditSummary","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditSummary"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ReplaceAll("#EditSummary","#EditSummaryWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","MoveUpSummaries"),
                        new KeyValue("EditSummary","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditSummary"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditSummary","#EditSummaryWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "仕入",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","MoveDownSummaries"),
                        new KeyValue("EditSummary","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditSummary"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditSummary","#EditSummaryWrap")),
                    userType: UserData.UserTypes.Privileged)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                forms,
                baseTests,
                userModel
            };
        }

        private static string Results(Context context, long siteId)
        {
            return SiteUtilities.SetSiteSettings(
                context: context,
                siteId: siteId);
        }
    }
}
