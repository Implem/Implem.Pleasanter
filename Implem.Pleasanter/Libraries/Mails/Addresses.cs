using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Data;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Libraries.DataSources.SqlServer;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class Addresses
    {
        public static IEnumerable<string> GetEnumerable(
            Context context, string addresses)
        {
            return addresses
                .Split(';', ',', '\n', ' ')
                .Select(address => address.Trim())
                .SelectMany(address => ConvertedMailAddresses(
                    context: context,
                    address: address))
                .Where(o => Get(o) == o)
                .Where(o => ExternalMailAddress(
                    context: context,
                    addresses: o).IsNullOrEmpty())
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
                        join: new SqlJoinCollection(
                            new SqlJoin(
                                tableBracket: "[Users]",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "[MailAddresses].[OwnerId]=[Users].[UserId]")),
                        where: Rds.MailAddressesWhere()
                            .OwnerType("Users")
                            .OwnerId(userId)
                            .Users_TenantId(context.TenantId)))
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
                Parameters.Mail.AddressValidation,
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