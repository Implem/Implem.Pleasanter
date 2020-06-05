using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Data;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Libraries.DataSources.SqlServer;
using System;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class Addresses
    {
        public static IEnumerable<string> Get(
            Context context, string addresses)
        {
            return addresses
                .GetSplitList()
                .SelectMany(address => ConvertedMailAddresses(
                    context: context,
                    address: address))
                .Where(address => IsValid(address: address))
                .Where(address => ExternalMailAddress(addresses: address).IsNullOrEmpty())
                .Where(address => !address.IsNullOrEmpty());
        }

        public static List<string> GetSplitList(this string addresses)
        {
            return addresses
                ?.Split(';', ',', '\n')
                .Select(address => address.Trim())
                .ToList()
                    ?? new List<string>();
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

        public static string BadAddress(string addresses)
        {
            foreach (var address in addresses.GetSplitList())
            {
                if (!IsValid(address))
                {
                    return address;
                }
            }
            return string.Empty;
        }

        public static string GetBody(string address)
        {
            if (address.IsNullOrEmpty())
            {
                return string.Empty;
            }
            try
            {
                return new MailAddress(address).Address;
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        private static bool IsValid(string address)
        {
            if (address.IsNullOrEmpty())
            {
                return false;
            }
            try
            {
                var mailAddress = new MailAddress(address);
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        public static string ExternalMailAddress(string addresses)
        {
            var domains = Parameters.Mail.InternalDomains
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
            if (domains.Count() == 0) return string.Empty;
            foreach (var mailAddress in addresses.GetSplitList())
            {
                if (!domains.Any(o => GetBody(mailAddress).EndsWith(o)))
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