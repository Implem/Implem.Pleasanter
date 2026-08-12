using System;
using System.Data;
using System.Data.Common;
using System.Text;
using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Pleasanter.Libraries.Requests;

namespace Implem.Pleasanter.Libraries.Server
{
    public sealed class DistributedLock : IDisposable
    {
        private const int CommandTimeoutMarginSeconds = 5;
        private const int MySqlMaxLockNameLength = 64;
        private const string PostgresLockNotAvailableSqlState = "55P03";
        private readonly ISqlConnection connection;
        private readonly string name;
        private readonly long key;
        private bool released;

        public bool Acquired { get; private set; }

        private DistributedLock(ISqlConnection connection, string name, long key)
        {
            this.connection = connection;
            this.name = name;
            this.key = key;
        }

        public static DistributedLock Acquire(Context context, string name, int timeoutSeconds)
        {
            var connection = context.CreateSqlConnection(Parameters.Rds.UserConnectionString);
            try
            {
                connection.Open();
                var distributedLock = new DistributedLock(
                    connection: connection,
                    name: name,
                    key: ComputeKey(name));
                distributedLock.Acquired = distributedLock.TryAcquire(timeoutSeconds: timeoutSeconds);
                if (!distributedLock.Acquired)
                {
                    connection.Close();
                    connection.Dispose();
                    distributedLock.released = true;
                }
                return distributedLock;
            }
            catch
            {
                connection.Close();
                connection.Dispose();
                throw;
            }
        }

        private bool TryAcquire(int timeoutSeconds)
        {
            return Parameters.Rds.Dbms switch
            {
                "SQLServer" => TryAcquireSqlServer(timeoutSeconds: timeoutSeconds),
                "PostgreSQL" => TryAcquirePostgreSql(timeoutSeconds: timeoutSeconds),
                "MySQL" => TryAcquireMySql(timeoutSeconds: timeoutSeconds),
                _ => true, // 未対応の DBMS では、従来どおり排他制御を行わずに処理を続ける。
            };
        }

        private bool TryAcquireSqlServer(int timeoutSeconds)
        {
            using var command = connection.CreateCommand();
            command.CommandTimeout = CommandTimeoutSeconds(timeoutSeconds: timeoutSeconds);
            command.CommandText =
                "declare @result int; "
                + "exec @result = sp_getapplock "
                + "@Resource = @name, @LockMode = 'Exclusive', "
                + "@LockOwner = 'Session', @LockTimeout = @timeout; "
                + "select @result;";
            AddParameter(command: command, parameterName: "@name", value: name);
            AddParameter(
                command: command,
                parameterName: "@timeout",
                value: timeoutSeconds <= 0 ? -1 : LockTimeoutMilliseconds(timeoutSeconds: timeoutSeconds));
            return Convert.ToInt32(command.ExecuteScalar()) >= 0;
        }

        private bool TryAcquirePostgreSql(int timeoutSeconds)
        {
            var lockTimeoutMilliseconds = timeoutSeconds <= 0 ? 0 : LockTimeoutMilliseconds(timeoutSeconds: timeoutSeconds);
            using var command = connection.CreateCommand();
            command.CommandTimeout = CommandTimeoutSeconds(timeoutSeconds: timeoutSeconds);
            command.CommandText =
                $"set lock_timeout = {lockTimeoutMilliseconds}; "
                + "select pg_advisory_lock(@key);";
            AddParameter(command: command, parameterName: "@key", value: key);
            try
            {
                command.ExecuteScalar();
                return true;
            }
            catch (DbException ex) when (ex.SqlState == PostgresLockNotAvailableSqlState)
            {
                return false;
            }
        }

        private bool TryAcquireMySql(int timeoutSeconds)
        {
            using var command = connection.CreateCommand();
            command.CommandTimeout = CommandTimeoutSeconds(timeoutSeconds: timeoutSeconds);
            command.CommandText = "select get_lock(@name, @timeout);";
            AddParameter(command: command, parameterName: "@name", value: MySqlLockName());
            AddParameter(
                command: command,
                parameterName: "@timeout",
                value: timeoutSeconds <= 0 ? -1 : timeoutSeconds);
            var result = command.ExecuteScalar();
            return result != null
                && result != DBNull.Value
                && Convert.ToInt32(result) == 1;
        }

        private void Release()
        {
            using var command = connection.CreateCommand();
            switch (Parameters.Rds.Dbms)
            {
                case "SQLServer":
                    command.CommandText =
                        "exec sp_releaseapplock @Resource = @name, @LockOwner = 'Session';";
                    AddParameter(command: command, parameterName: "@name", value: name);
                    command.ExecuteNonQuery();
                    break;
                case "PostgreSQL":
                    command.CommandText = "select pg_advisory_unlock(@key);";
                    AddParameter(command: command, parameterName: "@key", value: key);
                    command.ExecuteScalar();
                    break;
                case "MySQL":
                    command.CommandText = "select release_lock(@name);";
                    AddParameter(command: command, parameterName: "@name", value: MySqlLockName());
                    command.ExecuteScalar();
                    break;
            }
        }

        public void Dispose()
        {
            if (released)
            {
                return;
            }
            released = true;
            try
            {
                if (Acquired && connection.State == ConnectionState.Open)
                {
                    Release();
                }
            }
            catch
            {
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        private static void AddParameter(IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        private static int CommandTimeoutSeconds(int timeoutSeconds)
        {
            if (timeoutSeconds <= 0)
            {
                return 0;
            }
            return timeoutSeconds > int.MaxValue - CommandTimeoutMarginSeconds
                ? int.MaxValue
                : timeoutSeconds + CommandTimeoutMarginSeconds;
        }

        private static int LockTimeoutMilliseconds(int timeoutSeconds)
        {
            var milliseconds = (long)timeoutSeconds * 1000;
            return milliseconds > int.MaxValue
                ? int.MaxValue
                : (int)milliseconds;
        }

        private string MySqlLockName()
        {
            var suffix = $":{ComputeKey(connection.Database):x16}";
            var maxPrefixLength = MySqlMaxLockNameLength - suffix.Length;
            var prefix = name.Length > maxPrefixLength
                ? name[..maxPrefixLength]
                : name;
            return prefix + suffix;
        }

        private static long ComputeKey(string name)
        {
            const ulong offsetBasis = 14695981039346656037;
            const ulong prime = 1099511628211;
            var hash = offsetBasis;
            foreach (var b in Encoding.UTF8.GetBytes(name))
            {
                hash ^= b;
                hash *= prime;
            }
            return unchecked((long)hash);
        }
    }
}
