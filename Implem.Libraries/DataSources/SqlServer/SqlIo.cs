using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlIo : IDisposable
    {
        private bool Disposed;
        public SqlContainer SqlContainer;
        public SqlCommand SqlCommand = new SqlCommand();
        public StringBuilder CommandText = new StringBuilder();

        public SqlIo()
        {
        }

        public SqlIo(SqlContainer sqlContainer)
        {
            SqlContainer = sqlContainer;
        }

        private void SetCommand()
        {
            SetCommandUserParams();
            SetCommandText();
            SetSqlCommand();
            if (SqlContainer.WriteSqlToDebugLog)
            {
                SqlDebugs.WriteSqlLog(SqlContainer.RdsName, SqlCommand, Sqls.LogsPath);
            }
        }

        private void SetCommandUserParams()
        {
            SqlCommand.Parameters.AddWithValue("_T", SqlContainer.RdsUser.TenantId);
            SqlCommand.Parameters.AddWithValue("_D", SqlContainer.RdsUser.DeptId);
            SqlCommand.Parameters.AddWithValue("_U", SqlContainer.RdsUser.UserId);
        }

        private void SetCommandText()
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
        private void SetSqlCommand()
        {
            SqlCommand.CommandType = CommandType.Text;
            SqlCommand.CommandText = CommandText.ToString();
            SqlCommand.Connection = new SqlConnection(SqlContainer.ConnectionString);
            SqlCommand.CommandTimeout = SqlContainer.CommandTimeOut;
        }

        public void ExecuteNonQuery()
        {
            SetCommand();
            using (SqlCommand)
            using (SqlCommand.Connection)
            {
                SqlCommand.Connection.Open();
                Try(action: () =>
                {
                    SqlCommand.ExecuteNonQuery();
                });
                SqlCommand.Connection.Close();
            }
            Clear();
        }

        public object ExecuteScalar()
        {
            object value = null;
            SetCommand();
            using (SqlCommand)
            using (SqlCommand.Connection)
            {
                SqlCommand.Connection.Open();
                Try(action: () =>
                {
                    value = SqlCommand.ExecuteScalar();
                });
                SqlCommand.Connection.Close();
            }
            Clear();
            return value;
        }

        public DataTable ExecuteTable()
        {
            var dataTable = new DataTable();
            SetCommand();
            using (SqlCommand) 
            using (SqlCommand.Connection)
            {
                SqlCommand.Connection.Open();
                Try(action: () =>
                {
                    new SqlDataAdapter(SqlCommand).Fill(dataTable);
                });
                SqlCommand.Connection.Close();
            }
            Clear();
            return dataTable;
        }

        public DataSet ExecuteDataSet()
        {
            var dataSet = new DataSet();
            SetCommand();
            using (SqlCommand)
            using (SqlCommand.Connection)
            {
                SqlCommand.Connection.Open();
                Try(action: () =>
                {
                    SqlContainer.SqlDataAdapter(SqlCommand).Fill(dataSet);
                });
                SqlCommand.Connection.Close();
            }
            Clear();
            return dataSet;
        }

        public List<SqlResponse> ExecuteDataSet_responses()
        {
            var responses = new List<SqlResponse>();
            foreach(DataTable dataTable in ExecuteDataSet().Tables)
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

        private void Try(Action action)
        {
            for (int i = 0; i <= Environments.DeadlockRetryCount; i++)
            {
                try
                {
                    action();
                    break;
                }
                catch (SqlException e)
                {
                    if (e.Number == 1205)
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

        public void ExecuteNonQuery(SqlStatement sqlStatement)
        {
            SqlContainer.SqlStatementCollection.Add(sqlStatement);
            ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string commandText)
        {
            ExecuteNonQuery(new SqlStatement(commandText));
        }

        public bool ExecuteScalar_bool()
        {
            return ExecuteScalar().ToBool();
        }

        public int ExecuteScalar_int()
        {
            return ExecuteScalar().ToInt();
        }

        public long ExecuteScalar_long()
        {
            return ExecuteScalar().ToLong();
        }

        public decimal ExecuteScalar_decimal()
        {
            return ExecuteScalar().ToDecimal();
        }

        public DateTime ExecuteScalar_datetime()
        {
            return ExecuteScalar().ToDateTime();
        }

        public string ExecuteScalar_string()
        {
            return ExecuteScalar().ToStr();
        }

        public byte[] ExecuteScalar_bytes()
        {
            return (byte[])ExecuteScalar();
        }

        public SqlResponse ExecuteScalar_response()
        {
            var response = ExecuteScalar().ToStr().Deserialize<SqlResponse>() ?? new SqlResponse();
            if (!response.ErrorMessage.IsNullOrEmpty())
            {
                throw new Exception(response.ErrorMessage);
            }
            return response;
        }

        public DataTable ExecuteTable(string commandText)
        {
            SqlContainer.SqlStatementCollection.Add(new SqlStatement(commandText));
            return ExecuteTable();
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
