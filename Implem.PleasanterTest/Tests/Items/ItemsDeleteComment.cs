using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using System.Collections.Generic;
using Xunit;

namespace Implem.PleasanterTest.Tests.Items
{
    [Collection(nameof(ItemsDeleteComment))]
    public class ItemsDeleteComment
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
                routeData: RouteData.ItemsDeleteComment(id: id),
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
            var testParts = new List<MyTestPart>()
            {
                new MyTestPart(
                    title: "WBS",
                    commentId: 1,
                    updateResponseType: 1,
                    userType: UserData.UserTypes.TenantManager1),
                new MyTestPart(
                    title: "サーバの構築",
                    commentId: 2,
                    updateResponseType: 0),
                new MyTestPart(
                    title: "ディスク容量の要件に誤り",
                    commentId: 1,
                    updateResponseType: 0),
                new MyTestPart(
                    title: "Wiki1",
                    commentId: 1,
                    updateResponseType: 1)
            };
            foreach (var testPart in testParts)
            {
                yield return TestData(
                    title: testPart.Title,
                    forms: FormsUtilities.Get(
                        new KeyValue("ControlId", $"DeleteComment,{testPart.CommentId}")),
                    userModel: testPart.UserModel,
                    baseTests: testPart.UpdateResponseType == 0
                        ? BaseData.Tests(
                            JsonData.ReplaceAll(
                                target: "#MainContainer",
                                selector: "#Editor"))
                        : BaseData.Tests(
                            JsonData.ExistsOne(
                                method: "Html",
                                target: "#HeaderTitle"),
                            JsonData.ExistsOne(
                                method: "Html",
                                target: "#RecordInfo"),
                            JsonData.ExistsOne(
                                method: "SetMemory",
                                target: "formChanged"),
                            JsonData.ExistsOne(
                                method: "Remove",
                                target: $"[id=\"Comment{testPart.CommentId}.wrapper\"]")));
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
            var itemModel = Initializer.ItemIds.Get(context.Id);
            return itemModel.DeleteComment(context: context);
        }

        private class MyTestPart : TestPart
        {
            public int CommentId { get; set; }
            public int UpdateResponseType { get; set; }

            public MyTestPart(
                string title,
                int commentId,
                int updateResponseType,
                UserData.UserTypes userType = UserData.UserTypes.General1)
            {
                Title = title;
                CommentId = commentId;
                UpdateResponseType = updateResponseType;
                UserModel = UserData.Get(userType: userType);
            }
        }
    }
}
