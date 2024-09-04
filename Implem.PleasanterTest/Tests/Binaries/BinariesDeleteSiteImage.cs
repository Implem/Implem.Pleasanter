using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    public class BinariesDeleteSiteImage
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesDeleteSiteImage(id: id),
                httpMethod: "POST",
                forms: forms,
                contentType: "image/png");
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            // 事前にテスト用の画像をアップロード。
            var siteModel = Initializer.Sites.Get("WBS");
            BinaryUtilities.UpdateSiteImage(
                context: ContextData.Get(
                    userId: UserData.Get(userType: UserData.UserTypes.TenantManager1).UserId,
                    routeData: RouteData.BinariesUpdateSiteImage(id: siteModel.SiteId),
                    httpMethod: "POST",
                    fileName: "Image1.png",
                    contentType: "image/png"),
                siteModel: siteModel);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "WBS",
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#SiteImageIconContainer"),
                        JsonData.ExistsOne(
                            method: "Html",
                            target: "#SiteImageSettingsEditor"),
                        JsonData.Value(
                            method: "Message",
                            value: "{\"Id\":\"FileDeleteCompleted\",\"Text\":\"ファイルの削除が完了しました。\",\"Css\":\"alert-success\"}")),
                    userType: UserData.UserTypes.TenantManager1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return BinaryUtilities.DeleteSiteImage(
                context: context,
                siteModel: new SiteModel(
                    context: context,
                    siteId: context.Id));
        }
    }
}
