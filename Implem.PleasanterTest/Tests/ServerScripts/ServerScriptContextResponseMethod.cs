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
    context.AddResponse('ReplaceAll','#Results_TitleField','<div class="field-normal" id="#ServerScriptResponseCollection" style="color:red">PleasanterTitle</div>');
    context.AddResponse('Set','NumA',123);
    context.ResponseSet('NumB',456)
for (let key of context.Forms.Keys){
context.Log(key);
    context.Log(key + ': ' + context.Forms.Item.get(key));
}
    if(context.Forms.Item.get('NumA') == 123 && context.Forms.Item.get('NumB') == 456){
        context.AddMessage("OK",'alert-information');
    } else {
        context.AddMessage("NG",'alert-error');
    }
} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptContextResponseMethod
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
                routeData: RouteData.ItemsIndex(id: siteId));

            context.BackgroundServerScript = true; //サーバースクリプトのテスト実施時は必須

            var results = Results(context: context);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            string TitleValue = "[{\"Method\":\"ReplaceAll\",\"Target\":\"#Results_TitleField\",\"Value\":\"<div class=\\\"field-normal\\\" id=\\\"#ServerScriptResponseCollection\\\" style=\\\"color:red\\\">PleasanterTitle</div>\"},{\"Method\":\"Set\",\"Target\":\"NumA\",\"Value\":123},{\"Method\":\"Set\",\"Target\":\"NumB\",\"Value\":456}]"
;
            var hasMessages = BaseData.Tests(
                HtmlData.Attribute(name: "value", selector: "#ServerScriptResponseCollection", value: TitleValue));


            var testParts = new List<TestPart>()
            {
                new TestPart(title: "xUnit_contextResponseMethod", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),
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
            return itemModel.New(context: context);
        }
    }
}
