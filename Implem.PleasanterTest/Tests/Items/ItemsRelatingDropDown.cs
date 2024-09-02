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
    public class ItemsRelatingDropDown
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
                routeData: RouteData.ItemsRelatingDropDown(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var forms = FormsUtilities.Get(
                new KeyValue("ControlId", "TriggerRelatingColumns_Editor"),
                new KeyValue("RelatingDropDownControlId", "Results_ClassA"),
                new KeyValue("RelatingDropDownSelected", "[\"\"]"),
                new KeyValue("RelatingDropDownParentClass", "ClassA"),
                new KeyValue("RelatingDropDownParentDataId", $"[\"{Initializer.Titles.Get("株式会社プリザンター")}\"]"));
            var baseTests = BaseData.Tests(
                JsonData.Value(
                    method: "Html",
                    target: "#Results_ClassA",
                    value: $"<option value=\"\" selected=\"selected\"></option><option value=\"{Initializer.Titles.Get("グループウェアのクラウド環境への移行")}\">グループウェアのクラウド環境への移行</option>"),
                JsonData.ExistsOne(
                    method: "Invoke",
                    target: "callbackRelatingColumn"),
                JsonData.ExistsOne(
                    method: "ClearFormData",
                    target: "#Results_ClassA"));
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "X社ソフトウェア",
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
            return itemModel.RelatingDropDown(context: context);
        }
    }
}
