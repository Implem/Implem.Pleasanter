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
    [Collection(nameof(SiteSettingsDeleteReminders))]
    public class SiteSettingsDeleteReminders : IDisposable
    {
        private ParameterAccessor.Parts.BackgroundService SavedBackgroundService;
        private ParameterAccessor.Parts.Reminder SavedReminder;
        private ParameterAccessor.Parts.Service SavedService;

        public SiteSettingsDeleteReminders()
        {
            SavedBackgroundService = Parameters.BackgroundService;
            SavedReminder = Parameters.Reminder;
            SavedService = Parameters.Service;
        }

        public void Dispose()
        {
            Parameters.BackgroundService = SavedBackgroundService;
            Parameters.Reminder = SavedReminder;
            Parameters.Service = SavedService;
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            List<BaseTest> baseTests,
            UserModel userModel)
        {
            Parameters.BackgroundService = new ParameterAccessor.Parts.BackgroundService() { Reminder = true };
            Parameters.Reminder = new ParameterAccessor.Parts.Reminder() { Mail = true };
            Parameters.Service = new ParameterAccessor.Parts.Service() { AbsoluteUri = "http://pleasanter.example.local" };
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
                    title: "サイト設定 - Reminders",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "DeleteReminders"),
                        new KeyValue("EditReminder", @"[""1""]")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditReminder"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextCountOf(
                            method: "ReplaceAll",
                            target: "#EditReminder",
                            value: "<td>テスト用リマインダー1</td>",
                            estimate: 0)),
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
