using Implem.Pleasanter.Libraries.Requests;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGroupMemberModel
    {
        private readonly Context Context;
        public readonly int GroupId;
        public readonly string GroupName;
        public readonly int DeptId;
        public readonly string DeptName;
        public readonly string DeptCode;
        public readonly int UserId;
        public readonly string LoginId;
        public readonly string Name;
        public readonly string UserCode;
        public readonly bool? TenantManager;
        public readonly bool? Disabled;
        public readonly bool Admin;
        private readonly Dictionary<string, object> Extras;

        public ServerScriptModelGroupMemberModel(
            Context context,
            int groupId,
            string groupName,
            int deptId,
            string deptName,
            string deptCode,
            int userId,
            string loginId,
            string name,
            string userCode,
            bool? tenantManager,
            bool? disabled,
            bool admin,
            Dictionary<string, object> extras,
            HashSet<string> usersReadableColumnNames,
            HashSet<string> deptsReadableColumnNames,
            HashSet<string> groupsReadableColumnNames)
        {
            Context = context;
            GroupId = groupId;
            GroupName = CanRead(groupsReadableColumnNames, "GroupName") ? groupName : null;
            DeptId = deptId;
            DeptName = CanRead(deptsReadableColumnNames, "DeptName") ? deptName : null;
            DeptCode = CanRead(deptsReadableColumnNames, "DeptCode") ? deptCode : null;
            UserId = userId;
            LoginId = CanRead(usersReadableColumnNames, "LoginId") ? loginId : null;
            Name = CanRead(usersReadableColumnNames, "Name") ? name : null;
            UserCode = CanRead(usersReadableColumnNames, "UserCode") ? userCode : null;
            TenantManager = CanRead(usersReadableColumnNames, "TenantManager") ? tenantManager : null;
            Disabled = CanRead(usersReadableColumnNames, "Disabled") ? disabled : null;
            Admin = admin;
            Extras = extras;
        }

        private static bool CanRead(HashSet<string> readableColumnNames, string columnName)
        {
            return readableColumnNames?.Contains(columnName) == true;
        }

        public object ToJson()
        {
            dynamic d = new ExpandoObject();
            var dict = (IDictionary<string, object>)d;
            dict["GroupId"] = GroupId;
            dict["GroupName"] = GroupName;
            dict["DeptId"] = DeptId;
            dict["DeptName"] = DeptName;
            dict["DeptCode"] = DeptCode;
            dict["UserId"] = UserId;
            dict["LoginId"] = LoginId;
            dict["Name"] = Name;
            dict["UserCode"] = UserCode;
            dict["TenantManager"] = TenantManager;
            dict["Disabled"] = Disabled;
            dict["Admin"] = Admin;
            ServerScriptUtilities.MergeExtras(dict, Extras);
            return d;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToJson());
        }
    }
}