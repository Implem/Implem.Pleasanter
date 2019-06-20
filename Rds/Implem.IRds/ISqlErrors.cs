using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Implem.IRds
{
    public interface ISqlErrors
    {
        int ErrorCode(DbException dbException);
    }
}
