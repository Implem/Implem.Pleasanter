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
    [Collection(nameof(SiteSettingsFormula))]
    public class SiteSettingsFormula
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","NewFormula")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#FormulaDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#FormulaDialog","#FormulaForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","AddFormula"),
                        new KeyValue("Formula", "NumB + NumC"),
                        new KeyValue("FormulaTarget", "NumA"),
                        new KeyValue("FormulaCalculationMethod", "Default"),
                        new KeyValue("NotUseDisplayName", "false"),
                        new KeyValue("IsDisplayError", "false"),
                        new KeyValue("FormulaOutOfCondition", "")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditFormula"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ReplaceAll("#EditFormula","#EditFormulaWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","EditFormula"),
                        new KeyValue("FormulaId","1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#FormulaDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#FormulaDialog","#FormulaForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","UpdateFormula"),
                        new KeyValue("FormulaId","1"),
                        new KeyValue("Formula", "NumB + NumC + 1"),
                        new KeyValue("FormulaTarget", "NumA"),
                        new KeyValue("FormulaCalculationMethod", "Default"),
                        new KeyValue("NotUseDisplayName", "false"),
                        new KeyValue("IsDisplayError", "false"),
                        new KeyValue("FormulaOutOfCondition", "")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditFormula"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditFormula","#EditFormulaWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","CopyFormulas"),
                        new KeyValue("EditFormula","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditFormula"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ReplaceAll("#EditFormula","#EditFormulaWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","DeleteFormulas"),
                        new KeyValue("EditFormula","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditFormula"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ReplaceAll("#EditFormula","#EditFormulaWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","MoveUpFormulas"),
                        new KeyValue("EditFormula","[\"2\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditFormula"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditFormula","#EditFormulaWrap")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "商談",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId","MoveDownFormulas"),
                        new KeyValue("EditFormula","[\"1\"]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#EditFormula"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html("#EditFormula","#EditFormulaWrap")),
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
