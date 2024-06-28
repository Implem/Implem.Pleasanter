using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Implem.PleasanterTest.Tests.Libraries.Server
{
    public partial class Times
    {
        // メソッドの呼び出し途中でOSのタイムゾーンが変わらないように排他制御を行う。
        private static object LockObj = new object();


        public static IEnumerable<object[]> GetData()
        {
            var user = UserData.Get(userType: UserData.UserTypes.General1);
            var list = new List<object[]>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windowsは"Asia/Tokyo"の夏時間非対応
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/05 23:00:00", "1951/05/05 14:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/06 00:00:00", "1951/05/05 15:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/06 01:00:00", "1951/05/05 16:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/06 02:00:00", "1951/05/05 17:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/08 23:00:00", "1951/09/08 14:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/09 00:00:00", "1951/09/08 15:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/09 01:00:00", "1951/09/08 16:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/09 02:00:00", "1951/09/08 17:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/05/05 14:00:00", "1951/05/05 23:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/05/05 15:00:00", "1951/05/06 00:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/05/05 16:00:00", "1951/05/06 01:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/09/08 14:00:00", "1951/09/08 23:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/09/08 15:00:00", "1951/09/09 00:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/09/08 16:00:00", "1951/09/09 01:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 01:00:00", "2023/03/12 18:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 02:00:00", "2023/03/12 19:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 03:00:00", "2023/03/12 19:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 04:00:00", "2023/03/12 20:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/11/05 00:00:00", "2023/11/05 16:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/11/05 01:00:00", "2023/11/05 18:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/11/05 02:00:00", "2023/11/05 19:00:00", user));
            }
            else
            {
                // Linuxは"Asia/Tokyo"の夏時間対応
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/05 23:00:00", "1951/05/05 14:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/06 00:00:00", "1951/05/05 15:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/06 01:00:00", "1951/05/05 15:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/05/06 02:00:00", "1951/05/05 16:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/08 23:00:00", "1951/09/08 13:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/09 00:00:00", "1951/09/08 15:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/09 01:00:00", "1951/09/08 16:00:00", user));
                list.Add(TestData("Asia/Tokyo", "Etc/UTC", "1951/09/09 02:00:00", "1951/09/08 17:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/05/05 14:00:00", "1951/05/05 23:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/05/05 15:00:00", "1951/05/06 01:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/05/05 16:00:00", "1951/05/06 02:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/09/08 14:00:00", "1951/09/09 00:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/09/08 15:00:00", "1951/09/09 00:00:00", user));
                list.Add(TestData("Etc/UTC", "Asia/Tokyo", "1951/09/08 16:00:00", "1951/09/09 01:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 01:00:00", "2023/03/12 18:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 02:00:00", "2023/03/12 19:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 03:00:00", "2023/03/12 19:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/03/12 04:00:00", "2023/03/12 20:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/11/05 00:00:00", "2023/11/05 16:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/11/05 01:00:00", "2023/11/05 18:00:00", user));
                list.Add(TestData("America/Los_Angeles", "Asia/Tokyo", "2023/11/05 02:00:00", "2023/11/05 19:00:00", user));
            }
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/03/12 01:00:00", "2023/03/12 09:00:00", user));
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/03/12 02:00:00", "2023/03/12 10:00:00", user));
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/03/12 03:00:00", "2023/03/12 10:00:00", user));
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/03/12 04:00:00", "2023/03/12 11:00:00", user));
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/11/05 00:00:00", "2023/11/05 07:00:00", user));
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/11/05 01:00:00", "2023/11/05 09:00:00", user));
            list.Add(TestData("America/Los_Angeles", "Etc/UTC", "2023/11/05 02:00:00", "2023/11/05 10:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/03/12 09:00:00", "2023/03/12 01:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/03/12 10:00:00", "2023/03/12 03:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/03/12 10:00:00", "2023/03/12 03:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/03/12 11:00:00", "2023/03/12 04:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/11/05 07:00:00", "2023/11/05 00:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/11/05 08:00:00", "2023/11/05 01:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/11/05 09:00:00", "2023/11/05 01:00:00", user));
            list.Add(TestData("Etc/UTC", "America/Los_Angeles", "2023/11/05 10:00:00", "2023/11/05 02:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/04/02 01:00:00", "2023/04/01 12:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/04/02 02:00:00", "2023/04/01 14:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/04/02 03:00:00", "2023/04/01 15:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/09/24 01:00:00", "2023/09/23 13:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/09/24 02:00:00", "2023/09/23 14:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/09/24 03:00:00", "2023/09/23 14:00:00", user));
            list.Add(TestData("Pacific/Auckland", "Etc/UTC", "2023/09/24 04:00:00", "2023/09/23 15:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/04/01 13:00:00", "2023/04/02 02:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/04/01 14:00:00", "2023/04/02 02:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/04/01 15:00:00", "2023/04/02 03:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/04/01 16:00:00", "2023/04/02 04:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/09/23 13:30:00", "2023/09/24 01:30:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/09/23 14:00:00", "2023/09/24 03:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/09/23 15:00:00", "2023/09/24 04:00:00", user));
            list.Add(TestData("Etc/UTC", "Pacific/Auckland", "2023/09/23 16:00:00", "2023/09/24 05:00:00", user));
            foreach (var item in list)
            {
                yield return item;
            }
        }

        private static object[] TestData(
            string tzLocal,
            string tzUser,
            string srcDate,
            string dstDate,
            UserModel userModel)
        {
            return new object[]
            {
                tzLocal,
                tzUser,
                srcDate,
                dstDate,
                userModel
            };
        }

        /// <summary>
        /// TimeZoneInfo.Localを書き換える為のクラス
        /// </summary>
        public class LocalTimeZoneInfoMocker : IDisposable
        {
            public LocalTimeZoneInfoMocker(TimeZoneInfo mockTimeZoneInfo)
            {
                TimeZoneInfo.ClearCachedData();
                var info = typeof(TimeZoneInfo)
                    .GetField(
                        "s_cachedData",
                        BindingFlags.NonPublic | BindingFlags.Static);
                var cachedData = info.GetValue(null);
                var field = cachedData
                    .GetType()
                    .GetField(
                        "_localTimeZone",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Instance);
                field.SetValue(
                    cachedData,
                    mockTimeZoneInfo);
            }

            public void Dispose()
            {
                TimeZoneInfo.ClearCachedData();
            }
        }
    }
}
