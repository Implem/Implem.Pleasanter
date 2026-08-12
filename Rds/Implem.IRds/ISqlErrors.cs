using System.Data.Common;
namespace Implem.IRds
{
    public interface ISqlErrors
    {
        int ErrorCode(DbException dbException);
        bool IsTimeout(DbException dbException);
        int ErrorCodeDuplicateKey { get; }
        int ErrorCodeDuplicatePk { get; }
        int ErrorCodeDeadLocked { get; }
    }
}
