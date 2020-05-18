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
        public bool? UseCustomFormat;
        public string Format;
        public List<string> MonitorChangesColumns;
        public int BeforeCondition;
        public int AfterCondition;
        public Expressions Expression;
        public bool? Disabled;
        [NonSerialized]
        public int Index;

        public enum Types : int
        {
            Mail = 1,
            Slack = 2,
            ChatWork = 3,
            Line = 4,
            LineGroup = 5,
            Teams = 6
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
            bool? useCustomFormat,
            string format,
            List<string> monitorChangesColumns,
            int beforeCondition,
            int afterCondition,
            Expressions expression,
            bool disabled)
        {
            Id = id;
            Type = type;
            Prefix = prefix;
            Address = address;
            Token = token;
            UseCustomFormat = useCustomFormat;
            Format = format;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
            Disabled = disabled;
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
            bool? useCustomFormat,
            string format,
            List<string> monitorChangesColumns,
            int beforeCondition,
            int afterCondition,
            Expressions expression,
            bool disabled)
        {
            Type = type;
            Prefix = prefix;
            Address = address;
            Token = token;
            UseCustomFormat = useCustomFormat;
            Format = format;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
            Disabled = disabled;
        }

        public void Send(Context context, SiteSettings ss, string title, string body)
        {
            if (Disabled == true)
            {
                return;
            }
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
                            Addresses.BadAddress(
                                context: context,
                                addresses: from) == string.Empty
                                    ? from
                                    : Parameters.Mail.SupportFrom);
                        new OutgoingMailModel()
                        {
                            Title = new Title(Prefix + title),
                            Body = body,
                            From = mailFrom,
                            To = Addresses.GetEnumerable(
                                context: context,
                                addresses: Address).Join(",")
                        }.Send(context: context, ss: ss);
                    }
                    break;
                case Types.Slack:
                    if (Parameters.Notification.Slack)
                    {
                        new Slack(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}",
                            _username: from)
                                .Send(Address);
                    }
                    break;
                case Types.ChatWork:
                    if (Parameters.Notification.ChatWork)
                    {
                        new ChatWork(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}",
                            _username: from,
                            _token: Token)
                                .Send(Address);
                    }
                    break;
                case Types.Line:
                case Types.LineGroup:
                    if (Parameters.Notification.Line)
                    {
                        new Line(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}",
                            _username: from,
                            _token: Token)
                                .Send(Address, Type == Types.LineGroup);
                    }
                    break;
                case Types.Teams:
                    if (Parameters.Notification.Teams)
                    {
                        new Teams(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}")
                                .Send(Address);
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

        public void ReplaceRelatedUsers(Context context, List<int> users)
        {
            Address = Address.Replace(
                "[RelatedUsers]",
                Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn()
                            .OwnerId()
                            .MailAddress(),
                        where: Rds.MailAddressesWhere()
                            .OwnerId_In(users
                                .Distinct()
                                .Where(userId => SiteInfo.User(
                                    context: context,
                                    userId: userId).Id != User.UserTypes.Anonymous.ToInt())
                                .Select(userId => userId.ToLong()))
                            .OwnerType("Users")))
                                .AsEnumerable()
                                .GroupBy(o => o["OwnerId"])
                                .Select(o => MailAddressUtilities.Get(
                                    SiteInfo.UserName(
                                        context: context,
                                        userId: o.First().Int("OwnerId")),
                                    o.First()["MailAddress"].ToString()))
                                .Join(";"));
        }

        public Notification GetRecordingData(Context context, SiteSettings ss)
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
            if (Disabled == true)
            {
                notification.Disabled = Disabled;
            }
            if (!Address.IsNullOrEmpty())
            {
                notification.Address = Address;
            }
            if (!Token.IsNullOrEmpty())
            {
                notification.Token = Token;
            }
            if (UseCustomFormat == true)
            {
                notification.UseCustomFormat = UseCustomFormat;
                if (GetDefaultFormat(
                    context: context,
                    ss: ss) != Format)
                {
                    notification.Format = Format;
                }
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

        public string GetFormat(Context context, SiteSettings ss)
        {
            return Strings.CoalesceEmpty(Format, GetDefaultFormat(
                context: context,
                ss: ss));
        }

        public string GetDefaultFormat(Context context, SiteSettings ss)
        {
            return "{Url}\n"
                + ColumnCollection(
                    context: context,
                    ss: ss,
                    update: true)
                        .Select(o => new NotificationColumnFormat(
                            columnName: o.ColumnName).ToJson())
                        .Join("\n")
                + "\n\n{UserName}<{MailAddress}>";
        }
    }
}