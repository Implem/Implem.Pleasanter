using System;
using System.Data;
namespace Implem.IRds
{
    public interface ISqlConnection : IDbConnection, IDisposable, ICloneable
    {
    }
}
