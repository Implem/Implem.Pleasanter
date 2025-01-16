using Implem.IRds;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    internal class SqlServerCommand : ISqlCommand
    {
        private SqlCommand instance;

        internal SqlCommand InnerInstance
        {
            get
            {
                return instance;
            }
        }

        public SqlServerCommand()
        {
            instance = new SqlCommand();
        }

        public string CommandText
        {
            get
            {
                return instance.CommandText;
            }
            set
            {
                instance.CommandText = value;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return instance.CommandTimeout;
            }
            set
            {
                instance.CommandTimeout = value;
            }
        }

        public CommandType CommandType
        {
            get
            {
                return instance.CommandType;
            }
            set
            {
                instance.CommandType = value;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return instance.Connection;
            }
            set
            {
                if (value is SqlConnection sqlCon)
                {
                    instance.Connection = sqlCon;
                }
                if (value is SqlServerConnection sqlServerCon)
                {
                    instance.Connection = sqlServerCon.InnerInstance;
                }
            }
        }

        public IDataParameterCollection Parameters
        {
            get
            {
                return instance.Parameters;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                return instance.Transaction;
            }
            set
            {
                instance.Transaction = (SqlTransaction)value;
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                return instance.UpdatedRowSource;
            }
            set
            {
                instance.UpdatedRowSource = value;
            }
        }

        public void Cancel()
        {
            instance.Cancel();
        }

        public object Clone()
        {
            return instance.Clone();
        }

        public IDbDataParameter CreateParameter()
        {
            return instance.CreateParameter();
        }

        public void Dispose()
        {
            instance.Dispose();
        }

        public int ExecuteNonQuery()
        {
            return instance.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            return instance.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return instance.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            return instance.ExecuteScalar();
        }

        public void Prepare()
        {
            instance.Prepare();
        }

        public void Parameters_AddWithValue(string parameterName, object value)
        {
            instance.Parameters.AddWithValue(parameterName, value);
        }

        public void Parameters_Add(ISqlParameter parameter)
        {
            instance.Parameters.Add(
                new SqlParameter(parameter.ParameterName, parameter.Value));
        }

        public IEnumerable<ISqlParameter> SqlParameters()
        {
            foreach (SqlParameter parameter in instance.Parameters)
            {
                yield return new SqlServerParameter(parameter);
            }
        }
    }
}
