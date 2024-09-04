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
using Newtonsoft.Json.Linq;

//下記サーバースクリプトのテストを行います。
/*
try{
    if(context.Forms.Item.get("ClassA") === "context.Formテストです。"){
        context.AddMessage("OK",'alert-information');
    } else {
        context.AddMessage("NG",'alert-error');
    }
    if(context.Forms.ControlId() === "index"){
        context.Log("OK");
    } else {
        context.Log("NG");
    }
} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptContextMethods
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsIndex(id: siteId),
                forms: new Forms() { { "ClassA", "context.Formテストです。" }, { "ControlId", "index" } });

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
            string newLineEscaped = System.Environment.NewLine == "\r\n" ? "\\r\\n" : "\\n";
            string logValue = $"{{\"Log\":\"OK{newLineEscaped}\"}}";
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "OK"),
                HtmlData.Attribute(name: "value", selector: "#Log", value: logValue));


            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "xUnit_contextMethods",
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
            return itemModel.Index(context: context);
        }
    }
}
