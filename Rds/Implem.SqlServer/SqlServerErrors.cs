using Implem.IRds;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Implem.SqlServer
{
    class SqlServerErrors : ISqlErrors
    {
        public int ErrorCode(DbException dbException)
        {
            return ((SqlException)dbException).Number;
        }
    }
}
