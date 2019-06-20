using Implem.IRds;
namespace Implem.PostgreSql
{
    public class PostgreSqlObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors _sqlErrors = new PostgreSqlErrors();
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
    }
}
