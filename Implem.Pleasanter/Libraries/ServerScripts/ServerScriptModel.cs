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
                userId: context.UserId,
                deptId: context.DeptId,
                groupIds: context.Groups,
                controller: context.Controller,
                action: context.Action);
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
            public readonly int UserId;
            public readonly int DeptId;
            public readonly IList<int> Groups;
            public readonly string Controller;
            public readonly string Action;

            public ServerScriptModelContext(
                int userId,
                int deptId,
                IEnumerable<int> groupIds,
                string controller,
                string action)
            {
                UserId = userId;
                DeptId = deptId;
                Groups = groupIds?.ToArray() ?? new int[0];
                Controller = controller;
                Action = action;
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

            public bool Insert(long id, object model)
            {
                if (OnTesting)
                {
                    return false;
                }
                return ServerScriptUtilities.Insert(
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