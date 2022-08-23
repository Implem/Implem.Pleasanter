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
    public class BinariesSiteImageThumbnail
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            UserModel userModel)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesSiteImageThumbnail(id: id));
            var results = Results(context: context);
            Assert.True(results);
        }

        public static IEnumerable<object[]> GetData()
        {
            // 事前にテスト用の画像をアップロードしてGUIDを取得。
            var siteModel = Initializer.Sites.Get("プロジェクト管理の例");
            BinaryUtilities.UpdateSiteImage(
                context: ContextData.Get(
                    userId: UserData.Get(userType: UserData.UserTypes.TenantManager1).UserId,
                    routeData: RouteData.BinariesUpdateSiteImage(id: siteModel.SiteId),
                    httpMethod: "POST",
                    fileName: "Image3.png",
                    contentType: "image/png"),
                siteModel: siteModel);
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "プロジェクト管理の例")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(
            string title,
            UserModel userModel)
        {
            return new object[]
            {
                title,
                userModel
            };
        }

        private static bool Results(Context context)
        {
            var (bytes, contentType) = BinaryUtilities.SiteImageThumbnail(
                context: context,
                siteModel: new SiteModel(
                    context: context,
                    siteId: context.Id));
            return bytes != null;
        }
    }
}
