using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class SendGridMail
    {
        private static readonly System.Net.Http.HttpClient _httpClient;
        private const string SendGridApiUrl = "https://api.sendgrid.com/v3/mail/send";
        public MailboxAddress From;
        public string To;
        public string Cc;
        public string Bcc;
        public string Subject;
        public string Body;
        public Context Context;

        static SendGridMail()
        {
            _httpClient = new System.Net.Http.HttpClient();
        }

        public SendGridMail(
            Context context,
            MailboxAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            var siteFrom = Addresses.From(from);
            Context = context;
            From = siteFrom;
            To = Strings.CoalesceEmpty(
                to,
                Parameters.Mail.FixedFrom,
                Parameters.Mail.SupportFrom);
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public async Task SendAsync(Context context, Attachments attachments = null)
        {
            try
            {
                var existAddresses = new List<string>();
                var toList = new List<object>();
                Addresses.Get(
                    context: context,
                    addresses: To)
                        .ForEach(to =>
                        {
                            var mailAddress = MailboxAddress.Parse(to);
                            existAddresses.Add(mailAddress.Address);
                            toList.Add(new
                            {
                                email = mailAddress.Address,
                                name = mailAddress.Name
                            });
                        });
                var ccList = new List<object>();
                Addresses.Get(
                    context: context,
                    addresses: Cc)
                        .ForEach(cc =>
                        {
                            var mailAddress = MailboxAddress.Parse(cc);
                            if (!existAddresses.Contains(mailAddress.Address))
                            {
                                existAddresses.Add(mailAddress.Address);
                                ccList.Add(new
                                {
                                    email = mailAddress.Address,
                                    name = mailAddress.Name
                                });
                            }
                        });
                var bccList = new List<object>();
                Addresses.Get(
                    context: context,
                    addresses: Bcc)
                        .ForEach(bcc =>
                        {
                            var mailAddress = MailboxAddress.Parse(bcc);
                            if (!existAddresses.Contains(mailAddress.Address))
                            {
                                existAddresses.Add(mailAddress.Address);
                                bccList.Add(new
                                {
                                    email = mailAddress.Address,
                                    name = mailAddress.Name
                                });
                            }
                        });
                if (!toList.Any())
                {
                    new SysLogModel(
                        context: Context,
                        method: nameof(SendAsync),
                        message: "SendGrid: No recipients in the To field.",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                    return;
                }
                var personalization = new Dictionary<string, object>();
                personalization["to"] = toList;
                if (ccList.Any())
                {
                    personalization["cc"] = ccList;
                }
                if (bccList.Any())
                {
                    personalization["bcc"] = bccList;
                }
                var payload = new Dictionary<string, object>
                {
                    ["personalizations"] = new[] { personalization },
                    ["from"] = new
                    {
                        email = From.Address,
                        name = From.Name
                    },
                    ["subject"] = Subject,
                    ["content"] = new[]
                    {
                        new
                        {
                            type = "text/plain",
                            value = Body
                        }
                    }
                };
                var attachmentList = attachments
                    ?.Where(attachment =>
                        attachment?.Base64?.IsNullOrEmpty() == false)
                    .Select(attachment => new
                    {
                        content = attachment.Base64,
                        filename = Strings.CoalesceEmpty(
                            attachment.Name,
                            "NoName"),
                        type = attachment.ContentType,
                        disposition = "attachment"
                    })
                    .ToList();
                if (attachmentList?.Any() == true)
                {
                    payload["attachments"] = attachmentList;
                }
                var json = JsonConvert.SerializeObject(
                    payload,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                using var request = new System.Net.Http.HttpRequestMessage(
                    System.Net.Http.HttpMethod.Post,
                    SendGridApiUrl);
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        Parameters.Mail.SmtpPassword);
                request.Content = new System.Net.Http.StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");
                using var response = await _httpClient
                    .SendAsync(request)
                    .ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);
                    _ = new SysLogModel(
                        context: Context,
                        method: nameof(SendAsync),
                        message: $"SendGrid API error: {(int)response.StatusCode} {response.ReasonPhrase} - {responseBody}",
                        sysLogType: SysLogModel.SysLogTypes.SystemError);
                }
            }
            catch (Exception e)
            {
                _ = new SysLogModel(Context, e);
            }
        }
    }
}