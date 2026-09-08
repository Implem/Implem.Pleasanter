using Implem.IRds;
using System;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using Microsoft.Data.SqlClient;
namespace Implem.SqlServer
{
    internal class SqlServerErrors : ISqlErrors
    {
        public int ErrorCodeDuplicateKey { get; } = 2601;
        public int ErrorCodeDuplicatePk { get; } = 2627;
        public int ErrorCodeDeadLocked { get; } = 1205;

        public int ErrorCode(DbException dbException)
        {
            return ((SqlException)dbException).Number;
        }

        public bool IsTimeout(DbException dbException)
        {
            return dbException is SqlException sqlException
                && sqlException.Number == -2;
        }

        public bool IsCertificateError(DbException dbException)
        {
            if (dbException is SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case -2146893019:
                    case -2146762487:
                        return true;
                }
            }
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            for (Exception exception = dbException;
                exception != null;
                exception = exception.InnerException)
            {
                if (exception is Win32Exception win32Exception
                    && (win32Exception.NativeErrorCode == unchecked((int)0x80090325)
                        || win32Exception.NativeErrorCode == unchecked((int)0x800B0109)))
                {
                    return true;
                }
                if (!isWindows
                    && exception is AuthenticationException)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
