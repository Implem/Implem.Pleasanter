using Implem.IRds;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Implem.PostgreSql
{
    class PostgreSqlErrors : ISqlErrors
    {
        public int ErrorCode(DbException dbException)
        {
            //TODO
            return ((NpgsqlException)dbException).ErrorCode;
        }
    }
}
