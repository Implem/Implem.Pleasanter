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
        public string Subject;
        public string Address;
        public string CcAddress;
        public string BccAddress;
        public string Token;
        public MethodTypes? MethodType;
        public string Encoding;
        public string MediaType = "application/json";
        public string Headers;
        public bool? UseCustomFormat;
        public string Format;
        public string Body;
        public List<string> MonitorChangesColumns;
        public int BeforeCondition;
        public int AfterCondition;
        public Expressions Expression;
        public bool? AfterCreate;
        public bool? AfterUpdate;
        public bool? AfterDelete;
        public bool? AfterCopy;
        public bool? AfterBulkUpdate;
        public bool? AfterBulkDelete;
        public bool? AfterImport;
        public bool? Disabled;
        [NonSerialized]
        public int Index;
        public int? Delete;

        public enum Types : int
        {
            Mail = 1,
            Slack = 2,
            ChatWork = 3,
            Line = 4,
            LineGroup = 5,
            Teams = 6,
            RocketChat = 7,
            InCircle = 8,
            HttpClient = 9
        }

        public enum Expressions : int
        {
            Or = 1,
            And = 2
        }

        public enum MethodTypes : int
        {
            Get = 1,
            Post = 2,
            Put = 3,
            Delete = 4
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
            string subject,
            string address,
            string ccaddress,
            string bccaddress,
            string token,
            MethodTypes methodType,
            string encoding,
            string mediaType,
            string headers,
            bool? useCustomFormat,
            string format,
            List<string> monitorChangesColumns,
            int beforeCondition,
            int afterCondition,
            Expressions expression,
            bool afterCreate,
            bool afterUpdate,
            bool afterDelete,
            bool afterCopy,
            bool afterBulkUpdate,
            bool afterBulkDelete,
            bool afterImport,
            bool disabled)
        {
            Id = id;
            Type = type;
            Prefix = prefix;
            Subject = subject;
            Address = address;
            CcAddress = ccaddress;
            BccAddress = bccaddress;
            Token = token;
            MethodType = methodType;
            Encoding = encoding;
            MediaType = mediaType;
            Headers = headers;
            UseCustomFormat = useCustomFormat;
            Format = format;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
            AfterCreate = afterCreate;
            AfterUpdate = afterUpdate;
            AfterDelete = afterDelete;
            AfterCopy = afterCopy;
            AfterBulkUpdate = afterBulkUpdate;
            AfterBulkDelete = afterBulkDelete;
            AfterImport = afterImport;
            Disabled = disabled;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            MethodType = MethodType ?? MethodTypes.Get;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public void Update(
            Types type,
            string prefix,
            string subject,
            string address,
            string ccaddress,
            string bccaddress,
            string token,
            MethodTypes methodType,
            string encoding,
            string mediaType,
            string headers,
            bool? useCustomFormat,
            string format,
            List<string> monitorChangesColumns,
            int beforeCondition,
            int afterCondition,
            Expressions expression,
            bool afterCreate,
            bool afterUpdate,
            bool afterDelete,
            bool afterCopy,
            bool afterBulkUpdate,
            bool afterBulkDelete,
            bool afterImport,
            bool disabled)
        {
            Type = type;
            Prefix = prefix;
            Subject = subject;
            Address = address;
            CcAddress = ccaddress;
            BccAddress = bccaddress;
            Token = token;
            MethodType = methodType;
            Encoding = encoding;
            MediaType = mediaType;
            Headers = headers;
            UseCustomFormat = useCustomFormat;
            Format = format;
            MonitorChangesColumns = monitorChangesColumns;
            BeforeCondition = beforeCondition;
            AfterCondition = afterCondition;
            Expression = expression;
            AfterCreate = afterCreate;
            AfterUpdate = afterUpdate;
            AfterDelete = afterDelete;
            AfterCopy = afterCopy;
            AfterBulkUpdate = afterBulkUpdate;
            AfterBulkDelete = afterBulkDelete;
            AfterImport = afterImport;
            Disabled = disabled;
        }

        public void Send(
            Context context,
            SiteSettings ss,
            string title,
            string body,
            Dictionary<Column, string> values = null)
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
                        var mailFrom = MimeKit.MailboxAddress.Parse(
                            Addresses.BadAddress(addresses: from) == string.Empty
                                ? from
                                : Parameters.Mail.SupportFrom);
                        var addresses = Address;
                        var ccAdd = CcAddress;
                        var bccAdd = BccAddress;
                        values?.ForEach(data => addresses = addresses.Replace(
                            $"[{data.Key.ColumnName}]",
                            Addresses.ReplacedAddress(
                                context: context,
                                column: data.Key,
                                value: data.Value)));
                        values?.ForEach(data => ccAdd = ccAdd.Replace(
                            $"[{data.Key.ColumnName}]",
                            Addresses.ReplacedAddress(
                                context: context,
                                column: data.Key,
                                value: data.Value)));
                        values?.ForEach(data => bccAdd = bccAdd.Replace(
                            $"[{data.Key.ColumnName}]",
                            Addresses.ReplacedAddress(
                                context: context,
                                column: data.Key,
                                value: data.Value)));
                        var to = Addresses.Get( 
                            context: context,
                            addresses: addresses).Join(",");
                        var cc = Addresses.Get(
                            context: context,
                            addresses: ccAdd).Join(",");
                        var bcc = Addresses.Get(
                            context: context,
                            addresses: bccAdd).Join(",");

                        if (!to.IsNullOrEmpty())
                        {
                            new OutgoingMailModel()
                            {
                                Title = new Title(Prefix + title),
                                Body = body,
                                From = mailFrom,
                                To = to,
                                Cc = cc,
                                Bcc = bcc
                            }.Send(context: context, ss: ss);
                        }
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
                case Types.RocketChat:
                    if (Parameters.Notification.RocketChat)
                    {
                        new RocketChat(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}",
                            _username: from)
                                .Send(Address);
                    }
                    break;
                case Types.InCircle:
                    if (Parameters.Notification.InCircle)
                    {
                        new InCircle(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}",
                            _username: from,
                            _token: Token)
                                .Send(Address);
                    }
                    break;
                case Types.HttpClient:
                    if (Parameters.Notification.HttpClient)
                    {
                        new HttpClient(
                            _context: context,
                            _text: $"*{Prefix}{title}*\n{body}")
                        {
                            MethodType = MethodType,
                            Encoding = Encoding,
                            MediaType = MediaType,
                            Headers = Headers
                        }
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
                : ss.GetEditorColumnNames())?
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
                                    userId: userId).Id != SiteInfo.AnonymousId)
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
            if (!Subject.IsNullOrEmpty())
            {
                notification.Subject = Subject;
            }
            if (AfterCreate == false)
            {
                notification.AfterCreate = AfterCreate;
            }
            if (AfterUpdate == false)
            {
                notification.AfterUpdate = AfterUpdate;
            }
            if (AfterDelete == false)
            {
                notification.AfterDelete = AfterDelete;
            }
            if (AfterCopy == false)
            {
                notification.AfterCopy = AfterCopy;
            }
            if (AfterBulkUpdate == false)
            {
                notification.AfterBulkUpdate = AfterBulkUpdate;
            }
            if (AfterBulkDelete == false)
            {
                notification.AfterBulkDelete = AfterBulkDelete;
            }
            if (AfterImport == false)
            {
                notification.AfterImport = AfterImport;
            }
            if (Disabled == true)
            {
                notification.Disabled = Disabled;
            }
            if (!Address.IsNullOrEmpty())
            {
                notification.Address = Address;
            }
            if (!CcAddress.IsNullOrEmpty())
            {
                notification.CcAddress = CcAddress;
            }
            if (!BccAddress.IsNullOrEmpty())
            {
                notification.BccAddress = BccAddress;
            }
            if (MethodType != MethodTypes.Get)
            {
                notification.MethodType = MethodType;
            }
            if (!MediaType.IsNullOrEmpty())
            {
                notification.MediaType = MediaType;
            }
            if (!Encoding.IsNullOrEmpty())
            {
                notification.Encoding = Encoding;
            }
            if (!Headers.IsNullOrEmpty())
            {
                notification.Headers = Headers;
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
            if (!Body.IsNullOrEmpty())
            {
                notification.Body = Body;
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