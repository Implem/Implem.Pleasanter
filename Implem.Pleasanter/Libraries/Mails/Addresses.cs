using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using MimeKit;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Mails
{
    public static class Addresses
    {
        public static MailboxAddress SupportFrom()
        {
            return MailboxAddress.Parse(Parameters.Mail.SupportFrom);
        }

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
            var addressList = new List<string>();
            var isDisplayNameFlg = false;
            var addressStartIndex = 0;
            addresses
                .Select((c, i) => new
                {
                    Char = c,
                    Index = i
                })
                .ForEach(data =>
                {
                    if (data.Char == '\"')
                    {
                        isDisplayNameFlg = !isDisplayNameFlg;
                    }
                    if (!isDisplayNameFlg)
                    {
                        if (data.Char == ',' || data.Char == ';' || data.Char == '\n')
                        {
                            addressList.Add(addresses
                                .Substring(addressStartIndex, (data.Index - addressStartIndex))
                                .Trim());
                            addressStartIndex = data.Index + 1;
                        }
                        else if (data.Index == addresses.Length - 1)
                        {
                            addressList.Add(addresses.Substring(addressStartIndex).Trim());
                            addressStartIndex = data.Index;
                        }
                    }
                });
            return addressList;
        }

        private static IEnumerable<string> ConvertedMailAddresses(
            Context context, string address)
        {
            var userId = address?.RegexFirst(@"(?<=\[User)[0-9]+(?=\])").ToInt();
            return userId > 0
                ? Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn()
                            .MailAddress(),
                        join: new SqlJoinCollection(
                            new SqlJoin(
                                tableBracket: "\"Users\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"MailAddresses\".\"OwnerId\"=\"Users\".\"UserId\"")),
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
            if (MailboxAddress.TryParse(address, out MailboxAddress internetAddress))
            {
                return internetAddress.Address;
            }
            else
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
            return MailboxAddress.TryParse(address, out MailboxAddress _);
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

        public static MailboxAddress From(MailboxAddress from)
        {
            return FixedFrom(from)
                ? MailboxAddress.Parse(Parameters.Mail.FixedFrom)
                : from;
        }

        public static bool FixedFrom(MailboxAddress from)
        {
            return
                !Parameters.Mail.FixedFrom.IsNullOrEmpty() &&
                Parameters.Mail.AllowedFrom?.Contains(from.Address) != true;
        }
    }
}