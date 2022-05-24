using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Security
{
    public static class IpAddresses
    {
        /// <summary>
        /// 引数allowIpAddressesで渡されたアドレスリストに引数ipAddressが含まれているかどうか。
        /// allowIpAddressesはIPアドレス文字列(CIDR指定も可)のリスト。
        /// </summary>
        public static bool AllowedIpAddress(IList<string> allowIpAddresses, string ipAddress)
        {
            if (allowIpAddresses?.Any() != true)
            {
                return true;
            }
            return allowIpAddresses
                .Select(addr => IpRange.FromCidr(addr))
                .Any(range => range.InRange(ipAddress));
        }
    }
}