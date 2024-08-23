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
    context.AddResponse('ReplaceAll','#Results_TitleField','<div class="field-normal" style="color:red">PleasanterTitle</div>');
    context.AddResponse('Set','NumA',123);
    if(model.Num === 123){
        context.AddMessage("OK",'alert-information');
    } else {
        context.AddMessage("NG",'alert-error');
    }
} catch (e) {
    context.AddMessage(e.stack, 'alert-error');
}*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    public class ServerScriptContextAddresponse
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
            string TitleValue = "PleasanterTitle";
            var hasMessages = BaseData.Tests(
                HtmlData.Attribute(name: "value", selector: "#Results_TitleField", value: TitleValue),
                HtmlData.HasInformationMessage(message: "OK"));


            var testParts = new List<TestPart>()
            {
                new TestPart(title: "xUnit_contextAddresponse", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),
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
