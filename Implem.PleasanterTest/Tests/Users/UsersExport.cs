using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
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
    [Collection(nameof(UsersExport))]
    public class UsersExport
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
                routeData: RouteData.UsersExport(userModel.UserId),
                forms: forms);
            var results = Results(
                context: context,
                userId: userId);
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
                        new KeyValue("ExportId", "ChangePassword"),
                        new KeyValue("ExportEncoding", "Shift-JIS"),
                        new KeyValue("GridCheckedItems", "1"),
                        new KeyValue("ExportCommentsJsonFormat", "false"),
                        new KeyValue("GridCheckAll", "undefined"),
                        new KeyValue("GridUnCheckedItems", "undefined")),
                    baseTests: BaseData.Tests(FileData.Exists()),
                    userType: UserData.UserTypes.TenantManager1),
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

        private static ResponseFile Results(
            Context context,
            int userId)
        {
            return UserUtilities.Export(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
        }
    }
}