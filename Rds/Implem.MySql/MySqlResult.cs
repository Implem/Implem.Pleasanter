using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Data;
using System.Linq;
namespace Implem.MySql
{
    internal class MySqlResult : ISqlResult
    {
        public int DeleteCount(DataTable data)
        {
            return data?.AsEnumerable().FirstOrDefault()?.Field<long>(0).ToInt() ?? 0;
        }

        public int RestoreCount(DataTable data)
        {
            return data?.AsEnumerable().FirstOrDefault()?.Field<long>(0).ToInt() ?? 0;
        }
    }
}
