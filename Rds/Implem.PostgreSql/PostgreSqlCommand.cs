using Implem.IRds;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Implem.PostgreSql
{
    class PostgreSqlCommand : ISqlCommand
    {
        NpgsqlCommand _instance;
        internal NpgsqlCommand InnerInstance => _instance;

        public PostgreSqlCommand()
        {
            _instance = new NpgsqlCommand();
        }

        public string CommandText
        {
            get => _instance.CommandText;
            set
            {
                _instance.CommandText = value;
            }
        }
        public int CommandTimeout { get => _instance.CommandTimeout; set => _instance.CommandTimeout = value; }
        public CommandType CommandType { get => _instance.CommandType; set => _instance.CommandType = value; }
        public IDbConnection Connection { get => _instance.Connection;
            set
            {
                { if (value is NpgsqlConnection con) _instance.Connection = con; }
                { if (value is PostgreSqlConnection con) _instance.Connection = con.InnerInstance; }
            }
        }

        public IDataParameterCollection Parameters => _instance.Parameters;
        public IDbTransaction Transaction { get => _instance.Transaction; set => _instance.Transaction = (NpgsqlTransaction)value; }
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

        //TODO
        public void Parameters_AddWithValue(string parameterName, object value)
        {
            if (parameterName?.StartsWith("_") == true)
                parameterName = $"ip{parameterName?.Substring(1)}";
            _instance.Parameters.AddWithValue(parameterName, value);
        }

        public void Parameters_Add(ISqlParameter parameter)
        {
            _instance.Parameters.Add(new NpgsqlParameter(parameter.ParameterName, parameter.Value));
        }

        public IEnumerable<ISqlParameter> SqlParameters()
        {
            foreach(NpgsqlParameter parameter in _instance.Parameters)
                yield return new PostgreSqlParameter(parameter);
        }
    }
}
