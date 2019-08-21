using Implem.IRds;
using Npgsql;
using System.Data.Common;
namespace Implem.PostgreSql
{
    internal class PostgreSqlErrors : ISqlErrors
    {
        public int ErrorCodeDuplicateKey { get; } = 23505;

        public int ErrorCodeDeadLocked { get; } = 40001;

        public int ErrorCode(DbException dbException)
        {
            return ((NpgsqlException)dbException).ErrorCode;
        }
    }
}
