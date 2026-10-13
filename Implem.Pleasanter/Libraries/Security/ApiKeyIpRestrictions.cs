using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
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
            var parts = value.Split('/');
            if (parts.Length > 2)
            {
                return false;
            }
            if (parts.Length == 2
                && (!int.TryParse(parts[1], out var maskbits)
                    || maskbits < 1
                    || maskbits > 32))
            {
                return false;
            }
            var octet = parts[0].Split('.');
            if (octet.Length != 4)
            {
                return false;
            }
            return octet.All(o => uint.TryParse(o, out var value) && value <= 255);
        }
    }
}
