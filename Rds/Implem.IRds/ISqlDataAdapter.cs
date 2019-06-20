using System;
using System.Data;
namespace Implem.IRds
{
    public interface ISqlDataAdapter : IDbDataAdapter, IDataAdapter, ICloneable
    {
        void Fill(DataTable dataTable);
    }
}
