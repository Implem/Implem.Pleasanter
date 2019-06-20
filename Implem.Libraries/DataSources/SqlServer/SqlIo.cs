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
            CommandText.Append(
                "declare @_I bigint;\n",
                "declare @_C int;\n");
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
            SetTransaction();
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
            SqlCommand.Connection = factory.CreateSqlConnection(SqlContainer.ConnectionString);
            SqlCommand.CommandTimeout = SqlContainer.CommandTimeOut;
        }

        public void ExecuteNonQuery(ISqlObjectFactory factory)
        {
            SetCommand(factory: factory);
            SqlCommand.Connection.Open();
            Try(factory:factory,
                action: () =>
            {
                SqlCommand.ExecuteNonQuery();
            }, cmd: SqlCommand);
            SqlCommand.Connection.Close();
            Clear();
        }

        public object ExecuteScalar(ISqlObjectFactory factory)
        {
            object value = null;
            SetCommand(factory: factory);
            SqlCommand.Connection.Open();
            Try(factory: factory, 
                action: () =>
            {
                value = SqlCommand.ExecuteScalar();
            }, cmd: SqlCommand);
            SqlCommand.Connection.Close();

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

        public DataTable ExecuteTable(ISqlObjectFactory factory)
        {
            var dataTable = new DataTable();
            SetCommand(factory: factory);
            Try(factory: factory, 
                action: () =>
            {
                factory.CreateSqlDataAdapter(SqlCommand).Fill(dataTable);
            }, cmd: SqlCommand);

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

        public DataSet ExecuteDataSet(ISqlObjectFactory factory)
        {
            var dataSet = new DataSet();
            SetCommand(factory: factory);
            Try(factory: factory, 
                action: () =>
            {
                SqlContainer.SqlDataAdapter(factory: factory, sqlCommand: SqlCommand).Fill(dataSet);
            }, cmd: SqlCommand);

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
                        throw;
                    }
                }
            }
        }

        public void ExecuteNonQuery(ISqlObjectFactory factory, SqlStatement sqlStatement)
        {
            SqlContainer.SqlStatementCollection.Add(sqlStatement);
            ExecuteNonQuery(factory: factory);
        }

        public void ExecuteNonQuery(ISqlObjectFactory factory, string commandText)
        {
            ExecuteNonQuery(factory: factory, sqlStatement: new SqlStatement(commandText));
        }

        public bool ExecuteScalar_bool(ISqlObjectFactory factory)
        {
            return ExecuteScalar(factory: factory).ToBool();
        }

        public int ExecuteScalar_int(ISqlObjectFactory factory)
        {
            return ExecuteScalar(factory: factory).ToInt();
        }

        public long ExecuteScalar_long(ISqlObjectFactory factory)
        {
            return ExecuteScalar(factory: factory).ToLong();
        }

        public decimal ExecuteScalar_decimal(ISqlObjectFactory factory)
        {
            return ExecuteScalar(factory: factory).ToDecimal();
        }

        public DateTime ExecuteScalar_datetime(ISqlObjectFactory factory)
        {
            return ExecuteScalar(factory).ToDateTime();
        }

        public string ExecuteScalar_string(ISqlObjectFactory factory)
        {
            return ExecuteScalar(factory: factory).ToStr();
        }

        public byte[] ExecuteScalar_bytes(ISqlObjectFactory factory)
        {
            return (byte[])ExecuteScalar(factory: factory);
        }

        public SqlResponse ExecuteScalar_response(ISqlObjectFactory factory)
        {
            var response = ExecuteScalar(factory: factory).ToStr().Deserialize<SqlResponse>() ?? new SqlResponse();
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
