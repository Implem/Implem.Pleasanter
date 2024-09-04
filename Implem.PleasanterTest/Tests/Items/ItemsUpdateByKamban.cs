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
    public class ItemsUpdateByKamban
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
                routeData: RouteData.ItemsUpdateByKamban(id: id),
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
                new KeyValue("ControlId", "KambanBody"),
                new KeyValue("KambanId", Initializer.Titles.Get("運用者向けマニュアルを作成する").ToString()),
                new KeyValue("Issues_Status", "150"),
                new KeyValue("Issues_ClassC", "サーバ"));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#KambanBody"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "View"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "initRelatingColumnWhenViewChanged"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "title"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Breadcrumb"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Guide"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#CopyToClipboards"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#Aggregations"),
                JsonData.ExistsOne(
                    method: "ReplaceAll",
                    target: "#MainCommandsContainer"),
                JsonData.ExistsOne(
                    method: "SetValue",
                    target: "#EditOnGrid"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setKamban"),
                JsonData.ExistsOne(method: "ClearFormData"),
                JsonData.ExistsOne(method: "Log"),
                JsonData.ExistsOne(
                    method: "Events",
                    target: "on_kamban_load"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
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
            var itemModel = Initializer.ItemIds.Get(context.Id) ?? new ItemModel();
            return itemModel.UpdateByKamban(context: context);
        }
    }
}
