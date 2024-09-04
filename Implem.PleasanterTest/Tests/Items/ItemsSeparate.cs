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
    public class ItemsSeparate
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
                routeData: RouteData.ItemsSeparate(id: id),
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
                new KeyValue("SeparateNumber", "3"),
                new KeyValue("SourceWorkValue", "20"),
                new KeyValue("SeparateWorkValue_2", "5"),
                new KeyValue("SeparateWorkValue_3", "15"),
                new KeyValue("SeparateTitle_1", "稼動レポートの作成-1"),
                new KeyValue("SeparateTitle_2", "稼動レポートの作成-2"),
                new KeyValue("SeparateTitle_3", "稼動レポートの作成-3"),
                new KeyValue("SeparateCopyWithComments", "true"));
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
                    method: "SetMemory",
                    target: "formChanged"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "setCurrentIndex"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "initRelatingColumnEditor"),
                JsonData.Value(
                    method: "Message",
                    value: "{\"Id\":\"Separated\",\"Text\":\"分割しました。\",\"Css\":\"alert-success\"}"),
                JsonData.ExistsOne(method: "ClearFormData"),
                JsonData.ExistsOne(
                    method: "Events",
                    target: "on_editor_load"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "稼動レポートの作成",
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
            return itemModel.Separate(context: context);
        }
    }
}
