using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
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
        public readonly ExpandoObject ReadOnly = new ExpandoObject();
        public readonly ServerScriptModelContext Context;
        private readonly List<string> ChangeItemNames = new List<string>();

        public ServerScriptModel(
            Context context,
            IEnumerable<(string Name, object Value)> data,
            IEnumerable<(string Name, bool Value)> readOnly)
        {
            data?.ForEach(datam => ((IDictionary<string, object>)Data)[datam.Name] = datam.Value);
            readOnly?.ForEach(
                datam => ((IDictionary<string, object>)ReadOnly)[datam.Name] = datam.Value);
            ((INotifyPropertyChanged)Data).PropertyChanged += DataPropertyChanged;
            Context = new ServerScriptModelContext(
                userId: context.UserId,
                deptId: context.DeptId,
                groupIds: context.Groups,
                controller: context.Controller,
                action: context.Action);
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
            public readonly IEnumerable<int> Groups;
            public readonly string Controller;
            public readonly string Action;

            public ServerScriptModelContext(
                int userId,
                int deptId,
                IEnumerable<int> groupIds,
                string controller,
                string action)
            {
                (UserId, DeptId, Groups, Controller, Action)
                    = (userId, deptId, groupIds?.ToArray() ?? new int[0], controller, action);
            }
        }
    }
}