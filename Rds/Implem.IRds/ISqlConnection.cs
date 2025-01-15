using System;
using System.Data;
using System.Threading.Tasks;
namespace Implem.IRds
{
    public interface ISqlConnection : IDbConnection, IDisposable, ICloneable
    {
        Task OpenAsync();
    }
}
