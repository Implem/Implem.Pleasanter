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
    public class ItemsMove
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
                routeData: RouteData.ItemsMove(id: id),
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
                new KeyValue("MoveTargets", Initializer.Titles.Get("WBS").ToString()));
            var baseTests = BaseData.Tests(
                JsonData.ExistsOne(method: "Log"),
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
                    method: "SetValue",
                    target: "#SwitchTargets"),
                JsonData.ExistsOne(
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setCurrentIndex"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "initRelatingColumnEditor"),
                JsonData.ExistsOne(method: "Message"),
                JsonData.ExistsOne(method: "ClearFormData"),
                JsonData.ExistsOne(
                    method: "Events",
                    target: "on_editor_load"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "RecordToMove1",
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
            return itemModel.Move(context: context);
        }
    }
}
