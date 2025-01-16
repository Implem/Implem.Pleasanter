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
using Newtonsoft.Json.Linq;

//下記サーバースクリプトのテストを行います。
/*
if(context.SiteId === siteSettings.SiteId('xUnit_SiteSettingsMethod')){
        context.AddMessage('SiteId-Check:OK', 'alert-information');
} else {
        context.AddMessage('SiteId-Check:NG', 'alert-error');
}",
"siteSettings.Sections[0].LabelText = '内容';
if(siteSettings.Sections[0].LabelText === '内容'){
    context.AddMessage('Section-Check:OK', 'alert-information');
} else {
    context.AddMessage('Section-Check:NG', 'alert-error');
}",
"hidden.Add('xUnit-Test','xUnit-Test');",
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptSiteSettingsMethod))]
    public class ServerScriptSiteSettingsMethod
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
                HtmlData.HasInformationMessage(message: "SiteId-Check:OK"),
                HtmlData.HasInformationMessage(message: "Section-Check:OK"),
                HtmlData.TextContains(selector: "#xUnit-Test", value: "xUnit-Test"));

            var testParts = new List<TestPart>()
            {
                new TestPart(title: "xUnit_SiteSettingsMethod", baseTests: hasMessages,userType: UserData.UserTypes.Privileged),
                //new TestPart(title: "xUnit_SiteSettingsMethod", baseTests: BaseData.Tests(HtmlData.TextContains("xUnit-Test", "xUnit-Test")),userType: UserData.UserTypes.Privileged)
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
