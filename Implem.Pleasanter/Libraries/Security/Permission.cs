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

        public ControlData ControlData(Context context, SiteSettings ss, bool withType = true)
        {
            switch (Name)
            {
                case "Dept":
                    var dept = SiteInfo.Dept(
                        tenantId: context.TenantId,
                        deptId: Id);
                    return DisplayText(
                        Displays.Depts(),
                        Id != 0
                            ? dept?.Name
                            : null,
                        dept?.Code,
                        withType);
                case "Group":
                    var groupModel = Id != 0
                        ? new GroupModel(
                            context: context,
                            ss: SiteSettingsUtilities.GroupsSiteSettings(context: context),
                            groupId: Id)
                        : null;
                    return DisplayText(
                        Displays.Groups(),
                        groupModel?.AccessStatus == Databases.AccessStatuses.Selected
                            ? groupModel.GroupName
                            : null,
                        null,
                        withType);
                case "User":
                    var user = SiteInfo.User(
                        context: context,
                        userId: Id);
                    return DisplayText(
                        Displays.Users(),
                        Id != 0
                            ? user?.Name
                            : null,
                        Id != 0
                            ? user?.LoginId
                            : null,
                        withType);
                default:
                    var column = ss?.GetColumn(
                        context: context,
                        columnName: Name);
                    return DisplayText(
                        Displays.Column(),
                        column?.LabelText,
                        column?.LabelTextDefault,
                        withType);
            }
        }

        private ControlData DisplayText(string text, string name, string title, bool withType)
        {
            return new ControlData(
                text: "[" + text +
                    (Id != 0
                        ? " " + Id
                        : string.Empty) +
                    "]" +
                    (name != null
                        ? " " + name
                        : string.Empty) +
                    (withType
                        ? " - [" + DisplayTypeName() + "]"
                        : string.Empty),
                title: title);
        }

        private string DisplayTypeName()
        {
            var permissionType = Type.ToLong();
            return Parameters.Permissions.Pattern.ContainsValue(permissionType)
                ? Displays.Get(Parameters.Permissions.Pattern.First(o =>
                    o.Value == permissionType).Key)
                : Displays.Special();
        }
    }
}