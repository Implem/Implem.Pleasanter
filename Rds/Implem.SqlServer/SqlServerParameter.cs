using Implem.IRds;
using System.Data;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    internal class SqlServerParameter : ISqlParameter
    {
        private SqlParameter instance;

        public SqlServerParameter()
        {
            instance = new SqlParameter();
        }

        public SqlServerParameter(string parameterName, object value)
        {
            instance = new SqlParameter(parameterName, value);
        }

        internal SqlServerParameter(SqlParameter parameter)
        {
            instance = parameter;
        }

        public string SqlDbType
        {
            get
            {
                return instance.SqlDbType.ToString();
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
