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
    [Collection(nameof(SiteSettingsUpdateNotification))]
    public class SiteSettingsUpdateNotification : IDisposable
    {
        private ParameterAccessor.Parts.Notification SavedNotification;

        public SiteSettingsUpdateNotification()
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
                        new KeyValue("ControlId", "UpdateNotification"),
                        new KeyValue("NotificationId", "1"),
                        new KeyValue("NotificationType", "1"),
                        new KeyValue("NotificationPrefix", ""),
                        new KeyValue("NotificationSubject", "通知の変更"),
                        new KeyValue("NotificationAddress", "updated.dummy.xunit@implem.dummy.co.jp"),
                        new KeyValue("NotificationCcAddress", ""),
                        new KeyValue("NotificationBccAddress", ""),
                        new KeyValue("NotificationToken", ""),
                        new KeyValue("NotificationMethodType", "1"),
                        new KeyValue("NotificationEncoding", "utf-8"),
                        new KeyValue("NotificationMediaType", "application/json"),
                        new KeyValue("NotificationRequestHeaders", ""),
                        new KeyValue("NotificationUseCustomFormat", "false"),
                        new KeyValue("NotificationFormat", @"{Url}{""Name"":""[タイトル]""}{""Name"":""[内容]""}{""Name"":""[状況]""}{""Name"":""[管理者]""}
                            {""Name"":""[担当者]""}{""Name"":""[添付ファイルA]""}{""Name"":""[コメント]""}{UserName}<{MailAddress}>"),
                        new KeyValue("MonitorChangesColumnsAll", @"[""Title"",""Body"",""Status"",""Manager"",""Owner"",""AttachmentsA"",""Comments""]"),
                        new KeyValue("BeforeCondition", ""),
                        new KeyValue("AfterCondition", ""),
                        new KeyValue("Expression", "1"),
                        new KeyValue("NotificationAfterCreate", "true"),
                        new KeyValue("NotificationAfterUpdate", "true"),
                        new KeyValue("NotificationAfterDelete", "true"),
                        new KeyValue("NotificationAfterCopy", "true"),
                        new KeyValue("NotificationAfterBulkUpdate", "true"),
                        new KeyValue("NotificationAfterBulkDelete", "true"),
                        new KeyValue("NotificationAfterImport", "true"),
                        new KeyValue("NotificationDisabled", "false")),
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "ReplaceAll",
                            target: "#EditNotificationWrap"),
                        JsonData.ExistsOne(
                            method: "CloseDialog"),
                        JsonData.ExistsOne(
                            method: "SetMemory",
                            target: "formChanged"),
                        JsonData.TextContains(
                            method: "ReplaceAll",
                            target: "#EditNotificationWrap",
                            value: "通知の変更"),
                        JsonData.TextContains(
                            method: "ReplaceAll",
                            target: "#EditNotificationWrap",
                            value: "updated.dummy.xunit@implem.dummy.co.jp")),
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
