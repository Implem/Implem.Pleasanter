using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class StatusControl : ISettingListItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ControlConstraintsTypes
        {
            None,
            Required,
            ReadOnly,
            Hidden
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public bool? ReadOnly { get; set; }
        public Dictionary<string, ControlConstraintsTypes> ColumnHash { get; set; }
        public View View { get; set; }
        public List<int> Depts { get; set; }
        public List<int> Groups { get; set; }
        public List<int> Users { get; set; }
        public int? Delete {  get; set; }

        public StatusControl()
        {
        }

        public StatusControl(
            int id,
            string name,
            string description,
            int? status,
            bool? readOnly,
            Dictionary<string, ControlConstraintsTypes> columnHash,
            View view,
            List<Permission> permissions)
        {
            Id = id;
            Name = name;
            Status = status;
            ReadOnly = readOnly;
            ColumnHash = columnHash;
            Description = description;
            View = view;
            SetPermissions(permissions: permissions);
        }

        public void Update(
            string name,
            string description,
            int? status,
            bool? readOnly,
            Dictionary<string, ControlConstraintsTypes> columnHash,
            View view,
            List<Permission> permissions)
        {
            if (name != null)
            {
                Name = name;
            }
            if (status != null)
            {
                Status = status;
            }
            if (readOnly != null)
            {
                ReadOnly = readOnly;
            }
            if (columnHash != null)
            {
                ColumnHash = columnHash;
            }
            if (description != null)
            {
                Description = description;
            }
            if (view != null)
            {
                View = view;
            }
            if (permissions != null)
            {
                SetPermissions(permissions: permissions);
            }
        }

        private void SetPermissions(List<Permission> permissions)
        {
            Depts?.Clear();
            Groups?.Clear();
            Users?.Clear();
            foreach (var permission in permissions)
            {
                switch (permission.Name)
                {
                    case "Dept":
                        if (Depts == null)
                        {
                            Depts = new List<int>();
                        }
                        if (!Depts.Contains(permission.Id))
                        {
                            Depts.Add(permission.Id);
                        }
                        break;
                    case "Group":
                        if (Groups == null)
                        {
                            Groups = new List<int>();
                        }
                        if (!Groups.Contains(permission.Id))
                        {
                            Groups.Add(permission.Id);
                        }
                        break;
                    case "User":
                        if (Users == null)
                        {
                            Users = new List<int>();
                        }
                        if (!Users.Contains(permission.Id))
                        {
                            Users.Add(permission.Id);
                        }
                        break;
                }
            }
        }

        public List<Permission> GetPermissions(SiteSettings ss)
        {
            var permissions = new List<Permission>();
            Depts?.ForEach(deptId => permissions.Add(new Permission(
                ss: ss,
                name: "Dept",
                id: deptId)));
            Groups?.ForEach(groupId => permissions.Add(new Permission(
                ss: ss,
                name: "Group",
                id: groupId)));
            Users?.ForEach(userId => permissions.Add(new Permission(
                ss: ss,
                name: "User",
                id: userId)));
            return permissions;
        }

        public StatusControl GetRecordingData(
            Context context,
            SiteSettings ss)
        {
            var statusControl = new StatusControl();
            statusControl.Id = Id;
            statusControl.Name = Name;
            if (!Description.IsNullOrEmpty())
            {
                statusControl.Description = Description;
            }
            statusControl.Status = Status;
            if (ReadOnly == true)
            {
                statusControl.ReadOnly = ReadOnly;
            }
            if (ColumnHash?.Any(o => o.Value != ControlConstraintsTypes.None) == true)
            {
                statusControl.ColumnHash = ColumnHash
                    .Where(o => o.Value != ControlConstraintsTypes.None)
                    .ToDictionary(o => o.Key, o => o.Value);
            }
            statusControl.View = View?.GetRecordingData(
                context: context,
                ss: ss);
            if (Depts?.Any() == true)
            {
                statusControl.Depts = Depts;
            }
            if (Groups?.Any() == true)
            {
                statusControl.Groups = Groups;
            }
            if (Users?.Any() == true)
            {
                statusControl.Users = Users;
            }
            return statusControl;
        }

        public bool Accessable(Context context)
        {
            if (context.HasPrivilege)
            {
                return true;
            }
            if (Depts?.Any() != true
                && Groups?.Any() != true
                && Users?.Any() != true)
            {
                return true;
            }
            if (Depts?.Contains(context.DeptId) == true)
            {
                return true;
            }
            if (Groups?.Any(groupId => context.Groups.Contains(groupId)) == true)
            {
                return true;
            }
            if (Users?.Contains(context.UserId) == true)
            {
                return true;
            }
            return false;
        }

        public static ControlConstraintsTypes GetControlType(string controlType)
        {
            ControlConstraintsTypes ret;
            Enum.TryParse(controlType, out ret);
            return ret;
        }
    }
}