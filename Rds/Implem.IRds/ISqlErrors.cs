using System.Data.Common;
namespace Implem.IRds
{
    public interface ISqlErrors
    {
        int ErrorCode(DbException dbException);

        int ErrorCodeDuplicateKey { get; }

        int ErrorCodeDeadLocked { get; }
    }
}
