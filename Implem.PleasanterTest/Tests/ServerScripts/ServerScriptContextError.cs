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
using Implem.Pleasanter.Libraries.ServerScripts;
using System.ComponentModel.DataAnnotations;

//下記サーバースクリプトのテストを行います。
/*
try{
    context.Error('条件に該当したため、更新をキャンセルしました。');
    } catch (e) {
        context.Log(e.stack);
    }
}
*/

namespace Implem.PleasanterTest.Tests.ServerScript
{
    [Collection(nameof(ServerScriptContextError))]
    public class ServerScriptContextError
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

            Assert.True(Results(context: context));
        }

        public static IEnumerable<object[]> GetData()
        {
            var hasMessages = BaseData.Tests(
                HtmlData.HasInformationMessage(message: "条件に該当したため、更新をキャンセルしました。"));

            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "xUnit_contextError",
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

        private static bool Results(Context context)
        {
            var itemModel = Initializer.ItemIds.Get(context.Id);
            var stringObject = itemModel.Create(context: context);
            if (stringObject.Contains("条件に該当したため、更新をキャンセルしました。") && stringObject.Contains("alert-error")) {
                return true;
            } else {
                return false;
            }
        }
    }
}
