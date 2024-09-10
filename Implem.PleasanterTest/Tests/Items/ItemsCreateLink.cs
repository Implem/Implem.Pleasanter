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
    [Collection(nameof(ItemsCreateLink))]
    public class ItemsCreateLink
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
                routeData: RouteData.ItemsCreateLink(id: id),
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
                new KeyValue("SiteId", Initializer.Titles.Get("CreateLinkリンク元").ToString()),
                new KeyValue("DestinationId", Initializer.Titles.Get("CreateLinkリンク先").ToString()),
                new KeyValue("LinkColumn", "ClassA"),
                new KeyValue("LinkColumnLabelText", "CreateLinkリンク項目"));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(method: "CloseDialog"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#SiteMenu"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setSiteMenu"),
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"LinkCreated\",\"Text\":\"リンクを作成しました。\",\"Css\":\"alert-success\"}"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "プロジェクト管理の例",
                    forms: forms,
                    baseTests: baseTests,
                    userType: UserData.UserTypes.TenantManager1)
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
            return SiteUtilities.CreateLink(
                context: context,
                id: context.Id);
        }
    }
}
