using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Binaries
{
    public class BinariesTenantImageLogo
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(UserModel userModel)
        {
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesTenantImageLogo(id: Initializer.TenantId));
            var results = Results(context: context);
            Assert.True(results);
        }

        public static IEnumerable<object[]> GetData()
        {
            // 事前にテスト用の画像をアップロード。
            var context = ContextData.Get(
                userId: UserData.Get(userType: UserData.UserTypes.TenantManager1).UserId,
                routeData: RouteData.BinariesUpdateTenantImage(id: Initializer.TenantId),
                httpMethod: "POST",
                fileName: "Image1.png",
                contentType: "image/png");
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context: context);
            var tenantModel = new TenantModel(
                context: context,
                ss: ss)
                    .Get(
                        context: context,
                        ss: ss);
            BinaryUtilities.UpdateTenantImage(
                context: context,
                tenantModel: tenantModel);
            var testParts = new List<TestPart>()
            {
                new TestPart()
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(userModel: testPart.UserModel);
            }
        }

        private static object[] TestData(UserModel userModel)
        {
            return new object[]
            {
                userModel
            };
        }

        private static bool Results(Context context)
        {
            var (bytes, contentType) = BinaryUtilities.TenantImageLogo(
                context: context,
                tenantModel: new TenantModel(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context: context)));
            return bytes != null;
        }
    }
}
