using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
namespace Implem.Pleasanter.Libraries.Mails
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

        public static string ExternalMailAddress(string mailAddresses)
        {
            var domains = Parameters.Mail.InternalDomains
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

        public static MailAddress From(MailAddress from)
        {
            return FixedFrom(from)
                ? new MailAddress(Parameters.Mail.FixedFrom)
                : from;
        }

        public static bool FixedFrom(MailAddress from)
        {
            return
                !Parameters.Mail.FixedFrom.IsNullOrEmpty() &&
                Parameters.Mail.AllowedFrom?.Contains(from.Address) != true;
        }
    }
}