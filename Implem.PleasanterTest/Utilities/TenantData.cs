using Implem.Libraries.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.PleasanterTest.Utilities
{
    public static class TenantData
    {
        public static string Get()
        {
            var tenantsettings = Initializer.Tenant.TenantSettings;
            return JsonConvert.SerializeObject(tenantsettings);
        }
    }
}
