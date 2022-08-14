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
            List<JsonTest> jsonTests)
        {
            var id = Initializer.Titles.Get(title);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.BinariesUploadImage(id: id),
                httpMethod: "POST",
                forms: forms,
                fileName: fileName,
                contentType: "text/json");
            var results = Results(context: context);
            Assert.True(Compare.Json(
                context: context,
                results: results,
                jsonTests: jsonTests));
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
                    jsonTests: JsonData.Tests(
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
                    jsonTests: testPart.JsonTests);
            }
        }

        private static object[] TestData(
            string title,
            Forms forms,
            string fileName,
            UserModel userModel,
            List<JsonTest> jsonTests)
        {
            return new object[]
            {
                title,
                forms,
                fileName,
                userModel,
                jsonTests
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
