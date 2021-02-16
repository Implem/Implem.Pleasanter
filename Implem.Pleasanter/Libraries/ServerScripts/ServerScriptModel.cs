using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Collections;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModel : IDisposable
    {
        public readonly ExpandoObject Model = new ExpandoObject();
        public readonly ServerScriptModelDepts Depts;
        public readonly ServerScriptModelGroups Groups;
        public readonly ServerScriptModelUsers Users;
        public readonly ExpandoObject Columns = new ExpandoObject();
        public readonly ServerScriptModelContext Context;
        public readonly ServerScriptModelSiteSettings SiteSettings;
        public readonly ServerScriptModelView View = new ServerScriptModelView();
        public readonly ServerScriptModelApiItems Items;
        public ServerScriptModelHidden Hidden;
        public ServerScriptModelExtendedSql ExtendedSql;
        public ServerScriptModelNotification Notification;
        private readonly List<string> ChangeItemNames = new List<string>();
        private DateTime TimeOut;

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
            Depts = new ServerScriptModelDepts(context: context);
            Groups = new ServerScriptModelGroups(context: context);
            Users = new ServerScriptModelUsers(context: context);
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
                onTesting: onTesting,
                scriptDepth: context.ServerScriptDepth,
                logBuilder: context.LogBuilder,
                userData: context.UserData,
                controlId: context.Forms.ControlId());
            SiteSettings = new ServerScriptModelSiteSettings
            {
                DefaultViewId = ss?.GridView,
                Sections = ss?.Sections
            };
            Items = new ServerScriptModelApiItems(
                context: context,
                onTesting: onTesting);
            TimeOut = Parameters.Script.ServerScriptTimeOut == 0
                ? DateTime.MaxValue
                : DateTime.Now.AddMilliseconds(Parameters.Script.ServerScriptTimeOut);
            Hidden = new ServerScriptModelHidden();
            ExtendedSql = new ServerScriptModelExtendedSql(
                context: context,
                onTesting: onTesting);
            Notification = new ServerScriptModelNotification(
                context: context,
                ss: ss);
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

        public bool ContinuationCallback()
        {
            return TimeOut > DateTime.Now;
        }

        public class ServerScriptModelDepts
        {
            private readonly Context Context;

            public ServerScriptModelDepts(Context context)
            {
                Context = context;
            }

            public ServerScriptModelDeptModel Get(object id)
            {
                var dept = SiteInfo.Dept(
                    tenantId: Context.TenantId,
                    deptId: id.ToInt());
                var deptModel = dept.Id > 0
                    ? new ServerScriptModelDeptModel(
                        context: Context,
                        tenantId: dept.TenantId,
                        deptId: dept.Id,
                        deptCode: dept.Code,
                        deptName: dept.Name)
                    : null;
                return deptModel;
            }
        }

        public class ServerScriptModelGroups
        {
            private readonly Context Context;

            public ServerScriptModelGroups(Context context)
            {
                Context = context;
            }

            public ServerScriptModelGroupModel Get(object id)
            {
                var group = SiteInfo.Group(
                    tenantId: Context.TenantId,
                    groupId: id.ToInt());
                var groupModel = group.Id > 0
                    ? new ServerScriptModelGroupModel(
                        context: Context,
                        tenantId: group.TenantId,
                        groupId: group.Id,
                        groupName: group.Name)
                    : null;
                return groupModel;
            }
        }

        public class ServerScriptModelUsers
        {
            private readonly Context Context;

            public ServerScriptModelUsers(Context context)
            {
                Context = context;
            }

            public ServerScriptModelUserModel Get(object id)
            {
                var user = SiteInfo.User(
                    context: Context,
                    userId: id.ToInt());
                var userModel = !user.Anonymous()
                    ? new ServerScriptModelUserModel(
                        context: Context,
                        tenantId: user.TenantId,
                        userId: user.Id,
                        deptId: user.DeptId,
                        loginId: user.LoginId,
                        name: user.Name,
                        userCode: user.UserCode,
                        tenantManager: user.TenantManager,
                        serviceManager: user.ServiceManager,
                        disabled: user.Disabled)
                    : null;
                return userModel;
            }
        }

        public class ServerScriptModelContext
        {
            public StringBuilder LogBuilder;
            public ExpandoObject UserData;
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
            public readonly string ControlId;

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
                bool onTesting,
                long scriptDepth,
                StringBuilder logBuilder,
                ExpandoObject userData,
                string controlId)
            {
                LogBuilder = logBuilder;
                UserData = userData; 
                ServerScript = new ServerScriptModelContextServerScript(
                    onTesting: onTesting,
                    scriptDepth: scriptDepth);
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
                ControlId = controlId;
            }

            public void Log(object log)
            {
                LogBuilder.AppendLine(log?.ToString() ?? string.Empty);
            }
        }

        public class ServerScriptModelContextServerScript
        {
            public readonly bool OnTesting;
            public readonly long ScriptDepth;

            public ServerScriptModelContextServerScript(
                bool onTesting,
                long scriptDepth)
            {
                OnTesting = onTesting;
                ScriptDepth = scriptDepth;
            }
        }

        public class ServerScriptModelColumn
        {
            public Dictionary<object, object> ChoiceHash { get; set; }
            public bool ReadOnly { get; set; }
            public string ExtendedFieldCss { get; set; }
            public string ExtendedCellCss { get; set; }
            public string ExtendedHtmlBeforeField { get; set; }
            public string ExtendedHtmlAfterField { get; set; }
            public bool Hide { get; set; }
            public string RawText { get; set; }

            public void AddChoiceHash(object key, object value)
            {
                if (ChoiceHash == null)
                {
                    ChoiceHash = new Dictionary<object, object>();
                }
                ChoiceHash.Add(key, value);
            }
        }

        public class ServerScriptModelSiteSettings
        {
            public int? DefaultViewId { get; set; }
            public List<Section> Sections { get; set; }
        }

        public class ServerScriptModelView
        {
            public readonly ExpandoObject Filters = new ExpandoObject();
            public readonly ExpandoObject Sorters = new ExpandoObject();
        }

        public class ServerScriptModelRow
        {
            public string ExtendedRowCss { get; set; }
            public Dictionary<string, ServerScriptModelColumn> Columns { get; set; }
            public Dictionary<string, string> Hidden { get; set; }
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

            public ServerScriptModelApiModel[] Get(object id, string view = null)
            {
                if (OnTesting)
                {
                    return new ServerScriptModelApiModel[0];
                }
                return ServerScriptUtilities.Get(
                    context: Context,
                    id: id.ToLong(),
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

            public bool Create(object id, object model)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Create(
                    context: Context,
                    id: id.ToLong(),
                    model: model);
            }

            public bool Update(object id, object model)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Update(
                    context: Context,
                    id: id.ToLong(),
                    model: model);
            }

            public bool Delete(object id)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Delete(
                    context: Context,
                    id: id.ToLong());
            }

            public long BulkDelete(object id, string json)
            {
                if (OnTesting)
                {
                    return 0;
                }
                return ServerScriptUtilities.BulkDelete(
                    context: Context,
                    id: id.ToLong(),
                    apiRequestBody: json);
            }

            public decimal Sum(object siteId, string columnName, string view = null)
            {
                return CreateAggregate(
                    siteId: siteId,
                    columnName: columnName,
                    view: view,
                    function: Sqls.Functions.Sum);
            }

            public decimal Average(object siteId, string columnName, string view = null)
            {
                return CreateAggregate(
                    siteId: siteId,
                    columnName: columnName,
                    view: view,
                    function: Sqls.Functions.Avg);
            }

            public decimal Max(object siteId, string columnName, string view = null)
            {
                return CreateAggregate(
                    siteId: siteId,
                    columnName: columnName,
                    view: view,
                    function: Sqls.Functions.Max);
            }

            public decimal Min(object siteId, string columnName, string view = null)
            {
                return CreateAggregate(
                    siteId: siteId,
                    columnName: columnName,
                    view: view,
                    function: Sqls.Functions.Min);
            }

            public long Count(object siteId, string view = null)
            {
                var ss = SiteSettingsUtilities.Get(
                    context: Context,
                    siteId: siteId.ToLong());
                return ServerScriptUtilities.Aggregate(
                    context: Context,
                    ss: ss,
                    view: view);
            }

            private decimal CreateAggregate(object siteId, string columnName, string view, Sqls.Functions function)
            {
                var ss = SiteSettingsUtilities.Get(
                    context: Context,
                    siteId: siteId.ToLong());
                return ServerScriptUtilities.Aggregate(
                    context: Context,
                    ss: ss,
                    view: view,
                    columnName: columnName,
                    function: function);
            }
        }

        public class ServerScriptModelHidden
        {
            private Dictionary<string, string> data;

            public ServerScriptModelHidden()
            {
                data = new Dictionary<string, string>();
            }

            public Dictionary<string, string> GetAll()
            {
                return data;
            }

            public string Get(string key = null)
            {
                return data[key];
            }

            public void Add(string key = null, object value = null)
            {
                data.Add(key, value.ToString());
            }
        }

        public class ServerScriptModelExtendedSql
        {
            private readonly Context Context;
            private readonly bool OnTesting;

            public ServerScriptModelExtendedSql(Context context, bool onTesting)
            {
                Context = context;
                OnTesting = onTesting;
            }

            public dynamic ExecuteDataSet(string name, object _params = null)
            {
                dynamic dataSet = ExtensionUtilities.ExecuteDataSetAsDynamic(
                    context: Context,
                    name: name,
                    _params: _params?.ToString().Deserialize<Dictionary<string, object>>());
                return dataSet;
            }

            public dynamic ExecuteTable(string name, object _params = null)
            {
                dynamic dataSet = ExecuteDataSet(
                    name: name,
                    _params: _params);
                return dataSet?.Table;
            }

            public dynamic ExecuteRow(string name, object _params = null)
            {
                dynamic dataTable = ExecuteTable(
                    name: name,
                    _params: _params);
                return ((IList<object>)dataTable)?.FirstOrDefault();
            }

            public object ExecuteScalar(string name, object _params = null)
            {
                dynamic dataRow = ExecuteRow(
                    name: name,
                    _params: _params);
                return ((IDictionary<string,object>)dataRow)?.FirstOrDefault().Value;
            }

        }

        public class ServerScriptModelNotification
        {
            private readonly Context Context;
            private readonly SiteSettings SiteSettings;

            public ServerScriptModelNotification(Context context, SiteSettings ss)
            {
                Context = context;
                SiteSettings = ss;
            }

            public ServerScriptModelNorificationModel New()
            {
                var norification = new ServerScriptModelNorificationModel(
                    context: Context,
                    ss: SiteSettings);
                return norification;
            }

            public ServerScriptModelNorificationModel Get(int Id)
            {
                var norification = new ServerScriptModelNorificationModel(
                    context: Context,
                    ss: SiteSettings,
                    Id: Id);
                return norification;
            }
        }
    }
}