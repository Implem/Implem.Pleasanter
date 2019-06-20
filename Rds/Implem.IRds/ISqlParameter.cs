using System.Data;
namespace Implem.IRds
{
    public interface ISqlParameter : IDataParameter, IDbDataParameter
    {
        string SqlDbType { get; }
    }
}
