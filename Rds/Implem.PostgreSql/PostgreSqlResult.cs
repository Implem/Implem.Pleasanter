using Implem.IRds;
using System.Data;
namespace Implem.PostgreSql
{
    internal class PostgreSqlResult : ISqlResult
    {
        public int DeleteCount(DataTable data)
        {
            return data?.Rows.Count ?? 0;
        }
    }
}
