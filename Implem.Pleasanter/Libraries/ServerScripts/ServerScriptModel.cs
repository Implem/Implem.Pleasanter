using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModel : IDisposable
    {
        public readonly ExpandoObject Model = new ExpandoObject();
        public readonly ExpandoObject Columns = new ExpandoObject();
        public readonly ServerScriptModelContext Context;
        public readonly ServerScriptModelSiteSettings SiteSettings;
        public readonly ServerScriptModelView View = new ServerScriptModelView();
        public readonly ServerScriptModelApiItems Items;
        private readonly List<string> ChangeItemNames = new List<string>();

        public ServerScriptModel(
            Context context,
            SiteSettings ss,
            IEnumerable<(string Name, object Value)> data,
            IEnumerable<(string Name, ServerScriptModelColumn Value)> columns,
            IEnumerable<KeyValuePair<string, string>> columnFilterHach,
            IEnumerable<KeyValuePair<string, SqlOrderBy.Types>> columnSorterHach,
            bool onTesting)
        {
            data?.ForEach(datam => ((IDictionary<string, object>)Model)[datam.Name] = datam.Value);
            columns?.ForEach(
                datam => ((IDictionary<string, object>)Columns)[datam.Name] = datam.Value);
            columnFilterHach?.ForEach(columnFilter =>
                ((IDictionary<string, object>)View.Filters)[columnFilter.Key] = columnFilter.Value);
            columnSorterHach?.ForEach(columnSorter =>
                ((IDictionary<string, object>)View.Sorters)[columnSorter.Key] = Enum.GetName(
                    typeof(SqlOrderBy.Types),
                    columnSorter.Value));
            ((INotifyPropertyChanged)Model).PropertyChanged += DataPropertyChanged;
            Context = new ServerScriptModelContext(
                formStringRaw: context.FormStringRaw,
                formString: context.FormString,
                ajax: context.Ajax,
                mobile: context.Mobile,
                applicationPath: context.ApplicationPath,
                absoluteUri: context.AbsoluteUri,
                absolutePath: context.AbsolutePath,
                url: context.Url,
                urlReferrer: context.UrlReferrer,
                controller: context.Controller,
                query: context.Query,
                action: context.Action,
                tenantId: context.TenantId,
                siteId: context.SiteId,
                id: context.Id,
                groupIds: context.Groups,
                tenantTitle: context.TenantTitle,
                siteTitle: context.SiteTitle,
                recordTitle: context.RecordTitle,
                deptId: context.DeptId,
                userId: context.UserId,
                loginId: context.LoginId,
                language: context.Language,
                timeZoneInfo: context.TimeZoneInfo.ToString(),
                hasPrivilege: context.HasPrivilege,
                apiVersion: context.ApiVersion,
                apiRequestBody: context.ApiRequestBody,
                requestDataString: context.RequestDataString,
                contentType: context.ContentType,
                onTesting: onTesting);
            SiteSettings = new ServerScriptModelSiteSettings
            {
                DefaultViewId = ss?.GridView
            };
            Items = new ServerScriptModelApiItems(
                context: context,
                onTesting: onTesting);
        }

        private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ChangeItemNames.Add(e.PropertyName);
        }

        public IReadOnlyCollection<string> GetChangeItemNames()
        {
            return ChangeItemNames.ToArray();
        }

        public void Dispose()
        {
            ((INotifyPropertyChanged)Model).PropertyChanged -= DataPropertyChanged;
        }

        public class ServerScriptModelContext
        {
            public readonly ServerScriptModelContextServerScript ServerScript;
            public readonly string FormStringRaw;
            public readonly string FormString;
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

            public ServerScriptModelContext(
                string formStringRaw,
                string formString,
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
                bool onTesting)
            {
                ServerScript = new ServerScriptModelContextServerScript(onTesting: onTesting);
                FormStringRaw = formStringRaw;
                FormString = formString;
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
                Groups = groupIds?.ToArray() ?? new int[0];
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
            }
        }

        public class ServerScriptModelContextServerScript
        {
            public readonly bool OnTesting;

            public ServerScriptModelContextServerScript(bool onTesting)
            {
                OnTesting = onTesting;
            }
        }

        public class ServerScriptModelColumn
        {
            public bool ReadOnly { get; set; }
            public string ExtendedCellCss { get; set; }
        }

        public class ServerScriptModelRow
        {
            public string ExtendedRowCss { get; set; }
            public Dictionary<string, ServerScriptModelColumn> Columns { get; set; }
        }

        public class ServerScriptModelView
        {
            public readonly ExpandoObject Filters = new ExpandoObject();
            public readonly ExpandoObject Sorters = new ExpandoObject();
        }

        public class ServerScriptModelSiteSettings
        {
            public int? DefaultViewId { get; set; }
        }

        public class ServerScriptModelApiItems
        {
            private readonly Context Context;
            private readonly bool OnTesting;

            public ServerScriptModelApiItems(Context context, bool onTesting)
            {
                Context = context;
                OnTesting = onTesting;
            }

            public ServerScriptModelApiModel[] Get(long id, string view = null)
            {
                if (OnTesting)
                {
                    return new ServerScriptModelApiModel[0];
                }
                return ServerScriptUtilities.Get(
                    context: Context,
                    id: id,
                    view: view,
                    onTesting: OnTesting);
            }

            public ServerScriptModelApiModel New()
            {
                var itemModel = new IssueModel();
                var apiContext = ServerScriptUtilities.CreateContext(
                    context: Context,
                    id: 0,
                    apiRequestBody: string.Empty);
                var ss = new SiteSettings(
                    context: apiContext,
                    referenceType: "Issues");
                itemModel.SetDefault(
                    context: apiContext,
                    ss: ss);
                var apiModel = new ServerScriptModelApiModel(
                    context: Context,
                    model: itemModel,
                    onTesting: OnTesting);
                return apiModel;
            }

            public bool Create(long id, object model)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Create(
                    context: Context,
                    id: id,
                    model: model);
            }

            public bool Update(long id, object model)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Update(
                    context: Context,
                    id: id,
                    model: model);
            }

            public bool Delete(long id)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Delete(
                    context: Context,
                    id: id);
            }

            public long BulkDelete(long id, string json)
            {
                if (OnTesting)
                {
                    return 0;
                }
                return ServerScriptUtilities.BulkDelete(
                    context: Context,
                    id: id,
                    apiRequestBody: json);
            }
        }
    }
}