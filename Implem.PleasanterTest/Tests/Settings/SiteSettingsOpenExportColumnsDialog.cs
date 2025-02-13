using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using System.Web.Mvc;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsOpenExportColumnsDialog))]
    public class SiteSettingsOpenExportColumnsDialog
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests, 
            UserModel userModel,
            Dictionary<string, string> sessionData)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms,
                sessionData: sessionData);
            var results = Results(context: context, siteId: siteId);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var sessionData = new Dictionary<string, string>() {
            {
                "Export","[\"{\\\"Id\\\":1,\\\"ColumnName\\\":\\\"ResultId\\\"}\",\"{\\\"Id\\\":2,\\\"ColumnName\\\":\\\"Title\\\"}\",\"{\\\"Id\\\":3,\\\"ColumnName\\\":\\\"Body\\\"}\",\"{\\\"Id\\\":4,\\\"ColumnName\\\":\\\"TitleBody\\\"}\",\"{\\\"Id\\\":5,\\\"ColumnName\\\":\\\"Comments\\\"}\",\"{\\\"Id\\\":6,\\\"ColumnName\\\":\\\"Status\\\"}\",\"{\\\"Id\\\":7,\\\"ColumnName\\\":\\\"Manager\\\"}\",\"{\\\"Id\\\":8,\\\"ColumnName\\\":\\\"Owner\\\"}\",\"{\\\"Id\\\":9,\\\"ColumnName\\\":\\\"Creator\\\"}\",\"{\\\"Id\\\":10,\\\"ColumnName\\\":\\\"Updator\\\"}\",\"{\\\"Id\\\":11,\\\"ColumnName\\\":\\\"UpdatedTime\\\"}\"]"
            }
            };
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サイト設定 - OpenExportColumnsDialog",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "OpenExportColumnsDialog"),
                        new KeyValue("ExportId", "1"),
                        new KeyValue("ExportColumns", "[\"{\\\"Id\\\":1,\\\"ColumnName\\\":\\\"ResultId\\\"}\"]"),
                        new KeyValue("ExportColumnsAll", "[\"{\\\"Id\\\":1,\\\"ColumnName\\\":\\\"ResultId\\\"}\",\"{\\\"Id\\\":2,\\\"ColumnName\\\":\\\"Title\\\"}\",\"{\\\"Id\\\":3,\\\"ColumnName\\\":\\\"Body\\\"}\",\"{\\\"Id\\\":4,\\\"ColumnName\\\":\\\"TitleBody\\\"}\",\"{\\\"Id\\\":5,\\\"ColumnName\\\":\\\"Comments\\\"}\",\"{\\\"Id\\\":6,\\\"ColumnName\\\":\\\"Status\\\"}\",\"{\\\"Id\\\":7,\\\"ColumnName\\\":\\\"Manager\\\"}\",\"{\\\"Id\\\":8,\\\"ColumnName\\\":\\\"Owner\\\"}\",\"{\\\"Id\\\":9,\\\"ColumnName\\\":\\\"Creator\\\"}\",\"{\\\"Id\\\":10,\\\"ColumnName\\\":\\\"Updator\\\"}\",\"{\\\"Id\\\":11,\\\"ColumnName\\\":\\\"UpdatedTime\\\"}\"]")),
                    sessionData: sessionData,
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ExportColumnsDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.Html(
                            target: "#ExportColumnsDialog",
                            selector: "#ExportColumnsForm")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - OpenExportColumnsDialog",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateExportColumn"),
                        new KeyValue("ExportColumnId", "1"),
                        new KeyValue("ExportColumnType", "1"),
                        new KeyValue("EditExport", "[]")),
                    sessionData: sessionData,
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#ExportColumns"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.Privileged),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests,
                    userModel: testPart.UserModel,
                    sessionData: testPart.SessionData);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel,
            Dictionary<string, string> sessionData)
        {
            return new object[]
            {
                title,
                forms,
                baseTests,
                userModel,
                sessionData
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
