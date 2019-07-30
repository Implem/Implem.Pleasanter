using Implem.IRds;
using Implem.SqlServer;
namespace Implem.SqlServer
{
    public class SqlServerObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors _sqlErrors = new SqlServerErrors();
        private static ISqls _sqls = new SqlServerSqls();
        private static ISqlCommandText _sqlCommandText = new SqlServerCommandText();

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

        public ISqls Sqls
        {
            get
            {
                return _sqls;
            }
        }

        public ISqlCommandText SqlCommandText
        {
            get
            {
                return _sqlCommandText;
            }
        }
    }
}
