using Implem.Libraries.Utilities;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlIo : IDisposable
    {
        private bool disposed;
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

        public void AddSqlStatement(SqlStatement sqlStatement)
        {
            SqlContainer.SqlStatementCollection.Add(sqlStatement);
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
            SqlCommand.Parameters.AddWithValue("_U", SqlContainer.RdsUser.UserId);
            SqlCommand.Parameters.AddWithValue("_D", SqlContainer.RdsUser.DeptId);
        }

        private void SetCommandText()
        {
            if (SqlContainer.SqlStatementCollection.Any(o => o.SelectIdentity))
            {
                CommandText.Append("declare @_I bigint;\n");
            }
            if (SqlContainer.SqlStatementCollection.Count == 1)
            {
                SqlContainer.SqlStatementCollection[0].BuildCommandText(
                    SqlContainer, SqlCommand, CommandText);
            }
            else
            {
                SqlContainer.SqlStatementCollection
                    .Select((o, i) => new { Method = o, Count = i + 1 })
                    .ForEach(data =>
                        data.Method.BuildCommandText(
                            SqlContainer,
                            SqlCommand,
                            CommandText,
                            data.Count));
                SetTransaction();
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
            SqlCommand.Connection.Open();
            switch (SqlContainer.RdsProvider)
            {
                case "Azure":
                    SqlCommand.ExecuteNonQueryWithRetry();
                    break;
                default:
                    SqlCommand.ExecuteNonQuery();
                    break;
            }
            SqlCommand.Connection.Close();
            Clear();
        }

        public object ExecuteScalar()
        {
            object command = null;
            SetCommand();
            SqlCommand.Connection.Open();
            switch (SqlContainer.RdsProvider)
            {
                case "Azure":
                    command = SqlCommand.ExecuteScalarWithRetry();
                    break;
                default:
                    command = SqlCommand.ExecuteScalar();
                    break;
            }
            SqlCommand.Connection.Close();
            Clear();
            return command;
        }

        public DataTable ExecuteTable()
        {
            var dataTable = new DataTable();
            SetCommand();
            switch (SqlContainer.RdsProvider)
            {
                case "Azure":
                    var retryPolicy = Azures.RetryPolicy();
                    retryPolicy.ExecuteAction(() =>
                    {
                        using (var con = new ReliableSqlConnection(
                            SqlCommand.Connection.ConnectionString))
                        {
                            con.Open(retryPolicy);
                            SqlCommand.Connection = con.Current;
                            new SqlDataAdapter(SqlCommand).Fill(dataTable);
                        }
                    });
                    break;
                default:
                    new SqlDataAdapter(SqlCommand).Fill(dataTable);
                    break;
            }
            Clear();
            return dataTable;
        }

        public DataSet ExecuteDataSet()
        {
            var dataSet = new DataSet();
            SetCommand();
            switch (SqlContainer.RdsProvider)
            {
                case "Azure":
                    var retryPolicy = Azures.RetryPolicy();
                    retryPolicy.ExecuteAction(() =>
                    {
                        using (var con = new ReliableSqlConnection(
                            SqlCommand.Connection.ConnectionString))
                        {
                            con.Open(retryPolicy);
                            SqlCommand.Connection = con.Current;
                            SqlContainer.SqlDataAdapter(SqlCommand).Fill(dataSet);
                        }
                    });
                    break;
                default:
                    SqlContainer.SqlDataAdapter(SqlCommand).Fill(dataSet);
                    break;
            }
            Clear();
            return dataSet;
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

        public string ExecuteScalar_string()
        {
            return ExecuteScalar().ToStr();
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

        public byte[] ExecuteScalar_bytes()
        {
            return (byte[])ExecuteScalar();
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
                if (this.disposed)
                {
                    return;
                }
                this.disposed = true;
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
