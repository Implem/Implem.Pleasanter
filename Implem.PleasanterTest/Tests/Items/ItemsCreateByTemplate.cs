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
    public class ItemsPreviewTemplate
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
                routeData: RouteData.ItemsPreviewTemplate(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "SalesTemplates"),
                new KeyValue("SalesTemplates", "[\"Template18\"]"));
            var jsonTests = JsonData.Tests(
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#SalesTemplatesViewer .description"),
                JsonData.ExistsOne(
                    method: "Html",
                    target: "#SalesTemplatesViewer .viewer"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setTemplateViewer"),
                JsonData.ExistsOne(
                    method: "Toggle",
                    target: "#SalesTemplatesViewer .viewer"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: string.Empty,
                    forms: forms,
                    jsonTests: jsonTests),
                new TestPart(
                    title: "プロジェクト管理の例",
                    forms: forms,
                    jsonTests: jsonTests,
                    userType: UserData.UserTypes.TenantManager1),
                new TestPart(
                    title: "商談管理の例",
                    forms: forms,
                    jsonTests: jsonTests,
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    jsonTests: testPart.JsonTests);
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

        private static string Results(Context context)
        {
            return SiteUtilities.PreviewTemplate(context: context);
        }
    }
}
