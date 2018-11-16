using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Data;
using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class Addresses
    {
        public static IEnumerable<string> GetEnumerable(
            Context context, string addresses)
        {
            return addresses.Split(';', ',')
                .Select(address => address.Trim())
                .SelectMany(address => ConvertedMailAddresses(
                    context: context,
                    address: address))
                .Where(address => !address.IsNullOrEmpty());
        }

        private static IEnumerable<string> ConvertedMailAddresses(
            Context context, string address)
        {
            var userId = address?.RegexFirst(@"(?<=\[User)[0-9]+(?=\])").ToInt();
            return userId > 0
                ? Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn()
                            .MailAddress(),
                        where: Rds.MailAddressesWhere()
                            .OwnerType("Users")
                            .OwnerId(userId)))
                                .AsEnumerable()
                                .Select(o => o.String("MailAddress"))
                                .ToList()
                : address?.ToSingleList();
        }

        public static string BadAddress(Context context, string addresses)
        {
            foreach (var address in GetEnumerable(
                context: context,
                addresses: addresses))
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

        public static string ExternalMailAddress(Context context, string addresses)
        {
            var domains = Parameters.Mail.InternalDomains
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
            if (domains.Count() == 0) return string.Empty;
            foreach (var mailAddress in GetEnumerable(
                context: context,
                addresses: addresses))
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