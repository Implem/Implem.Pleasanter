using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Security
{
    public class Permission
    {
        public string Name;
        public int Id;
        public Permissions.Types Type;
        public bool Source;

        public Permission(string name, int id, Permissions.Types type, bool source = false)
        {
            Name = name;
            Id = id;
            Type = type;
            Source = source;
        }

        public Permission(SiteSettings ss, string name, int id, bool source = false)
        {
            Name = name;
            Id = id;
            Source = source;
        }

        public Permission(DataRow dataRow, bool source = false)
        {
            if (dataRow.Int("DeptId") != 0)
            {
                Name = "Dept";
                Id = dataRow.Int("DeptId");
            }
            else if (dataRow.Int("GroupId") != 0)
            {
                Name = "Group";
                Id = dataRow.Int("GroupId");
            }
            else if (dataRow.Int("UserId") != 0)
            {
                Name = "User";
                Id = dataRow.Int("UserId");
            }
            if (dataRow.Table.Columns.Contains("PermissionType"))
            {
                Type = (Permissions.Types)dataRow["PermissionType"].ToLong();
            }
            Source = source;
        }

        public string NameAndId()
        {
            return Name + "," + Id;
        }

        public string Key()
        {
            return Name + "," + Id + "," + Type.ToInt().ToString();
        }

        public bool Exists(Context context)
        {
            if (Id == 0)
            {
                return false;
            }
            switch (Name)
            {
                case "Dept":
                    return SiteInfo.Dept(
                        tenantId: context.TenantId,
                        deptId: Id) != null;
                case "Group":
                    return new GroupModel(
                        context: context,
                        ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                        groupId: Id).AccessStatus == Databases.AccessStatuses.Selected;
                case "User":
                    return !SiteInfo.User(
                        context: context,
                        userId: Id).Anonymous();
                default:
                    return false;
            }
        }

        public ControlData ControlData(Context context, SiteSettings ss, bool withType = true)
        {
            var typeName = withType
                ? DisplayTypeName(context: context)
                : null;
            switch (Name)
            {
                case "Dept":
                    var dept = SiteInfo.Dept(
                        tenantId: context.TenantId,
                        deptId: Id);
                    return new ControlData(
                        id: Id,
                        text: Displays.Depts(context: context),
                        name: Id != 0
                            ? dept?.Name
                            : null,
                        title: dept?.Tooltip(),
                        typeName: typeName);
                case "Group":
                    var group = SiteInfo.Group(
                        tenantId: context.TenantId,
                        groupId: Id);
                    return new ControlData(
                        id: Id,
                        text: Displays.Groups(context: context),
                        name: Id != 0
                            ? group?.Name
                            : null,
                        title: group?.Tooltip(),
                        typeName: typeName);
                case "User":
                    var user = SiteInfo.User(
                        context: context,
                        userId: Id);
                    return new ControlData(
                        id: Id,
                        text: Displays.Users(context: context),
                        name: Id != 0
                            ? user?.Name
                            : null,
                        title: user?.Tooltip(context: context),
                        typeName: typeName);
                default:
                    var column = ss?.GetColumn(
                        context: context,
                        columnName: Name);
                    return new ControlData(
                        id: Id,
                        text: Displays.Column(context: context),
                        name: column?.LabelText,
                        title: column?.LabelTextDefault,
                        typeName: typeName);
            }
        }

        private string DisplayTypeName(Context context)
        {
            var permissionType = Type.ToLong();
            var typeName = Parameters.Permissions.Pattern.ContainsValue(permissionType)
                ? Displays.Get(
                    context: context,
                    id: Parameters.Permissions.Pattern.First(o =>
                        o.Value == permissionType).Key)
                : Displays.Special(context: context);
            return $" - [{typeName}]";
        }
    }
}