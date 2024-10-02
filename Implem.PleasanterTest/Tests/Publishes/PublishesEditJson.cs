using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Publishes
{
    [Collection(nameof(PublishesEditJson))]
    public class PublishesEditJson
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishesEdit(id: id));
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
                new TestPart(title: "サーバのテスト")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Response",
                            target: "id"),
                        JsonData.ExistsOne(
                            method: "Invoke",
                            target: "clearDialogs"),
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#MainContainer"),
                        JsonData.ExistsOne(
                            method: "SetValue",
                            target: "#Id"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.ExistsOne(
                            method: "Invoke",
                            target: "setCurrentIndex"),
                        JsonData.ExistsOne(
                            method: "Invoke",
                            target: "initRelatingColumnEditor"),
                        JsonData.ExistsOne(method: "ClearFormData"),
                        JsonData.ExistsOne(
                            method: "Events",
                            target: "on_editor_load"),
                        JsonData.ExistsOne(method: "Log")));
            }
        }

        private static object[] TestData(
            string title,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.EditorJson(context: context);
        }
    }
}
