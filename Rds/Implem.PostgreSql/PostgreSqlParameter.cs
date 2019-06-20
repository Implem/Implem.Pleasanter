using Implem.IRds;
using Npgsql;
using System.Data;
namespace Implem.PostgreSql
{
    class PostgreSqlParameter : ISqlParameter
    {
        NpgsqlParameter _instance;

        public PostgreSqlParameter()
        {
            _instance = new NpgsqlParameter();
        }

        public PostgreSqlParameter(string parameterName, object value)
        {
            //TODO
            if(parameterName?.StartsWith("_") == true)
                parameterName = $"ip{parameterName?.Substring(1)}";
            if(value is System.Enum)
                value = (int)value;
            _instance = new NpgsqlParameter(parameterName, value);
        }

        internal PostgreSqlParameter(NpgsqlParameter parameter)
        {
            //TODO
            if(parameter != null)
                if (parameter.ParameterName?.StartsWith("_") == true)
                    parameter.ParameterName = $"ip{parameter.ParameterName?.Substring(1)}";
            if (parameter?.Value is System.Enum)
                parameter.Value = (int)parameter.Value;
            _instance = parameter;
        }

        //TODO
        public string SqlDbType
        {
            get
            {
                try
                {
                    return _instance.NpgsqlDbType.ToString();
                }
                catch {; }
                return "";
            }
        }

        public byte Precision { get => _instance.Precision; set => _instance.Precision = value; }
        public byte Scale { get => _instance.Scale; set => _instance.Scale = value; }
        public int Size { get => _instance.Size; set => _instance.Size = value; }
        public DbType DbType { get => _instance.DbType; set => _instance.DbType = value; }
        public ParameterDirection Direction { get => _instance.Direction; set => _instance.Direction = value; }

        public bool IsNullable => _instance.IsNullable;

        //TODO
        public string ParameterName {
            get => _instance.ParameterName;
            set
            {
                var parameterName = value;
                if (parameterName?.StartsWith("_") == true)
                    parameterName = $"ip{parameterName?.Substring(1)}";
                _instance.ParameterName = parameterName;
            }
        }
        public string SourceColumn { get => _instance.SourceColumn; set => _instance.SourceColumn = value; }
        public DataRowVersion SourceVersion { get => _instance.SourceVersion; set => _instance.SourceVersion = value; }
        public object Value { get => _instance.Value; set => _instance.Value = value; }
    }
}
