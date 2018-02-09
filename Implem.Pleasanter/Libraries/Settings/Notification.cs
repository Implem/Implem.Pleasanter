using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
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
            ChatWork = 3
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
            List<string> monitorChangesColumns,
            int beforeCondition = 0,
            int afterCondition = 0,
            Expressions expression = Expressions.Or)
        {
            Type = type;
            Prefix = prefix;
            Address = address;
            Token = token;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
        }

        public void Send(string title, string url, string body)
        {
            var from = MailAddressUtilities.Get(Sessions.UserId(), withFullName: true);
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
                        }.Send();
                    }
                    break;
                case Types.Slack:
                    if (Parameters.Notification.Slack)
                    {
                        new Slack(
                            "*{0}{1}*\n{2}\n{3}".Params(Prefix, title, url, body),
                            from)
                                .Send(Address);
                    }
                    break;
                case Types.ChatWork:
                    if (Parameters.Notification.ChatWork)
                    {
                        new ChatWork(
                            "*{0}{1}*\n{2}\n{3}".Params(Prefix, title, url, body),
                            from,
                            Token)
                                .Send(Address);
                    }
                    break;
                default:
                    break;
            }
        }

        public IEnumerable<Column> ColumnCollection(SiteSettings ss, bool update)
        {
            return (update
                ? MonitorChangesColumns
                : ss.EditorColumns)?
                    .Select(o => ss.GetColumn(o))
                    .Where(o => o != null)
                    .ToList();
        }

        public bool HasRelatedUsers()
        {
            return Type == Types.Mail && Address?.Contains("[RelatedUsers]") == true;
        }

        public void ReplaceRelatedUsers(IEnumerable<long> users)
        {
            Address = Address.Replace(
                "[RelatedUsers]",
                Rds.ExecuteTable(statements: Rds.SelectMailAddresses(
                    column: Rds.MailAddressesColumn()
                        .OwnerId()
                        .MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId_In(users.Distinct())))
                            .AsEnumerable()
                            .GroupBy(o => o["OwnerId"])
                            .Select(o => MailAddressUtilities.Get(
                                SiteInfo.UserName(o.First()["OwnerId"].ToInt()),
                                o.First()["MailAddress"].ToString()))
                            .Join(";"));
        }

        public Notification GetRecordingData()
        {
            var notification = new Notification();
            notification.Id = Id;
            notification.Type = Type;
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