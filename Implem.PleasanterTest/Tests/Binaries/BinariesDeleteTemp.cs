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
    public class BinariesDeleteTemp
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
                routeData: RouteData.BinariesDeleteTemp(id: id),
                httpMethod: "POST",
                forms: forms);
            var results = Results(context: context);
            Initializer.SaveResults(results);
            Assert.True(Tester.Test(
                context: context,
                results: results,
                baseTests: baseTests));
        }

        public static IEnumerable<object[]> GetData()
        {
            var id = Initializer.Titles.Get("業務アプリケーション設定のデザインシート作成");
            var uuid = Strings.NewGuid();
            BinaryUtilities.UploadFile(
                context: ContextData.Get(
                    userId: UserData.Get(userType: UserData.UserTypes.General1).UserId,
                    routeData: RouteData.BinariesUpload(id: id),
                    httpMethod: "POST",
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", "Issues_AttachmentsA"),
                        new KeyValue("ColumnName", "AttachmentsA"),
                        new KeyValue("AttachmentsData", "[]"),
                        new KeyValue("fileNames", "[\"Attachments.txt\"]"),
                        new KeyValue("fileSizes", "[15]"),
                        new KeyValue("fileTypes", "[\"text/plain\"]"),
                        new KeyValue("FileHash", "242988169a3d92a28807ff02153d5e3a"),
                        new KeyValue("uuid", uuid),
                        new KeyValue("Uuids", uuid)),
                    fileName: "Attachments.txt",
                    contentType: "text/json"),
                id: id,
                contentRange: null);
            var testParts = new List<TestPart>()
            {
                new TestPart(
                    title: "業務アプリケーション設定のデザインシート作成",
                    guid: uuid,
                    forms: FormsUtilities.Get(new KeyValue("Guid", uuid)),
                    baseTests: BaseData.Tests(TextData.Equals(value: "[]")))
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
            return BinaryUtilities.DeleteTemp(context: context);
        }
    }
}
