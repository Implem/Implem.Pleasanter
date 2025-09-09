using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelContext
    {
        public StringBuilder LogBuilder;
        public ExpandoObject UserData;
        public ResponseCollection ResponseCollection;
        public List<Message> Messages;
        public ErrorData ErrorData;
        public RedirectData RedirectData;
        public readonly ServerScriptModelContextServerScript ServerScript;
        public readonly QueryStrings QueryStrings;
        public readonly Forms Forms;
        public readonly string FormStringRaw;
        public readonly string FormString;
        public readonly string HttpMethod;
        public readonly bool Ajax;
        public readonly bool Mobile;
        public readonly string ApplicationPath;
        public readonly string AbsoluteUri;
        public readonly string AbsolutePath;
        public readonly string Url;
        public readonly string UrlReferrer;
        public readonly string Controller;
        public readonly string Query;
        public readonly string Action;
        public readonly int TenantId;
        public readonly long SiteId;
        public readonly long Id;
        public readonly IList<int> Groups;
        public readonly string TenantTitle;
        public readonly string SiteTitle;
        public readonly string RecordTitle;
        public readonly int DeptId;
        public readonly int UserId;
        public readonly string LoginId;
        public readonly string Language;
        public readonly string TimeZoneInfo;
        public readonly bool HasPrivilege;
        public readonly decimal ApiVersion;
        public readonly string ApiRequestBody;
        public readonly string RequestDataString;
        public readonly string ContentType;
        public readonly string ControlId;
        public readonly string Condition;
        public readonly string ReferenceType;

        public ServerScriptModelContext(
            Context context,
            StringBuilder logBuilder,
            ExpandoObject userData,
            ResponseCollection responseCollection,
            List<Message> messages,
            ErrorData errorData,
            RedirectData redirectData,
            string formStringRaw,
            string formString,
            string httpMethod,
            bool ajax,
            bool mobile,
            string applicationPath,
            string absoluteUri,
            string absolutePath,
            string url,
            string urlReferrer,
            string controller,
            string query,
            string action,
            int tenantId,
            long siteId,
            long id,
            IEnumerable<int> groupIds,
            string tenantTitle,
            string siteTitle,
            string recordTitle,
            int deptId,
            int userId,
            string loginId,
            string language,
            string timeZoneInfo,
            bool hasPrivilege,
            decimal apiVersion,
            string apiRequestBody,
            string requestDataString,
            string contentType,
            bool onTesting,
            long scriptDepth,
            string controlId,
            string condition,
            string referenceType)
        {
            LogBuilder = logBuilder;
            UserData = userData;
            ResponseCollection = responseCollection;
            Messages = messages;
            ErrorData = errorData;
            RedirectData = redirectData;
            ServerScript = new ServerScriptModelContextServerScript(
                onTesting: onTesting,
                scriptDepth: scriptDepth);
            QueryStrings = context.QueryStrings;
            Forms = context.Forms;
            FormStringRaw = formStringRaw;
            FormString = formString;
            HttpMethod = httpMethod;
            Ajax = ajax;
            Mobile = mobile;
            ApplicationPath = applicationPath;
            AbsoluteUri = absoluteUri;
            AbsolutePath = absolutePath;
            Url = url;
            UrlReferrer = urlReferrer;
            Controller = controller;
            Query = query;
            Action = action;
            TenantId = tenantId;
            SiteId = siteId;
            Id = id;
            Groups = groupIds?.Distinct()?.ToArray() ?? Array.Empty<int>();
            TenantTitle = tenantTitle;
            SiteTitle = siteTitle;
            RecordTitle = recordTitle;
            DeptId = deptId;
            UserId = userId;
            LoginId = loginId;
            Language = language;
            TimeZoneInfo = timeZoneInfo;
            HasPrivilege = hasPrivilege;
            ApiVersion = apiVersion;
            ApiRequestBody = apiRequestBody;
            RequestDataString = requestDataString;
            ContentType = contentType;
            ControlId = controlId;
            Condition = condition;
            ReferenceType = referenceType;
        }

        public void Log(object log)
        {
            LogBuilder.AppendLine(log?.ToString() ?? string.Empty);
        }

        public void Error(string message)
        {
            ErrorData.Type = General.Error.Types.CustomError;
            ErrorData.Data = message.ToSingleArray();
        }

        public void AddMessage(string text, string css = "alert-information")
        {
            Messages.Add(new Message()
            {
                Text = text,
                Css = css
            });
        }

        public void Redirect(string url)
        {
            RedirectData.Url = url;
        }

        public void AddResponse(
            string method,
            string target = null,
            object value = null,
            string options = null)
        {
            ResponseCollection.Add(
                method: method,
                target: target,
                value: value,
                options: options);
        }

        public void ResponseSet(
            string target = null,
            object value = null,
            string options = null)
        {
            AddResponse(
                method: "Set",
                target: target,
                value: value,
                options: options);
        }
    }
}