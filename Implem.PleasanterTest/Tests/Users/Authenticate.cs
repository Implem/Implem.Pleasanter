using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    /// <summary>
    /// ログイン操作のテストです。
    /// </summary>
    [Collection(nameof(Authenticate))]
    public class Authenticate
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            Context context,
            string returnUrl,
            List<BaseTest> baseTests)
        {
            var results = Authentications.SignIn(
                context: context,
                returnUrl: returnUrl,
                noHttpContext: true);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData => new List<object[]>()
        {
            Allow(),
            LowerCaseLoginId(),
            DifferentPassword1(),
            DifferentPassword2(),
            DifferentPassword3(),
            BlankLoginId(),
            BlankPassword()
        };

        private static object[] Allow()
        {
            var context = GetContext(
                loginId: $"Tenant{Initializer.TenantId}_User1",
                password: "ABCDEF");
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                AllowMessage(context, returnUrl)
            };
        }

        private static object[] LowerCaseLoginId()
        {
            var context = GetContext(
                loginId: $"tenant{Initializer.TenantId}_user1",
                password: "ABCDEF");
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                AllowMessage(context, returnUrl)
            };
        }

        private static object[] DifferentPassword1()
        {
            var context = GetContext(
                loginId: $"Tenant{Initializer.TenantId}_User1",
                password: "ABCDEf");
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                FailResponse(context)
            };
        }

        private static object[] DifferentPassword2()
        {
            var context = GetContext(
                loginId: $"Tenant{Initializer.TenantId}_User1",
                password: "ABCDEF ");
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                FailResponse(context)
            };
        }

        private static object[] DifferentPassword3()
        {
            var context = GetContext(
                loginId: $"Tenant{Initializer.TenantId}_User1",
                password: "111111");
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                FailResponse(context)
            };
        }

        private static object[] BlankLoginId()
        {
            var context = GetContext(
                loginId: string.Empty,
                password: "ABCDEF");
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                FailResponse(context)
            };
        }

        private static object[] BlankPassword()
        {
            var context = GetContext(
                loginId: $"Tenant{Initializer.TenantId}_User1",
                password: string.Empty);
            var returnUrl = string.Empty;
            return new object[]
            {
                context,
                returnUrl,
                FailResponse(context)
            };
        }

        private static Context GetContext(string loginId, string password)
        {
            return ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                httpMethod: "post",
                absolutePath: "/users/authenticate",
                forms: FormsUtilities.Get(
                    new KeyValue("Users_LoginId", loginId),
                    new KeyValue("Users_Password", password)));
        }

        private static List<BaseTest> AllowMessage(Context context, string returnUrl)
        {
            return BaseData.Tests(
                JsonData.Value(
                    method: "Message",
                    target: "#LoginMessage",
                    value: Messages.LoginIn(context: context).ToJson()),
                JsonData.Value(
                    method: "Href",
                    value: returnUrl.IsNullOrEmpty()
                        ? Locations.Top(context: context)
                        : returnUrl));
        }

        private static List<BaseTest> FailResponse(Context context)
        {
            return BaseData.Tests(
                JsonData.Value(
                    method: "Message",
                    target: "#LoginMessage",
                    value: Messages.Authentication(context: context).ToJson()),
                JsonData.ExistsOne(
                    method: "Focus",
                    target: "#Password"));
        }
    }
}
