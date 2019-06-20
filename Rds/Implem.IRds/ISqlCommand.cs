using System;
using System.Collections.Generic;
using System.Data;
namespace Implem.IRds
{
    public interface ISqlCommand: IDbCommand, IDisposable, ICloneable
    {
        void Parameters_AddWithValue(string parameterName, object value);

        void Parameters_Add(ISqlParameter parameter);

        IEnumerable<ISqlParameter> SqlParameters();
    }
}
