using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
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
            IEnumerable<KeyValuePair<string, string>> columnFilterHash,
            IEnumerable<KeyValuePair<string, SqlOrderBy.Types>> columnSorterHash,
            bool onTesting)
        {
            data?.ForEach(datam => ((IDictionary<string, object>)Model)[datam.Name] = datam.Value);
            Depts = new ServerScriptModelDepts(context: context);
            Groups = new ServerScriptModelGroups(context: context);
            Users = new ServerScriptModelUsers(context: context);
            columns?.ForEach(
                datam => ((IDictionary<string, object>)Columns)[datam.Name] = datam.Value);
            columnFilterHash?.ForEach(columnFilter =>
                ((IDictionary<string, object>)View.Filters)[columnFilter.Key] = columnFilter.Value);
            columnSorterHash?.ForEach(columnSorter =>
                ((IDictionary<string, object>)View.Sorters)[columnSorter.Key] = Enum.GetName(
                    typeof(SqlOrderBy.Types),
                    columnSorter.Value));
            ((INotifyPropertyChanged)Model).PropertyChanged += DataPropertyChanged;
            Context = new ServerScriptModelContext(
                context: context,
                logBuilder: context.LogBuilder,
                userData: context.UserData,
                errorData: context.ErrorData,
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
                timeZoneInfo: context.TimeZoneInfo?.ToString(),
                hasPrivilege: context.HasPrivilege,
                apiVersion: context.ApiVersion,
                apiRequestBody: context.ApiRequestBody,
                requestDataString: context.RequestDataString,
                contentType: context.ContentType,
                onTesting: onTesting,
                scriptDepth: context.ServerScriptDepth,
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

        public class ServerScriptModelColumn
        {
            public string LabelText { get; set; }
            public Dictionary<object, object> ChoiceHash { get; set; }
            public bool ReadOnly { get; set; }
            public string ExtendedFieldCss { get; set; }
            public string ExtendedControlCss { get; set; }
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

            public bool NeedReplace(
                Context context,
                SiteSettings ss,
                string columnName)
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: columnName);
                return ChoiceHash?.Any() == true
                    || ReadOnly != !(column?.CanRead(
                        context: context,
                        ss: ss,
                        mine: null,
                        noCache: true) == true && column?.CanUpdate(
                            context: context,
                            ss: ss,
                            mine: null,
                            noCache: true) == true)
                    || !ExtendedFieldCss.IsNullOrEmpty()
                    || !ExtendedControlCss.IsNullOrEmpty()
                    || !ExtendedCellCss.IsNullOrEmpty()
                    || !ExtendedHtmlBeforeField.IsNullOrEmpty()
                    || !ExtendedHtmlAfterField.IsNullOrEmpty()
                    || !RawText.IsNullOrEmpty();
            }
        }

        public class ServerScriptModelRow
        {
            public string ExtendedRowCss { get; set; }
            public Dictionary<string, ServerScriptModelColumn> Columns { get; set; }
            public Dictionary<string, string> Hidden { get; set; }
            private List<string> NeedReplaceHtmlCache { get; set; }

            public List<string> NeedReplaceHtml(Context context, SiteSettings ss)
            {
                if (NeedReplaceHtmlCache == null)
                {
                    var targetColumns = Columns
                        ?.Where(o => o.Value.NeedReplace(
                            context: context,
                            ss: ss,
                            columnName: o.Key))
                        .Select(o => o.Key)
                        .ToList();
                    NeedReplaceHtmlCache = context.Forms.List("NeedReplaceHtml")
                        .Concat(targetColumns)
                        .Distinct()
                        .ToList();
                }
                return NeedReplaceHtmlCache;
            }
        }
    }
}