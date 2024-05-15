using Implem.IRds;
using System.Data;
namespace Implem.MySql
{
    internal class MySqlResult : ISqlResult
    {
        public int DeleteCount(DataTable data)
        {
            return data?.Rows.Count ?? 0;
        }

        public int RestoreCount(DataTable data)
        {
            return data?.Rows.Count ?? 0;
        }
    }
}
