using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using Xunit;
using Initializer = Implem.PleasanterTest.Utilities.Initializer;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsNewEditNotification))]
    public class SiteSettingsNewEditNotification : IDisposable
    {
        private ParameterAccessor.Parts.Notification SavedNotification;

        public SiteSettingsNewEditNotification()
        {
            SavedNotification = Parameters.Notification;
        }

        public void Dispose()
        {
            Parameters.Notification = SavedNotification;
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            Parameters.Notification = new ParameterAccessor.Parts.Notification() { Mail = true };
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
                    title: "サイト設定 - Notifications",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "NewNotification")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#NotificationDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextContains(
                            method: "Html",
                            target: "#NotificationDialog",
                            value: @"<option value=""1"" selected=""selected"">メール</option>")),
                    userType: UserData.UserTypes.Privileged),
                new TestPart(
                    title: "サイト設定 - Notifications",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "EditNotification"),
                        new KeyValue("NotificationId", "1")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#NotificationDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextContains(
                            method: "Html",
                            target: "#NotificationDialog",
                            value: "implem.dummy.xunit1@implem.dummy.co.jp")),
                    userType: UserData.UserTypes.Privileged)
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
