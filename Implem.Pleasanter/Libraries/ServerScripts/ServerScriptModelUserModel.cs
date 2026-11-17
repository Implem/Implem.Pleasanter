using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelUserModel
    {
        private readonly Context Context;
        private readonly HashSet<string> ReadableColumnNames;
        private readonly int TenantId;
        public readonly int UserId;
        public readonly int? DeptId;
        public readonly string LoginId;
        public readonly string Name;
        public readonly string UserCode;
        public readonly bool? TenantManager;
        public readonly bool? ServiceManager;
        public readonly bool? Disabled;
        private readonly Dictionary<string, object> Extras;

        public ServerScriptModelUserModel(
            Context context,
            int tenantId,
            int userId,
            int deptId,
            string loginId,
            string name,
            string userCode,
            bool? tenantManager,
            bool? serviceManager,
            bool? disabled,
            Dictionary<string, object> extras,
            HashSet<string> readableColumnNames)
        {
            Context = context;
            ReadableColumnNames = readableColumnNames;
            TenantId = tenantId;
            UserId = userId;
            DeptId = CanRead("DeptId") ? deptId : null;
            LoginId = CanRead("LoginId") ? loginId : null;
            Name = CanRead("Name") ? name : null;
            UserCode = CanRead("UserCode") ? userCode : null;
            TenantManager = CanRead("TenantManager") ? tenantManager : null;
            ServiceManager = CanRead("ServiceManager") ? serviceManager : null;
            Disabled = CanRead("Disabled") ? disabled : null;
            Extras = extras;
        }

        private bool CanRead(string columnName)
        {
            return ReadableColumnNames?.Contains(columnName) == true;
        }

        public object ToJson()
        {
            dynamic d = new ExpandoObject();
            var dict = (IDictionary<string, object>)d;
            dict["TenantId"] = TenantId;
            dict["UserId"] = UserId;
            dict["DeptId"] = DeptId;
            dict["LoginId"] = LoginId;
            dict["Name"] = Name;
            dict["UserCode"] = UserCode;
            dict["TenantManager"] = TenantManager;
            dict["ServiceManager"] = ServiceManager;
            dict["Disabled"] = Disabled;
            ServerScriptUtilities.MergeExtras(
                target: dict,
                extras: Extras,
                readableColumnNames: ReadableColumnNames);
            return d;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToJson());
        }
    }
}