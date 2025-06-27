using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public class PermissionIdList
    {
        public List<DeptIdHash> DeptIdList;
        public List<GroupIdHash> GroupIdList;
        public List<UserIdHash> UserIdList;
        [NonSerialized]
        public Dictionary<int, int> DeptConvertCache = new Dictionary<int, int>();
        [NonSerialized]
        public Dictionary<int, int> GroupConvertCache = new Dictionary<int, int>();
        [NonSerialized]
        public Dictionary<int, int> UserConvertCache = new Dictionary<int, int>();

        public PermissionIdList()
        {
        }
    }
}