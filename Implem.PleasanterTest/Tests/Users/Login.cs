using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
namespace Implem.PleasanterTest.Tests.Users
{
    /// <summary>
    /// ログインページのテストです。
    /// </summary>
    [Collection(nameof(Login))]
    public class Login
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Context context,
            string returnUrl,
            string message,
            List<BaseTest> baseTests)
        {
            var results = UserUtilities.HtmlLogin(
                context: context,
                returnUrl: returnUrl,
                message: message);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData => new List<object[]>()
        {
            Allow()
        };

        private static object[] Allow()
        {
            var context = GetContext();
            return new object[]
            {
                context,
                string.Empty,
                string.Empty,
                BaseData.Tests(
                    HtmlData.ExistsOne(selector: "#Users_LoginId"),
                    HtmlData.ExistsOne(selector: "#Users_Password"),
                    HtmlData.ExistsOne(selector: "#Users_RememberMe"),
                    HtmlData.TextContent(
                        selector: "#Login",
                        value: Displays.Login(context: context)))
            };
        }

        private static Context GetContext()
        {
            return ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                httpMethod: "get",
                absolutePath: "/users/login");
        }
    }
}
