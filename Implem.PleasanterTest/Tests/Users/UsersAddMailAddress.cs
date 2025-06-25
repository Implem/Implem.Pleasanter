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
    [Collection(nameof(UsersAddMailAddress))]
    public class UsersAddMailAddress
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
            context.SessionData.Add("MailAddresses", $"[\"Tenant{userModel.TenantId}_User26@example.com\"]");
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
            //テナントId取得
            var userModel = UserData.Get(UserData.UserTypes.ChangePasswordUser);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("MailAddress", $"Tenant{userModel.TenantId}_User26@example.com"),
                        new KeyValue("VerUp", "true")),
                    baseTests: BaseData.Tests(
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"AlreadyAdded\",\"Text\":\"既に追加されています。\",\"Css\":\"alert-error\"}")),
                    userType: UserData.UserTypes.ChangePasswordUser),
                new TestPart(
                    forms: FormsUtilities.Get(
                        new KeyValue("MailAddress", $"AddMailAddress_User26@example.com"),
                        new KeyValue("VerUp", "true")),
                    baseTests: BaseData.Tests(
                        JsonData.TextCheckOrder(
                            method: "Html",
                            target: "#MailAddresses",
                            wordArray: new string[]
                            {
                                $"<li class=\"ui-widget-content\">Tenant{userModel.TenantId}_User26@example.com</li>",
                                "<li class=\"ui-widget-content\">AddMailAddress_User26@example.com</li>"
                            }),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged")),
                    userType: UserData.UserTypes.ChangePasswordUser),
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
            return UserUtilities.AddMailAddresses(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                userId: userId);
        }
    }
}
