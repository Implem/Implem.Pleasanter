using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class ColumnAccessControl
    {
        [NonSerialized]
        public int? No;
        public string ColumnName;
        public List<int> Depts;
        public List<int> Groups;
        public List<int> Users;
        public List<string> RecordUsers;
        public Permissions.Types? Type;
        // compatibility Version 1.013
        public List<string> AllowedUsers;

        public ColumnAccessControl()
        {
        }

        public ColumnAccessControl(SiteSettings ss, Column column, string type)
        {
            No = column.No;
            ColumnName = column.ColumnName;
            Type = DefaultType(ss, type);
        }

        public bool IsDefault(SiteSettings ss, string type)
        {
            return Depts?.Any() != true
                && Groups?.Any() != true
                && Users?.Any() != true
                && RecordUsers?.Any() != true
                && DefaultType(ss, type) == (Type ?? Permissions.Types.NotSet);
        }

        private Permissions.Types DefaultType(SiteSettings ss, string type)
        {
            switch (type)
            {
                case "Create":
                    return Permissions.Get(
                        ss.ColumnDefinitionHash.Get(ColumnName)?.CreateAccessControl);
                case "Read":
                    return Permissions.Get(
                        ss.ColumnDefinitionHash.Get(ColumnName)?.ReadAccessControl);
                case "Update":
                    return Permissions.Get(
                        ss.ColumnDefinitionHash.Get(ColumnName)?.UpdateAccessControl);
                default: return Permissions.Types.NotSet;
            }
        }

        public ControlData ControlData(Context context, SiteSettings ss, string type)
        {
            var column = ss.GetColumn(context: context, columnName: ColumnName);
            return column != null
                ? new ControlData(column.LabelText + (!IsDefault(ss, type)
                    ? " (" + Displays.Enabled(context: context) + ")"
                    : string.Empty))
                : new ControlData(string.Empty);
        }

        public bool Allowed(
            Context context,
            SiteSettings ss,
            Permissions.Types? type,
            List<string> mine)
        {
            if (Depts?.Any() != true
                && Groups?.Any() != true
                && Users?.Any() != true
                && RecordUsers?.Any() != true
                && (Type ?? Permissions.Types.NotSet) == Permissions.Types.NotSet)
            {
                return true;
            }
            else if (Depts?.Contains(context.DeptId) == true)
            {
                return true;
            }
            else if (GroupContains(context: context, ss: ss))
            {
                return true;
            }
            else if (Users?.Contains(context.UserId) == true)
            {
                return true;
            }
            else if (Type != null
                && Type != Permissions.Types.NotSet
                && (Type & type) == Type)
            {
                return true;
            }
            else if (Type != null
                && Type != Permissions.Types.NotSet
                && RecordUsers?.Any() != true)
            {
                return false;
            }
            else if (RecordUsers?.Any(o => mine == null || mine?.Contains(o) == true) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool GroupContains(Context context, SiteSettings ss)
        {
            if (Groups?.Any() == true)
            {
                var groups = PermissionUtilities.Groups(
                    context: context,
                    ss: ss);
                return groups.Any(o => Groups.Contains(o));
            }
            else
            {
                return false;
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

        public ColumnAccessControl RecordingData()
        {
            return new ColumnAccessControl()
            {
                No = No,
                ColumnName = ColumnName,
                Depts = Depts?.Any() == true
                    ? Depts
                    : null,
                Groups = Groups?.Any() == true
                    ? Groups
                    : null,
                Users = Users?.Any() == true
                    ? Users
                    : null,
                RecordUsers = RecordUsers?.Any() == true
                    ? RecordUsers
                    : null,
                Type = Type != Permissions.Types.NotSet
                    ? Type
                    : null
            };
        }
    }
}