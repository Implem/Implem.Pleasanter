using Implem.IRds;
namespace Implem.SqlServer
{
    public class SqlServerObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors sqlErrors = new SqlServerErrors();
        private static ISqls sqls = new SqlServerSqls();
        private static ISqlCommandText sqlCommandText = new SqlServerCommandText();
        private static ISqlResult sqlResult = new SqlServerResult();

        public ISqlErrors SqlErrors
        {
            get
            {
                return sqlErrors;
            }
        }

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
                return sqls;
            }
        }

        public ISqlCommandText SqlCommandText
        {
            get
            {
                return sqlCommandText;
            }
        }

        public ISqlResult SqlResult
        {
            get
            {
                return sqlResult;
            }
        }
    }
}
