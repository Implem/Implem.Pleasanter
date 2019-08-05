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
            SqlCommand.Parameters_AddWithValue("_T", SqlContainer.RdsUser.TenantId);
            SqlCommand.Parameters_AddWithValue("_D", SqlContainer.RdsUser.DeptId);
            SqlCommand.Parameters_AddWithValue("_U", SqlContainer.RdsUser.UserId);
        }

        private void SetCommandText(ISqlObjectFactory factory)
        {
            //TODO
            if (Implem.DefinitionAccessor.Parameters.Rds.Dbms?.ToLower() != "PostgreSQL".ToLower())
            {
                CommandText.Append(
                    $"declare {Parameters.Parameter.SqlParameterPrefix}I bigint;\n",
                    $"declare {Parameters.Parameter.SqlParameterPrefix}C int;\n");
            }
            /**/

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

            //TODO
            if (Implem.DefinitionAccessor.Parameters.Rds.Dbms?.ToLower() != "PostgreSQL".ToLower())
            {
                SetTransaction();
            }
            /**/
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
            //TODO
            //SqlCommand.Connection = factory.CreateSqlConnection(SqlContainer.ConnectionString);
            SqlCommand.CommandTimeout = SqlContainer.CommandTimeOut;
        }

        public int ExecuteNonQuery(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection = null)
        {
            int value = 0;
            SetCommand(factory: factory);

            //TODO
            try
            {
                SetTransactionOrConnectionToCommand(SqlCommand, factory, dbTransaction, dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        value = SqlCommand.ExecuteNonQuery();
                    },
                    cmd: SqlCommand);
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }

            /*
            SqlCommand.Connection.Open();
            Try(factory: factory,
                action: () =>
                {
                    SqlCommand.ExecuteNonQuery();
                }, cmd: SqlCommand);
            SqlCommand.Connection.Close();
            */

            Clear();
            return value;
        }

        public object ExecuteScalar(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            object value = null;
            SetCommand(factory: factory);

            //TODO
            try
            {
                SetTransactionOrConnectionToCommand(SqlCommand, factory, dbTransaction, dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        //value = SqlCommand.ExecuteScalar();
                        var dt = new DataTable();
                        dt.Load(SqlCommand.ExecuteReader());
                        var cmd = SqlCommand.CommandText;
                        value = dt.AsEnumerable().Select(dr => dr[0]).FirstOrDefault();
                    },
                    cmd: SqlCommand);
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }

            /*
            SqlCommand.Connection.Open();
            Try(factory: factory, 
                action: () =>
            {
                value = SqlCommand.ExecuteScalar();
            }, cmd: SqlCommand);
            SqlCommand.Connection.Close();
             */

            //TODO
            {
                var cmd = SqlCommand.CommandText;
                var param = SqlCommand.Parameters;
                var data = value;

                Console.WriteLine(cmd);
                Console.WriteLine(string.Join(", ", param.Cast<IDataParameter>().Select(p => $"{p.ParameterName}: {p.Value}")));
                Console.WriteLine($"value: {value}");
            }

            Clear();
            return value;
        }

        public DataTable ExecuteTable(ISqlObjectFactory factory, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {
            var dataTable = new DataTable();
            SetCommand(factory: factory);

            //TODO
            try
            {
                SetTransactionOrConnectionToCommand(SqlCommand, factory, dbTransaction, dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        factory.CreateSqlDataAdapter(SqlCommand).Fill(dataTable);
                    },
                    cmd: SqlCommand);
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }

            /*
            Try(factory: factory,
                action: () =>
                {
                    factory.CreateSqlDataAdapter(SqlCommand).Fill(dataTable);
                }, cmd: SqlCommand);
            */


            //TODO
            {
                var cmd = SqlCommand.CommandText;
                var param = SqlCommand.Parameters;
                var data = dataTable;

                Console.WriteLine(cmd);
                Console.WriteLine(string.Join(", ", param.Cast<IDataParameter>().Select(p => $"{p.ParameterName}: {p.Value}")));
                Console.WriteLine($"coun: {data.Rows.Count}");
            }

            Clear();
            return dataTable;
        }

        public DataSet ExecuteDataSet(ISqlObjectFactory factory, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {
            var dataSet = new DataSet();
            SetCommand(factory: factory);

            //TODO
            try {
                SetTransactionOrConnectionToCommand(SqlCommand, factory, dbTransaction, dbConnection);
                Try(factory: factory,
                    action: () =>
                    {
                        SqlContainer.SqlDataAdapter(factory: factory, sqlCommand: SqlCommand).Fill(dataSet);
                    },
                    cmd: SqlCommand);
            }
            finally
            {
                CleanupConnection(SqlCommand, dbConnection);
            }

            /*
            Try(factory: factory,
                action: () =>
                {
                    SqlContainer.SqlDataAdapter(factory: factory, sqlCommand: SqlCommand).Fill(dataSet);
                }, cmd: SqlCommand);
                */

            //TODO
            {
                var cmd = SqlCommand.CommandText;
                var param = SqlCommand.Parameters;
                var data = dataSet;

                Console.WriteLine(cmd);
                Console.WriteLine(string.Join(", ", param.Cast<IDataParameter>().Select(p => $"{p.ParameterName}: {p.Value}")));
                Console.WriteLine($"coun: {string.Join(", ", data.Tables.Cast<DataTable>().Select(t=>t.Rows.Count.ToString()))}");
            }

            Clear();
            return dataSet;
        }

        //TODO
        private ISqlConnection CreateAndOpenConnection(ISqlObjectFactory factory)
        {
            var sqlConnection = factory.CreateSqlConnection(SqlContainer.ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        //TODO
        private void SetTransactionOrConnectionToCommand(ISqlCommand sqlCommand, ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            sqlCommand.Transaction = dbTransaction;
            sqlCommand.Connection = dbConnection;
            if(sqlCommand.Connection == null) sqlCommand.Connection = CreateAndOpenConnection(factory);
        }

        //TODO
        private void CleanupConnection(ISqlCommand sqlCommand, IDbConnection dbConnection)
        {
            if (dbConnection == null)
                if (SqlCommand.Connection != null)
                    SqlCommand.Connection.Close();
            SqlCommand.Connection = null;
            SqlCommand.Transaction = null;
        }

        public List<SqlResponse> ExecuteDataSet_responses(ISqlObjectFactory factory)
        {
            var responses = new List<SqlResponse>();
            foreach(DataTable dataTable in ExecuteDataSet(factory: factory).Tables)
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

        //TODO
        //private void Try(Action action)
        private void Try(ISqlObjectFactory factory, Action action, ISqlCommand cmd)
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
                    if (factory.SqlErrors.ErrorCode(e) == 1205)
                    {
                        System.Threading.Thread.Sleep(Environments.DeadlockRetryInterval);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(cmd.CommandText);
                        throw;
                    }
                }
            }
        }

        public void ExecuteNonQuery(ISqlObjectFactory factory, SqlStatement sqlStatement, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            SqlContainer.SqlStatementCollection.Add(sqlStatement);
            ExecuteNonQuery(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection);
        }

        public void ExecuteNonQuery(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection, string commandText)
        {
            ExecuteNonQuery(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection, sqlStatement: new SqlStatement(commandText));
        }

        public bool ExecuteScalar_bool(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToBool();
        }

        public int ExecuteScalar_int(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToInt();
        }

        public long ExecuteScalar_long(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToLong();
        }

        public decimal ExecuteScalar_decimal(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToDecimal();
        }

        public DateTime ExecuteScalar_datetime(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return ExecuteScalar(factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToDateTime();
        }

        public string ExecuteScalar_string(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToStr();
        }

        public byte[] ExecuteScalar_bytes(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            return (byte[])ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection);
        }

        public SqlResponse ExecuteScalar_response(ISqlObjectFactory factory, IDbTransaction dbTransaction, IDbConnection dbConnection)
        {
            var response = ExecuteScalar(factory: factory, dbTransaction: dbTransaction, dbConnection: dbConnection).ToStr().Deserialize<SqlResponse>() ?? new SqlResponse();
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
