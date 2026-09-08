using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelUserModel
    {
        private readonly Context Context;
        private readonly int TenantId;
        public readonly int UserId;
        public readonly int DeptId;
        public readonly string LoginId;
        public readonly string Name;
        public readonly string UserCode;
        public readonly bool TenantManager;
        public readonly bool ServiceManager;
        public readonly bool Disabled;
        private readonly Dictionary<string, object> Extras;

        public ServerScriptModelUserModel(
            Context context,
            int tenantId,
            int userId,
            int deptId,
            string loginId,
            string name,
            string userCode,
            bool tenantManager,
            bool serviceManager,
            bool disabled,
            Dictionary<string, object> extras)
        {
            Context = context;
            TenantId = tenantId;
            UserId = userId;
            DeptId = deptId;
            LoginId = loginId;
            Name = name;
            UserCode = userCode;
            TenantManager = tenantManager;
            ServiceManager = serviceManager;
            Disabled = disabled;
            Extras = extras;
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
            ServerScriptUtilities.MergeExtras(dict, Extras);
            return d;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToJson());
        }
    }
}