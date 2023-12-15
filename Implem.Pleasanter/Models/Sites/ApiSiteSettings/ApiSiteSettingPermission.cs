using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ApiSiteSettingPermission
    {
        public List<int> Depts { get; set; }
        public List<int> Groups { get; set; }
        public List<int> Users { get; set; }
        public ApiSiteSettingPermission()
        {
        }
    }
}
