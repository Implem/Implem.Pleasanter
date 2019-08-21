using Implem.IRds;
using Npgsql;
using System.Data;
namespace Implem.PostgreSql
{
    internal class PostgreSqlParameter : ISqlParameter
    {
        private NpgsqlParameter instance;

        internal PostgreSqlParameter()
        {
            instance = new NpgsqlParameter();
        }

        internal PostgreSqlParameter(string parameterName, object value)
        {
            if (value is System.Enum)
            {
                value = (int)value;
            }
            instance = new NpgsqlParameter(parameterName, value);
        }

        internal PostgreSqlParameter(NpgsqlParameter parameter)
        {
            if (parameter?.Value is System.Enum)
            {
                parameter.Value = (int)parameter.Value;
            }
            instance = parameter;
        }

        public string SqlDbType
        {
            get
            {
                try
                {
                    return instance.NpgsqlDbType.ToString();
                }
                catch
                {
                }
                return string.Empty;
            }
        }

        public byte Precision
        {
            get
            {
                return instance.Precision;
            }
            set
            {
                instance.Precision = value;
            }
        }

        public byte Scale
        {
            get
            {
                return instance.Scale;
            }
            set
            {
                instance.Scale = value;
            }
        }

        public int Size
        {
            get
            {
                return instance.Size;
            }
            set
            {
                instance.Size = value;
            }
        }

        public DbType DbType
        {
            get
            {
                return instance.DbType;
            }
            set
            {
                instance.DbType = value;
            }
        }

        public ParameterDirection Direction
        {
            get
            {
                return instance.Direction;
            }
            set
            {
                instance.Direction = value;
            }
        }

        public bool IsNullable
        {
            get
            {
                return instance.IsNullable;
            }
        }

        public string ParameterName
        {
            get
            {
                return instance.ParameterName;
            }
            set
            {
                instance.ParameterName = value;
            }
        }

        public string SourceColumn
        {
            get
            {
                return instance.SourceColumn;
            }
            set
            {
                instance.SourceColumn = value;
            }
        }

        public DataRowVersion SourceVersion
        {
            get
            {
                return instance.SourceVersion;
            }
            set
            {
                instance.SourceVersion = value;
            }
        }

        public object Value
        {
            get
            {
                return instance.Value;
            }
            set
            {
                instance.Value = value;
            }
        }
    }
}
