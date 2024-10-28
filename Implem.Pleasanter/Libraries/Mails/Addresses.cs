using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
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
            if(addresses is null)
            {
                return [];
            }
            return addresses
                .GetSplitList()
                .SelectMany(address => ConvertedMailAddresses(
                    context: context,
                    address: address))
                .Where(address => IsValid(address: address))
                .Where(address => ExternalMailAddress(addresses: address).IsNullOrEmpty())
                .Where(address => !address.IsNullOrEmpty())
                .Distinct();
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
            var users = new List<User>();
            var deptId = address?.RegexFirst(@"(?<=\[Dept)[0-9]+(?=\])").ToInt() ?? 0;
            var groupId = address?.RegexFirst(@"(?<=\[Group)[0-9]+(?=\])").ToInt() ?? 0;
            var userId = address?.RegexFirst(@"(?<=\[User)[0-9]+(?=\])").ToInt() ?? 0;
            if (deptId > 0)
            {
                users.AddRange(SiteInfo.DeptUsers(
                    context: context,
                    dept: SiteInfo.Dept(
                        tenantId: context.TenantId,
                        deptId: deptId)));
            }
            else if (groupId > 0)
            {
                users.AddRange(SiteInfo.GroupUsers(
                    context: context,
                    group: SiteInfo.Group(
                        tenantId: context.TenantId,
                        groupId: groupId)));
            }
            else if (userId > 0)
            {
                users.Add(SiteInfo.User(
                    context: context,
                    userId: userId));
            }
            if (users.Any())
            {
                var mailAddresses = new List<string>();
                users.ForEach(user => Repository.ExecuteTable(
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
                            .OwnerId(user.Id)
                            .Users_TenantId(context.TenantId)
                            .Users_Disabled(false)))
                                .AsEnumerable()
                                .Select(dataRow => dataRow.String("MailAddress"))
                                .ForEach(mailAddress =>
                                    mailAddresses.Add(mailAddress)));
                return mailAddresses.Distinct();
            }
            else
            {
                return address?.ToSingleList();
            }
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
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(address);
            }
            catch (System.FormatException)
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

        public static string ReplacedAddress(Context context, Column column, string value)
        {
            var users = new List<User>();
            switch (column.Type)
            {
                case Column.Types.Dept:
                    users.AddRange(column.MultipleSelections == true
                        ? value.Deserialize<List<int>>()
                            ?.Select(deptId => SiteInfo.Dept(
                                tenantId: context.TenantId,
                                deptId: deptId))
                            .SelectMany(dept => SiteInfo.DeptUsers(
                                context: context,
                                dept: dept)) ?? new List<User>()
                        : SiteInfo.DeptUsers(
                            context: context,
                            dept: SiteInfo.Dept(
                                tenantId: context.TenantId,
                                deptId: value.ToInt())));
                    break;
                case Column.Types.Group:
                    users.AddRange(column.MultipleSelections == true
                        ? value.Deserialize<List<int>>()
                            ?.Select(groupId => SiteInfo.Group(
                                tenantId: context.TenantId,
                                groupId: groupId))
                            .SelectMany(group => SiteInfo.GroupUsers(
                                context: context,
                                group: group)) ?? new List<User>()
                        : SiteInfo.GroupUsers(
                            context: context,
                            group: SiteInfo.Group(
                                tenantId: context.TenantId,
                                groupId: value.ToInt())));
                    break;
                case Column.Types.User:
                    users.AddRange(column.MultipleSelections == true
                        ? value.Deserialize<List<int>>()
                            ?.Select(userId => SiteInfo.User(
                                context: context,
                                userId: userId)) ?? new List<User>()
                        : SiteInfo.User(
                            context: context,
                            userId: value.ToInt()).ToSingleList());
                    break;
                default:
                    return column.MultipleSelections == true
                        ? value.Deserialize<List<string>>()?.Join()
                        : value;
            }
            return users
                .Where(user => !user.Anonymous())
                .Where(user => !user.Disabled)
                .Select(user => $"[User{user.Id}]")
                .Join();
        }

        public static MailboxAddress SetEncoding(this MailboxAddress address, System.Text.Encoding encoding)
        {
            address.Encoding = encoding;
            return address;
        }
    }
}