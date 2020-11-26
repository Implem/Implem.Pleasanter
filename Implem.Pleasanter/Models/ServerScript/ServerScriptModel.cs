using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public class ServerScriptModel : IDisposable
    {
        public readonly ExpandoObject Data = new ExpandoObject();
        public readonly ExpandoObject Columns = new ExpandoObject();
        public readonly ServerScriptModelContext Context;
        public readonly ServerScriptModelSiteSettings SiteSettings;
        public readonly ServerScriptModelView View = new ServerScriptModelView();
        private readonly List<string> ChangeItemNames = new List<string>();

        public ServerScriptModel(
            Context context,
            SiteSettings ss,
            IEnumerable<(string Name, object Value)> data,
            IEnumerable<(string Name, ServerScriptModelColumn Value)> columns,
            IEnumerable<KeyValuePair<string, string>> columnFilterHach)
        {
            data?.ForEach(datam => ((IDictionary<string, object>)Data)[datam.Name] = datam.Value);
            columns?.ForEach(
                datam => ((IDictionary<string, object>)Columns)[datam.Name] = datam.Value);
            columnFilterHach?.ForEach(columnFilter =>
                ((IDictionary<string, object>)View.Filters)[columnFilter.Key] = columnFilter.Value);
            ((INotifyPropertyChanged)Data).PropertyChanged += DataPropertyChanged;
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
            ((INotifyPropertyChanged)Data).PropertyChanged -= DataPropertyChanged;
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
        }

        public class ServerScriptModelView
        {
            public readonly ExpandoObject Filters = new ExpandoObject();
        }

        public class ServerScriptModelSiteSettings
        {
            public int? DefaultViewId { get; set; }
        }
    }
}