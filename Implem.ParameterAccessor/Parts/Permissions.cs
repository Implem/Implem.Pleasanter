using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Permissions
    {
        public bool CheckManagePermission;
        public long General;
        public long Manager;
        public Dictionary<string, long> Pattern;
        public int PageSize;
    }
}
