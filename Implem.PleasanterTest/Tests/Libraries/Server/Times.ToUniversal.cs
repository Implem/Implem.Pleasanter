using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Utilities;
using System;
using Xunit;

namespace Implem.PleasanterTest.Tests.Libraries.Server
{
    public partial class Times
    {
        [Theory]
        [MemberData(nameof(GetData))]
        public void TestToUniversal(
            string srcTimeZone,
            string dstTimeZone,
            string srcDate,
            string dstDate,
            UserModel userModel)
        {
            using (new LocalTimeZoneInfoMocker(TimeZoneInfo.FindSystemTimeZoneById(dstTimeZone)))
            {
                var context = ContextData.Get(
                    userId: userModel.UserId,
                    userTimeZone: srcTimeZone);
                var date = DateTime.Parse(srcDate).ToUniversal(context);
            Assert.True(dstDate == date.ToStr(), $"param:\"{dstDate}\" == calc:\"{date.ToStr()}\" (os:\"{TimeZoneInfo.Local.Id}\")");
            }
        }
    }
}
