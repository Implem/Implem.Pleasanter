using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
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
        public static bool AllowedIpAddress(
            Context context,
            IList<string> allowIpAddresses,
            IList<string> ipRestrictionExcludeMembers,
            string ipAddress)
        {
            if (allowIpAddresses?.Any() != true)
            {
                return true;
            }
            if (ipRestrictionExcludeMembers?.Any() == true)
            {
                // IpRestrictionExcludeMembersの指定がありログインしていない場合はIP制限しない(ログイン画面を表示するため)
                if (context.Authenticated != true)
                {
                    return true;
                }
                if (ipRestrictionExcludeMembers.Contains("Dept" + context.DeptId))
                {
                    return true;
                }
                if (IpRestrictionExcludeGroup(
                    context: context,
                    ipRestrictionExcludeMembers: ipRestrictionExcludeMembers))
                {
                    return true;
                }
                if (ipRestrictionExcludeMembers.Contains("User" + context.UserId))
                {
                    return true;
                }
            }
            return allowIpAddresses
                .Select(IpRange.FromCidr)
                .Any(range => range.InRange(ipAddress));
        }

        private static bool IpRestrictionExcludeGroup(
            Context context,
            IList<string> ipRestrictionExcludeMembers)
        {
            var excludeGroups = ipRestrictionExcludeMembers
                .Select(o => o.RegexMatches("(?<=Group)[0-9]+").FirstOrDefault().ToInt())
                .Where(o => o > 0)
                .ToList();
            var groups = PermissionUtilities.Groups(context: context);
            return groups.Any(excludeGroups.Contains);
        }
    }
}