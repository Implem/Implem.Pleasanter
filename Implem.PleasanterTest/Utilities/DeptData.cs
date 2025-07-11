using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class DeptData
    {
        public static int GetDeptIdByName(this IDictionary<int, DeptModel> depts, string deptName)
        {
            return depts
                .FirstOrDefault(o => o.Value.DeptName == deptName)
                .Value?.DeptId ?? 0;
        }
    }
}