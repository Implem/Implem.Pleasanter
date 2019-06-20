using Implem.IRds;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    class SqlServerCommand : ISqlCommand
    {
        SqlCommand _instance;
        internal SqlCommand InnerInstance => _instance;

        public SqlServerCommand()
        {
            _instance = new SqlCommand();
        }

        public string CommandText { get => _instance.CommandText; set => _instance.CommandText = value; }
        public int CommandTimeout { get => _instance.CommandTimeout; set => _instance.CommandTimeout = value; }
        public CommandType CommandType { get => _instance.CommandType; set => _instance.CommandType = value; }
        public IDbConnection Connection { get => _instance.Connection;
            set
            {
                { if (value is SqlConnection con) _instance.Connection = con; }
                { if (value is SqlServerConnection con) _instance.Connection = con.InnerInstance; }
            }
        }

        public IDataParameterCollection Parameters => _instance.Parameters;

        public IDbTransaction Transaction { get => _instance.Transaction; set => _instance.Transaction = (SqlTransaction)value; }
        public UpdateRowSource UpdatedRowSource { get => _instance.UpdatedRowSource; set => _instance.UpdatedRowSource = value; }

        public void Cancel()
        {
            _instance.Cancel();
        }

        public object Clone()
        {
            return _instance.Clone();
        }

        public IDbDataParameter CreateParameter()
        {
            return _instance.CreateParameter();
        }

        public void Dispose()
        {
            _instance.Dispose();
        }

        public int ExecuteNonQuery()
        {
            return _instance.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            return _instance.ExecuteReader();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return _instance.ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            return _instance.ExecuteScalar();
        }

        public void Prepare()
        {
            _instance.Prepare();
        }

        public void Parameters_AddWithValue(string parameterName, object value)
        {
            _instance.Parameters.AddWithValue(parameterName, value);
        }

        public void Parameters_Add(ISqlParameter parameter)
        {
            _instance.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.Value));
        }

        public IEnumerable<ISqlParameter> SqlParameters()
        {
            foreach(SqlParameter parameter in _instance.Parameters)
                yield return new SqlServerParameter(parameter);
        }
    }
}
