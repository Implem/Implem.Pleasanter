using System.Data.Common;
using System.Data.SqlClient;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlServerErrors
    {
        public int ErrorCodeDuplicateKey { get; } = 2601;
        public int ErrorCodeDuplicatePk { get; } = 2627;
        public int ErrorCodeDeadLocked { get; } = 1205;

        public int ErrorCode(DbException dbException)
        {
            return ((SqlException)dbException).Number;
        }
    }
}
