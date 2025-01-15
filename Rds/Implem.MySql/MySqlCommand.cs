using Implem.IRds;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
namespace Implem.MySql
{
    internal class MySqlCommand : ISqlCommand
    {
        private MySqlConnector.MySqlCommand instance;

        internal MySqlConnector.MySqlCommand InnerInstance
        {
            get
            {
                return instance;
            }
        }

        public MySqlCommand()
        {
            instance = new MySqlConnector.MySqlCommand();
        }

        public MySqlCommand(
            string cmdText,
            IDbConnection connection)
        {
            instance = new MySqlConnector.MySqlCommand(
                commandText: cmdText,
                connection: (MySqlConnector.MySqlConnection)connection);
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
                if (value is MySqlConnector.MySqlConnection mySqlConnectorCon)
                {
                    instance.Connection = mySqlConnectorCon;
                }
                if (value is MySqlConnection mySqlCon)
                {
                    instance.Connection = mySqlCon.InnerInstance;
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
                instance.Transaction = (MySqlTransaction)value;
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
                new MySqlConnector.MySqlParameter(parameter.ParameterName, parameter.Value));
        }

        public IEnumerable<ISqlParameter> SqlParameters()
        {
            foreach (MySqlConnector.MySqlParameter parameter in instance.Parameters)
            {
                yield return new MySqlParameter(parameter);
            }
        }
    }
}
