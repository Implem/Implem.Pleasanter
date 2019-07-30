using Implem.IRds;
namespace Implem.PostgreSql
{
    public class PostgreSqlObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors _sqlErrors = new PostgreSqlErrors();
        private static ISqls _sqls = new PostgreSqlSqls();
        private static ISqlCommandText _sqlCommandText = new PostgreSqlCommandText();

        public ISqlErrors SqlErrors => _sqlErrors;

        public ISqlCommand CreateSqlCommand()
        {
            return new PostgreSqlCommand();
        }

        public ISqlConnection CreateSqlConnection(string connectionString)
        {
            return new PostgreSqlConnection(connectionString);
        }

        public ISqlDataAdapter CreateSqlDataAdapter(ISqlCommand sqlCommand)
        {
            return new PostgreSqlDataAdapter(sqlCommand);
        }

        public ISqlParameter CreateSqlParameter()
        {
            return new PostgreSqlParameter();
        }

        public ISqlParameter CreateSqlParameter(string name, object value)
        {
            return new PostgreSqlParameter(name, value);
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
