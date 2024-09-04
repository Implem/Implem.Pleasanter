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
    public class BinariesUploadImage
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void Test(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesUploadImage(id: id),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
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
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "ネットワークの構築",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "Issues_Body")),
                    fileName: "Image1.png",
                    baseTests: BaseData.Tests(
                        JsonData.ExistsOne(
                            method: "InsertText",
                            target: "#Issues_Body")))
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: testPart.Forms,
                    fileName: testPart.FileName,
                    userModel: testPart.UserModel,
                    baseTests: testPart.BaseTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<BaseTest> baseTests)
        {
            return new object[]
            {
                title,
                forms,
                fileName,
                userModel,
                baseTests
            };
        }

        private static string Results(Context context)
        {
            return BinaryUtilities.UploadImage(
                context: context,
                id: context.Id);
        }
    }
}
