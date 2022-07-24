using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;
namespace Implem.PleasanterTest.Users
{
    /// <summary>
    /// ログインページのテストです。
    /// </summary>
    public class Login
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Context context,
            string returnUrl,
            string message,
            List<HtmlTest> htmlTests)
        {
            var html = UserUtilities.HtmlLogin(
                context: context,
                returnUrl: returnUrl,
                message: message);
            Assert.True(Compare.Html(
                context: context,
                html: html,
                htmlTests: htmlTests));
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
                new List<HtmlTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = "#Users_LoginId"
                    },
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = "#Users_Password"
                    },
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = "#Users_RememberMe"
                    },
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.TextContent,
                        Selector = "#Login",
                        Value = Displays.Login(context: context)
                    }
                }
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
