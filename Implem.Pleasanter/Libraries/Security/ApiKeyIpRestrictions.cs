using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class ApiKeyIpRestrictions
    {
        public static List<string> IpAddressRanges(string value)
        {
            return value
                ?.SplitReturn()
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
        }

        public static List<string> InvalidIpAddressRanges(string value)
        {
            return IpAddressRanges(value: value)
                ?.Where(o => !Valid(value: o))
                .ToList()
                ?? new List<string>();
        }

        public static bool Allowed(Context context)
        {
            return context.ApiKey.IsNullOrEmpty()
                || IpAddresses.AllowedIpAddress(
                    context: context,
                    allowIpAddresses: IpAddressRanges(value: context.ApiAllowIpAddressRange),
                    ipRestrictionExcludeMembers: null,
                    ipAddress: context.UserHostAddress);
        }

        private static bool Valid(string value)
        {
            return IpRange.TryParse(
                cidrIpString: value,
                network: out _,
                strictPrefixLength: true);
        }
    }
}
