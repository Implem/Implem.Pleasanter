using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Settings
{
    [Collection(nameof(SiteSettingsUpdate))]
    public class SiteSettingsUpdate
    {

        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel)
        {
            var siteId = Initializer.Sites.Get(title).SiteId;
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.ItemsSetSiteSettings(id: siteId),
                httpMethod: "POST",
                forms: forms);
            var html = Results(context: context, id: siteId);
            Initializer.SaveResults(html);
            bool result = html.Contains("サイト設定 - 更新 \\\\\\\" を更新しました。");
            Assert.True(result);
        }

        public static IEnumerable<object[]> GetData()
        {
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "サイト設定 - 更新",
                    forms: FormsUtilities.Get(new KeyValue("Sites_SiteName", "サイト設定 - 更新後")),
                    userType: UserData.UserTypes.Privileged),
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                forms,
                userModel
            };
        }

        private static string Results(Context context, long id)
        {
            return new ItemModel(
                context: context,
                referenceId: id)
                    .Update(context: context);
        }
    }
}
