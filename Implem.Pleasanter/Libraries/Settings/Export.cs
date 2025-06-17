using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Export : ISettingListItem
    {
        public enum Types : int
        {
            Csv = 0,
            Json = 1
        }
        public enum DelimiterTypes : int
        {
            Comma = 0,
            Tab = 1
        }

        public enum ExecutionTypes : int
        {
            Direct = 0,
            MailNotify =1
        }

        public int Id { get; set; }
        public string Name;
        public bool? Header;
        public List<ExportColumn> Columns;
        public Types Type;
        public DelimiterTypes DelimiterType;
        public bool? EncloseDoubleQuotes;
        public ExecutionTypes ExecutionType;
        public List<int> Depts;
        public List<int> Groups;
        public List<int> Users;
        // compatibility Version 1.014
        public Join Join;
        public bool ExportCommentsJsonFormat = false;

        public Export()
        {
        }

        public Export(
            int id,
            string name,
            Types type,
            bool header,
            List<ExportColumn> columns,
            DelimiterTypes delimiterType,
            bool encloseDoubleQuotes,
            ExecutionTypes executionType,
            bool exportCommentsJsonFormat)
        {
            Id = id;
            Name = name;
            Type = type;
            Header = header;
            Columns = new List<ExportColumn>();
            columns.ForEach(column =>
            {
                column.Id = NewColumnId();
                Columns.Add(column);
            });
            DelimiterType = delimiterType;
            EncloseDoubleQuotes = encloseDoubleQuotes;
            ExecutionType = executionType;
            ExportCommentsJsonFormat = exportCommentsJsonFormat;
        }

        public Export(List<ExportColumn> columns)
        {
            Header = true;
            EncloseDoubleQuotes = true;
            Columns = new List<ExportColumn>();
            columns.ForEach(column =>
            {
                column.Id = NewColumnId();
                Columns.Add(column);
            });
        }

        public void SetColumns(Context context, SiteSettings ss)
        {
            Header = Header ?? true;
            Columns.ForEach(exportColumn =>
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: exportColumn.ColumnName);
                if (column != null)
                {
                    exportColumn.Column = column;
                    exportColumn.SiteTitle = ss.GetJoinedSs(column.TableName()).Title;
                }
            });
        }

        public void Update(
            string name,
            Types type,
            bool header,
            List<ExportColumn> columns,
            DelimiterTypes delimiterType,
            bool encloseDoubleQuotes,
            ExecutionTypes executionType,
            List<Permission> permissions,
            bool exportCommentsJsonFormat)
        {
            Name = name;
            Type = type;
            Header = header;
            Columns = columns;
            DelimiterType = delimiterType;
            EncloseDoubleQuotes = encloseDoubleQuotes;
            ExecutionType = executionType;
            ExportCommentsJsonFormat = exportCommentsJsonFormat;
            SetPermissions(permissions);
        }

        public int NewColumnId()
        {
            return Columns.Any()
                ? Columns.Max(o => o.Id) + 1
                : 1;
        }

        public void SetPermissions(List<Permission> permissions)
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

        public Export GetRecordingData()
        {
            var export = new Export();
            export.Id = Id;
            export.Name = Name;
            export.Header = Header == true
                ? null
                : Header;
            export.Columns = new List<ExportColumn>();
            export.Type = Type;
            if (DelimiterType != DelimiterTypes.Comma)
            {
                export.DelimiterType = DelimiterType;
            }
            export.EncloseDoubleQuotes = EncloseDoubleQuotes == true
                ? null
                : EncloseDoubleQuotes;
            Columns?.ForEach(column => export.Columns.Add(column.GetRecordingData()));
            export.ExecutionType = ExecutionType;
            export.ExportCommentsJsonFormat = ExportCommentsJsonFormat;
            if (Depts?.Any() == true)
            {
                export.Depts = Depts;
            }
            if (Groups?.Any() == true)
            {
                export.Groups = Groups;
            }
            if (Users?.Any() == true)
            {
                export.Users = Users;
            }
            return export;
        }
    }
}