using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class Notification : ISettingListItem
    {
        public int Id { get; set; }
        public Types Type;
        public string Prefix;
        public string Address;
        public string Token;
        public bool? IsGroup;
        public List<string> MonitorChangesColumns;
        public int BeforeCondition;
        public int AfterCondition;
        public Expressions Expression;
        [NonSerialized]
        public bool Enabled = true;

        public enum Types : int
        {
            Mail = 1,
            Slack = 2,
            ChatWork = 3,
            LineBot = 4
        }

        public enum Expressions : int
        {
            Or = 1,
            And = 2
        }

        public Notification()
        {
        }

        public Notification(Types type, List<string> monitorChangesColumns)
        {
            Type = type;
            MonitorChangesColumns = monitorChangesColumns;
        }

        public Notification(
            int id,
            Types type,
            string prefix,
            string address,
            string token,
            bool? isGroup,
            List<string> monitorChangesColumns,
            int beforeCondition = 0,
            int afterCondition = 0,
            Expressions expression = Expressions.Or)
        {
            Id = id;
            Type = type;
            Prefix = prefix;
            Address = address;
            Token = token;
            IsGroup = isGroup;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public void Update(
            Types type,
            string prefix,
            string address,
            string token,
            bool? isGroup,
            List<string> monitorChangesColumns,
            int beforeCondition = 0,
            int afterCondition = 0,
            Expressions expression = Expressions.Or)
        {
            Type = type;
            Prefix = prefix;
            Address = address;
            Token = token;
            IsGroup = isGroup;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
        }

        public void Send(Context context, SiteSettings ss, string title, string url, string body)
        {
            var from = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId,
                withFullName: true);
            switch (Type)
            {
                case Types.Mail:
                    if (Parameters.Notification.Mail)
                    {
                        var mailFrom = new System.Net.Mail.MailAddress(
                            Addresses.BadAddress(from) == string.Empty
                                ? from
                                : Parameters.Mail.SupportFrom);
                        new OutgoingMailModel()
                        {
                            Title = new Title(Prefix + title),
                            Body = "{0}\r\n{1}".Params(url, body) + (Addresses.FixedFrom(mailFrom)
                                ? "\r\n\r\n{0}<{1}>".Params(mailFrom.DisplayName, mailFrom.Address)
                                : string.Empty),
                            From = mailFrom,
                            To = Address
                        }.Send(context: context, ss: ss);
                    }
                    break;
                case Types.Slack:
                    if (Parameters.Notification.Slack)
                    {
                        new Slack(context,
                            "*{0}{1}*\n{2}\n{3}".Params(Prefix, title, url, body),
                            from)
                                .Send(Address);
                    }
                    break;
                case Types.ChatWork:
                    if (Parameters.Notification.ChatWork)
                    {
                        new ChatWork(context,
                            "*{0}{1}*\n{2}\n{3}".Params(Prefix, title, url, body),
                            from,
                            Token)
                                .Send(Address);
                    }
                    break;
                case Types.LineBot:
                    if (Parameters.Notification.LineBot)
                    {
                        new LineBot(context,
                            "*{0}{1}*\n{2}\n{3}".Params(Prefix, title, url, body),
                            from,
                            Parameters.Notification.LineChannelAccessToken)
                                .Send(Address, IsGroup == true);
                    }
                    break;
                default:
                    break;
            }
        }

        public IEnumerable<Column> ColumnCollection(Context context, SiteSettings ss, bool update)
        {
            return (update
                ? MonitorChangesColumns
                : ss.EditorColumns)?
                    .Select(columnName => ss.GetColumn(
                        context: context,
                        columnName: columnName))
                    .Where(o => o != null)
                    .ToList();
        }

        public bool HasRelatedUsers()
        {
            return Type == Types.Mail && Address?.Contains("[RelatedUsers]") == true;
        }

        public void ReplaceRelatedUsers(Context context, IEnumerable<long> users)
        {
            Address = Address.Replace(
                "[RelatedUsers]",
                Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn()
                            .OwnerId()
                            .MailAddress(),
                        where: Rds.MailAddressesWhere()
                            .OwnerId_In(users.Distinct())))
                                .AsEnumerable()
                                .GroupBy(o => o["OwnerId"])
                                .Select(o => MailAddressUtilities.Get(
                                    SiteInfo.UserName(
                                        context: context,
                                        userId: o.First().Int("OwnerId")),
                                    o.First()["MailAddress"].ToString()))
                                .Join(";"));
        }

        public Notification GetRecordingData()
        {
            var notification = new Notification
            {
                Id = Id,
                Type = Type
            };
            if (!Prefix.IsNullOrEmpty())
            {
                notification.Prefix = Prefix;
            }
            if (!Address.IsNullOrEmpty())
            {
                notification.Address = Address;
            }
            if (!Token.IsNullOrEmpty())
            {
                notification.Token = Token;
            }
            if (IsGroup == true)
            {
                notification.IsGroup = IsGroup;
            }
            if (MonitorChangesColumns?.Any() == true)
            {
                notification.MonitorChangesColumns = MonitorChangesColumns;
            }
            notification.BeforeCondition = BeforeCondition;
            notification.AfterCondition = AfterCondition;
            notification.Expression = Expression;
            return notification;
        }
    }
}