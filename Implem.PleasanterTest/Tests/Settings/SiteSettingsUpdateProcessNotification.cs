﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsUpdateProcessNotification))]
    public class SiteSettingsUpdateProcessNotification
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context, siteId: siteId);
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
                    title: "サイト設定 - UpdateProcessNotification",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "UpdateProcessNotification"),
                        new KeyValue("ProcessNotificationIdTemp", "1"),
                        new KeyValue("ProcessNotificationsTemp", "[{\"Type\":1,\"Subject\":\"test\",\"Address\":\"sample@sample.com\",\"MethodType\":1,\"MediaType\":\"application/json\",\"Body\":\"test\",\"BeforeCondition\":0,\"AfterCondition\":0,\"Expression\":0,\"Id\":1}]"),
                        new KeyValue("ProcessNotificationBody", "内容 - xUnit テスト用"),
                        new KeyValue("ProcessNotificationSubject", "件名 - xUnit テスト用"),
                        new KeyValue("ProcessNotificationAddress", "sample@test.com")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditProcessNotification"),
                        JsonData.ExistsOne(
                            method: "SetValue",
                            target: "#ProcessNotifications")),
                    userType: UserData.UserTypes.Privileged),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    baseTests: testPart.BaseTests,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                forms,
                baseTests,
                userModel
            };
        }

        private static string Results(Context context, long siteId)
        {
            return SiteUtilities.SetSiteSettings(
                context: context,
                siteId: siteId);
        }
    }
}
