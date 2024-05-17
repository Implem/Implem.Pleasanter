using Implem.IRds;
using Implem.Libraries.Utilities;
using MySqlConnector;
using System.Data.Common;
namespace Implem.MySql
{
    internal class MySqlErrors : ISqlErrors
    {
        public int ErrorCodeDuplicateKey { get; } = 23505;
        public int ErrorCodeDuplicatePk { get; } = 23505;
        public int ErrorCodeDeadLocked { get; } = 40001;

        public int ErrorCode(DbException dbException)
        {
            return ((MySqlException)dbException).SqlState.ToInt();
        }
    }
}
