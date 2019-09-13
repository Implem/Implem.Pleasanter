using Implem.IRds;
using System.Data;
using System.Linq;
namespace Implem.SqlServer
{
    internal class SqlServerResult : ISqlResult
    {
        public int DeleteCount(DataTable data)
        {
            return data?.AsEnumerable().FirstOrDefault()?.Field<int>(0) ?? 0;
        }

        public int RestoreCount(DataTable data)
        {
            return data?.AsEnumerable().FirstOrDefault()?.Field<int>(0) ?? 0;
        }
    }
}
