using Implem.IRds;
using System.Data.Common;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    internal class SqlServerErrors : ISqlErrors
    {
        public int ErrorCodeDuplicateKey { get; } = 2601;

        public int ErrorCodeDeadLocked { get; } = 1205;

        public int ErrorCode(DbException dbException)
        {
            return ((SqlException)dbException).Number;
        }
    }
}
