using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.PublishBinaries
{
    public class PublishBinariesSiteImageIcon
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(string title)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userType: UserData.UserTypes.Anonymous,
                routeData: RouteData.PublishBinariesSiteImageIcon(id: id));
            var results = Results(context: context);
			Assert.True(results);
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
                    fileName: "Image3.png",
                    contentType: "image/png"),
                siteModel: siteModel);
            var testParts = new List<TestPart>()
            {
                new TestPart(title: "WBS")
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(title: testPart.Title);
            }
        }

        private static object[] TestData(string title)
        {
            return new object[]
            {
                title
            };
        }

        private static bool Results(Context context)
        {
            var (bytes, contentType) = BinaryUtilities.SiteImageIcon(
                context: context,
                siteModel: new SiteModel(
                    context: context,
                    siteId: context.Id));
            return bytes != null;
        }
    }
}
