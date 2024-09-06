using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using System.Linq;

//下記サーバースクリプトのテストを行います。
/*
try{
    elements.DisplayType('HelpMenuContainer',1);
    elements.DisplayType('DeleteCommand',2);
    elements.DisplayType('Process_1',3);
    elements.LabelText("Process_2", "xUnit_elements.LabelText");
}catch{
    context.Log(ex.stack);
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptelementsDisplayType))]
    public class ServerScriptelementsDisplayType
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsEdit(id: id));

            context.BackgroundServerScript = true; //サーバースクリプトのテスト実施時は必須

            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var style = "display:none;";
            var labelText = "xUnit_elements.LabelText";
            var hasMessages = BaseData.Tests(
                HtmlData.Disabled(selector: "#DeleteCommand"),
                HtmlData.NotExists(selector: "#HelpMenuContainer"),
                HtmlData.Attribute(selector: "#Process_1", name: "style", value: style),
                HtmlData.TextContent(selector: "#Process_2", value: labelText));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "ResultsA_elements",
                    baseTests: hasMessages,
                    userType: UserData.UserTypes.Privileged),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.Editor(context: context);
        }
    }
}
