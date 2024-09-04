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
        public void TestToLocal(
            string srcTimeZone,
            string dstTimeZone,
            string srcDate,
            string dstDate,
            UserModel userModel)
        {
            lock(LockObj)
            {
                using (new LocalTimeZoneInfoMocker(TimeZoneInfo.FindSystemTimeZoneById(srcTimeZone)))
                {
                    var context = ContextData.Get(
                        userId: userModel.UserId,
                        userTimeZone: dstTimeZone);
                    var date = DateTime.Parse(srcDate).ToLocal(context);
            Assert.True(dstDate == date.ToStr(), $"param:\"{dstDate}\" == calc:\"{date.ToStr()}\" (os:\"{TimeZoneInfo.Local.Id}\")");
                }
            }
        }
    }
}
