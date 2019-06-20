using Implem.IRds;
using Implem.SqlServer;
namespace Implem.SqlServer
{
    public class SqlServerObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors _sqlErrors = new SqlServerErrors();
        public ISqlErrors SqlErrors => _sqlErrors;

        public ISqlCommand CreateSqlCommand()
        {
            return new SqlServerCommand();
        }

        public ISqlConnection CreateSqlConnection(string connectionString)
        {
            return new SqlServerConnection(connectionString);
        }

        public ISqlDataAdapter CreateSqlDataAdapter(ISqlCommand sqlCommand)
        {
            return new SqlServerDataAdapter(sqlCommand);
        }

        public ISqlParameter CreateSqlParameter()
        {
            return new SqlServerParameter();
        }

        public ISqlParameter CreateSqlParameter(string name, object value)
        {
            return new SqlServerParameter(name, value);
        }
    }
}
