using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlIo : IDisposable
    {
        private bool Disposed;
        public SqlContainer SqlContainer;
        public ISqlCommand SqlCommand { get; private set; }
        public StringBuilder CommandText = new StringBuilder();

        public SqlIo(ISqlObjectFactory factory)
        {
            SqlCommand = factory.CreateSqlCommand();
        }

        public SqlIo(ISqlObjectFactory factory, SqlContainer sqlContainer) : this(factory)
        {
            SqlContainer = sqlContainer;
        }

        private void SetCommand(ISqlObjectFactory factory)
        {
            SetCommandUserParams();
            SetCommandText(factory: factory);
            SetSqlCommand(factory: factory);
            if (SqlContainer.WriteSqlToDebugLog)
            {
                SqlDebugs.WriteSqlLog(SqlContainer.RdsName, SqlCommand, Sqls.LogsPath);
            }
        }

        private void SetCommandUserParams()
        {
            SqlCommand.Parameters_AddWithValue(
                parameterName: $"{Parameters.Parameter.SqlParameterPrefix}T",
                value: SqlContainer.RdsUser.TenantId);
            SqlCommand.Parameters_AddWithValue(
                parameterName: $"{Parameters.Parameter.SqlParameterPrefix}D",
                value: SqlContainer.RdsUser.DeptId);
            SqlCommand.Parameters_AddWithValue(
                parameterName: $"{Parameters.Parameter.SqlParameterPrefix}U",
                value: SqlContainer.RdsUser.UserId);
        }

        private void SetCommandText(ISqlObjectFactory factory)
        {
            SqlContainer.SqlStatementCollection
                .Select((o, i) => new
                {
                    Statement = o,
                    Count = SqlContainer.SqlStatementCollection.Count > 1
                        ? (int?)(i + 1)
                        : null
                })
                .ForEach(data =>
                    data.Statement.BuildCommandText(
                        factory: factory,
                        sqlContainer: SqlContainer,
                        sqlCommand: SqlCommand,
                        commandText: CommandText,
                        commandCount: data.Count));
            var additionalParams = SqlContainer
                .SqlStatementCollection
                .Where(statement=>statement.AdditionalParams != null)
                .SelectMany(statement => statement.AdditionalParams)
                .GroupBy(param => param.Name)
                .ToArray();
            if (additionalParams != null)
            {
                additionalParams.ForEach(group =>
              {
                  SqlCommand
                  .Parameters_AddWithValue(group.Key, group.First().Value);
              });
            }
        }

        private void SetTransaction()
        {
            if (SqlContainer.Transactional)
            {
                CommandText.Insert(0, Sqls.BeginTransaction);
                CommandText.Append(Sqls.CommitTransaction);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:SQL It has been confirmed that the vulnerability on the query of security does not exist")]
        private void SetSqlCommand(ISqlObjectFactory factory)
        {
            SqlCommand.CommandType = CommandType.Text;
            SqlCommand.CommandText = CommandText.ToString();
            SqlCommand.CommandTimeout = SqlContainer.CommandTimeOut;
        }

        public int ExecuteNonQuery(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection = null)
        {
            int value = 0;
            SetCommand(factory: factory);

            try
            {
                SetTransactionOrConnectionToCommand(
                    sqlCommand: SqlCommand,
                    factory: factory,
                    dbTransaction: dbTransaction,
                    dbConnection: dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        value = SqlCommand.ExecuteNonQuery();
                    });
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }

            Clear();
            return value;
        }

        public object ExecuteScalar(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            object value = null;
            SetCommand(factory: factory);
            try
            {
                SetTransactionOrConnectionToCommand(
                    sqlCommand: SqlCommand,
                    factory: factory,
                    dbTransaction: dbTransaction,
                    dbConnection: dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        var dt = new DataTable();
                        dt.Load(SqlCommand.ExecuteReader());
                        var cmd = SqlCommand.CommandText;
                        value = dt.AsEnumerable()
                        .Select(dr => dr[0])
                        .FirstOrDefault();
                    });
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }
            Clear();
            return value;
        }

        public DataTable ExecuteTable(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction = null,
            IDbConnection dbConnection = null)
        {
            var dataTable = new DataTable();
            SetCommand(factory: factory);
            try
            {
                SetTransactionOrConnectionToCommand(
                    sqlCommand: SqlCommand,
                    factory: factory,
                    dbTransaction: dbTransaction,
                    dbConnection: dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        factory
                        .CreateSqlDataAdapter(SqlCommand)
                        .Fill(dataTable);
                    });
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }
            Clear();
            return dataTable;
        }

        public DataSet ExecuteDataSet(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction = null,
            IDbConnection dbConnection = null)
        {
            var dataSet = new DataSet();
            SetCommand(factory: factory);
            try {
                SetTransactionOrConnectionToCommand(
                    sqlCommand: SqlCommand,
                    factory: factory,
                    dbTransaction: dbTransaction,
                    dbConnection: dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        SqlContainer
                        .SqlDataAdapter(
                            factory: factory,
                            sqlCommand: SqlCommand)
                            .Fill(dataSet);
                    });
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }
            Clear();
            return dataSet;
        }

        private ISqlConnection CreateAndOpenConnection(
            ISqlObjectFactory factory)
        {
            var sqlConnection = factory.CreateSqlConnection(
                connectionString: SqlContainer.ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        private void SetTransactionOrConnectionToCommand(
            ISqlCommand sqlCommand,
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            sqlCommand.Transaction = dbTransaction;
            sqlCommand.Connection = dbConnection;
            if (sqlCommand.Connection == null)
            {
                sqlCommand.Connection = CreateAndOpenConnection(factory);
            }
        }

        private void CleanupConnection(
            ISqlCommand sqlCommand,
            IDbConnection dbConnection)
        {
            if (dbConnection == null)
            {
                if (SqlCommand.Connection != null)
                {
                    SqlCommand.Connection.Close();
                }
            }
            SqlCommand.Connection = null;
            SqlCommand.Transaction = null;
        }

        public List<SqlResponse> ExecuteDataSet_responses(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction = null,
            IDbConnection dbConnection = null)
        {
            var responses = new List<SqlResponse>();
            foreach(DataTable dataTable in ExecuteDataSet(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).Tables)
            {
                if (dataTable.Rows.Count == 1)
                {
                    var response = dataTable.Rows[0][0].ToString().Deserialize<SqlResponse>();
                    if (response != null)
                    {
                        response.DataTableName = dataTable.TableName;
                        responses.Add(response);
                    }
                }
            }
            return responses;
        }

        private void Try(ISqlObjectFactory factory, Action action)
        {
            for (int i = 0; i <= Environments.DeadlockRetryCount; i++)
            {
                try
                {
                    action();
                    break;
                }
                catch (DbException e)
                {
                    if (factory.SqlErrors.ErrorCode(e) == factory.SqlErrors.ErrorCodeDeadLocked)
                    {
                        System.Threading.Thread.Sleep(Environments.DeadlockRetryInterval);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void ExecuteNonQuery(
            ISqlObjectFactory factory,
            SqlStatement sqlStatement,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            SqlContainer.SqlStatementCollection.Add(sqlStatement);
            ExecuteNonQuery(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection);
        }

        public void ExecuteNonQuery(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection,
            string commandText)
        {
            ExecuteNonQuery(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection,
                sqlStatement: new SqlStatement(commandText));
        }

        public bool ExecuteScalar_bool(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).ToBool();
        }

        public int ExecuteScalar_int(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).ToInt();
        }

        public long ExecuteScalar_long(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).ToLong();
        }

        public decimal ExecuteScalar_decimal(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).ToDecimal();
        }

        public DateTime ExecuteScalar_datetime(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return ExecuteScalar(
                factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).ToDateTime();
        }

        public string ExecuteScalar_string(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection).ToStr();
        }

        public byte[] ExecuteScalar_bytes(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            return (byte[])ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection);
        }

        public SqlResponse ExecuteScalar_response(
            ISqlObjectFactory factory,
            IDbTransaction dbTransaction,
            IDbConnection dbConnection)
        {
            var response = ExecuteScalar(
                factory: factory,
                dbTransaction: dbTransaction,
                dbConnection: dbConnection)
                .ToStr()
                .Deserialize<SqlResponse>() ?? new SqlResponse();
            if (!response.ErrorMessage.IsNullOrEmpty())
            {
                throw new Exception(response.ErrorMessage);
            }
            return response;
        }

        public DataTable ExecuteTable(ISqlObjectFactory factory, string commandText)
        {
            SqlContainer.SqlStatementCollection.Add(new SqlStatement(commandText));
            return ExecuteTable(factory: factory);
        }

        public void Clear()
        {
            CommandText.Clear();
            SqlCommand.Parameters.Clear();
            SqlContainer.SqlStatementCollection.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (Disposed)
                {
                    return;
                }
                Disposed = true;
                if (disposing)
                {
                    SqlCommand.Dispose();
                    SqlCommand = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
