using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
namespace Implem.SupportTools.MailTester.Model
{
    public static class Addresses
    {
        public static IEnumerable<string> GetEnumerable(string addresses)
        {
            return addresses.Split(';', ',')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
        }

        public static string BadAddress(string addresses)
        {
            foreach (var address in GetEnumerable(addresses))
            {
                if (Get(address) == string.Empty)
                {
                    return address;
                }
            }
            return string.Empty;
        }

        public static string Get(string address)
        {
            return address.RegexFirst(
                @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static string ExternalMailAddress(string mailAddresses, string internalDomains)
        {
            var domains = internalDomains
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
            if (domains.Count() == 0) return string.Empty;
            foreach (var mailAddress in GetEnumerable(mailAddresses))
            {
                if (!domains.Any(o => Get(mailAddress).EndsWith(o)))
                {
                    return mailAddress;
                }
            }
            return string.Empty;
        }

        public static MailAddress From(MailAddress from, string fixedFrom, string allowedFrom)
        {
            return FixedFrom(from, fixedFrom, allowedFrom)
                ? new MailAddress(fixedFrom)
                : from;
        }

        public static bool FixedFrom(MailAddress from,string fixedFrom, string allowedFrom)
        {
            return
                !string.IsNullOrEmpty(fixedFrom) &&
                allowedFrom?.Contains(from.Address) != true;
        }
    }
}