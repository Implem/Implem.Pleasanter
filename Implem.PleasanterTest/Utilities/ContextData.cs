using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using static Implem.PleasanterTest.Utilities.UserData;

namespace Implem.PleasanterTest.Utilities
{
    public static class ContextData
    {
        public static Context Get(
            int userId = 0,
            UserTypes userType = UserTypes.General1,
            bool hasRoute = true,
            string formStringRaw = "",
            string formString = "",
            string httpMethod = "GET",
            bool ajax = false,
            bool mobile = false,
            Dictionary<string, string> routeData = null,
            string server = "http://localhost:59802",
            string applicationPath = "/",
            string absoluteUri = "http://localhost:59802/",
            string absolutePath = "",
            string url = "",
            string urlReferrer = "",
            string query = "",
            string userHostName = "::1",
            string userHostAddress = "::1",
            string userAgent = "Implem.PleasanterTest",
            string userTimeZone = null,
            QueryStrings queryStrings = null,
            Forms forms = null,
            decimal? apiVersion = null,
            string apiRequestBody = null,
            string apiKey = null,
            string fileName = null,
            string contentType = null,
            bool setItemProperties = true,
            bool setPermissions = true,
            bool setPublish = true)
        {
            var userModel = userId > 0
                ? UserData.Get(userId: userId)
                : UserData.Get(userType: userType);
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            if (userModel != null)
            {
                context.Authenticated = !userModel.Disabled && !userModel.Lockout;
                context.TenantId = userModel.TenantId;
                context.LoginId = userModel.LoginId;
                context.DeptId = userModel.DeptId;
                context.UserId = userModel.UserId;
                context.Dept = SiteInfo.Dept(
                    tenantId: context.TenantId,
                    deptId: context.DeptId);
                context.User = SiteInfo.User(
                    context: context,
                    userId: context.UserId);
                context.Language = userModel.Language;
                context.UserTheme = userModel.Theme;
                context.Developer = userModel.Developer;
                context.TimeZoneInfo = userModel.TimeZoneInfo;
                context.UserSettings = userModel.UserSettings;
                context.HasPrivilege = Permissions.PrivilegedUsers(userModel.LoginId);
            }
            context.HasRoute = hasRoute;
            context.FormStringRaw = formStringRaw;
            context.FormString = formString;
            context.HttpMethod = httpMethod;
            context.Ajax = ajax;
            context.Mobile = mobile;
            context.RouteData = routeData;
            context.Server = server;
            context.ApplicationPath = applicationPath;
            context.AbsoluteUri = absoluteUri;
            context.AbsolutePath = absolutePath;
            context.Url = url;
            context.UrlReferrer = urlReferrer;
            context.Query = query;
            context.Controller = context.RouteData.Get("controller")?.ToLower() ?? string.Empty;
            context.Action = context.RouteData.Get("action")?.ToLower() ?? string.Empty;
            context.Id = context.RouteData.Get("id")?.ToLong() ?? 0;
            context.Guid = context.RouteData.Get("guid")?.ToUpper();
            context.UserHostName = userHostName;
            context.UserHostAddress = userHostAddress;
            context.UserAgent = userAgent;
            context.QueryStrings = queryStrings ?? new QueryStrings();
            context.Forms = forms ?? new Forms();
            context.TimeZoneInfo = userTimeZone.IsNullOrEmpty()
                ? null
                : TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
            if (apiVersion != null)
            {
                context.ApiVersion = apiVersion.ToDecimal();
            }
            context.ApiRequestBody = apiRequestBody;
            context.ApiKey = apiKey;
            if (fileName != null)
            {
                var postedFile = Files.Bytes(Path.Combine(
                    Directories.PleasanterTest(),
                    "BinaryData",
                    fileName));
                context.PostedFiles.Add(new PostedFile()
                {
                    Guid = postedFile.WriteToTemp(fileName: fileName),
                    FileName = fileName,
                    Extension = Path.GetExtension(fileName),
                    Size = postedFile.Length,
                    ContentType = contentType,
                    ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(
                        0,
                        postedFile.Length - 1,
                        postedFile.Length),
                    InputStream = new MemoryStream(postedFile)
                });
            }
            if (setItemProperties) context.SetItemProperties();
            if (setPermissions) context.SetPermissions();
            if (setPublish) context.SetPublish();
            return context;
        }
    }
}
