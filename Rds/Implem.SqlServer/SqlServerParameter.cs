using Implem.IRds;
using System.Data;
using System.Data.SqlClient;
namespace Implem.SqlServer
{
    class SqlServerParameter : ISqlParameter
    {
        SqlParameter _instance;

        public SqlServerParameter()
        {
            _instance = new SqlParameter();
        }

        public SqlServerParameter(string parameterName, object value)
        {
            _instance = new SqlParameter(parameterName, value);
        }

        internal SqlServerParameter(SqlParameter parameter)
        {
            _instance = parameter;
        }

        public string SqlDbType => _instance.SqlDbType.ToString();

        public byte Precision { get => _instance.Precision; set => _instance.Precision = value; }
        public byte Scale { get => _instance.Scale; set => _instance.Scale = value; }
        public int Size { get => _instance.Size; set => _instance.Size = value; }
        public DbType DbType { get => _instance.DbType; set => _instance.DbType = value; }
        public ParameterDirection Direction { get => _instance.Direction; set => _instance.Direction = value; }

        public bool IsNullable => _instance.IsNullable;

        public string ParameterName { get => _instance.ParameterName; set => _instance.ParameterName = value; }
        public string SourceColumn { get => _instance.SourceColumn; set => _instance.SourceColumn = value; }
        public DataRowVersion SourceVersion { get => _instance.SourceVersion; set => _instance.SourceVersion = value; }
        public object Value { get => _instance.Value; set => _instance.Value = value; }
    }
}
