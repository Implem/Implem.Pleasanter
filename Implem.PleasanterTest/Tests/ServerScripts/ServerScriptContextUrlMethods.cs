using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
using Org.BouncyCastle.Asn1.X509;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Xunit.Sdk;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

//下記サーバースクリプトのテストを行います。
/*
if((context.Url.match(/name/g) || []).length === 0){
    let myUrl = context.Url;
    context.Redirect(myUrl + "?name=" + context.UserData.MyData);
}
let urlName = context.QueryStrings.Data('name');
if(urlName === "HAYATO"){
    context.AddMessage("OK",'alert-information');
} else {
    context.AddMessage("NG", 'alert-error');
}
context.UserData.MyData = 'HAYATO';
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptContextUrlMethods))]
    public class ServerScriptContextUrlMethods
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
                redirectUrl: "http://localhost/items/591618/index?name=HAYATO");
            context.BackgroundServerScript = true;
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));

        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "OK"));
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "xUnit_contextUrlMethod", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),

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
