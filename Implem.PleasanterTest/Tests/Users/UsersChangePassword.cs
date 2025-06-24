using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Mvc;
using NLog.Targets;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Implem.PleasanterTest.Tests.Users
{
    [Collection(nameof(UsersChangePassword))]
    public class UsersChangePassword
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            UserModel userModel,
            List<BaseTest> baseTests,
            Forms forms)
        {
            var userId = Initializer.Users.Values.FirstOrDefault(o => o.Name == "ユーザ26（パスワード設定用）").UserId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.UsersEdit(userModel.UserId),
                forms: forms);
            var results = Results(
                context: context,
                userId: userId);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }
        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "ChangePassword"),
                        new KeyValue("Users_OldPassword", "ABCDEF"),
                        new KeyValue("Users_ChangedPassword", "ABC123")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"ChangingPasswordComplete\",\"Text\":\"パスワードを変更しました。\",\"Css\":\"alert-success\"}")),
                    userType: UserData.UserTypes.ChagePasswordUser),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests,
                    forms: testPart.Forms);
            }
        }

        private static object[] TestData(
            UserModel userModel,
            List<BaseTest> baseTests,
            Forms forms)
        {
            return new object[]
            {
                userModel,
                baseTests,
                forms,
            };
        }

        private static string Results(
            Context context,
            int userId)
        {
            return UserUtilities.ChangePassword(
                context: context,
                userId: userId);
        }
    }
}
