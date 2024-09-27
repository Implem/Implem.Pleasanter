using Implem.IRds;
using System.Data;
namespace Implem.MySql
{
    internal class MySqlParameter : ISqlParameter
    {
        private MySqlConnector.MySqlParameter instance;

        internal MySqlParameter()
        {
            instance = new MySqlConnector.MySqlParameter();
        }

        internal MySqlParameter(string parameterName, object value)
        {
            if (value is System.Enum)
            {
                value = (int)value;
            }
            instance = new MySqlConnector.MySqlParameter(parameterName, value);
        }

        internal MySqlParameter(MySqlConnector.MySqlParameter parameter)
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
                    return instance.MySqlDbType.ToString();
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
