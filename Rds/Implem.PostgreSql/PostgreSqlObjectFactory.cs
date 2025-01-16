using Implem.IRds;
namespace Implem.PostgreSql
{
    public class PostgreSqlObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors sqlErrors = new PostgreSqlErrors();
        private static ISqls sqls = new PostgreSqlSqls();
        private static ISqlCommandText sqlCommandText = new PostgreSqlCommandText();
        private static ISqlResult sqlResult = new PostgreSqlResult();
        private static ISqlDataType sqlDataTypes = new PostgreSqlDataType();
        private static ISqlDefinitionSetting sqlDefinitionSetting = new PostgreSqlDefinitionSetting();

        public ISqlErrors SqlErrors
        {
            get
            {
                return sqlErrors;
            }
        }

        public ISqlCommand CreateSqlCommand()
        {
            return new PostgreSqlCommand();
        }

        public ISqlConnection CreateSqlConnection(string connectionString)
        {
            return new PostgreSqlConnection(connectionString);
        }

        public ISqlConnectionStringBuilder CreateSqlConnectionStringBuilder(string connectionString)
        {
            return new PostgreSqlConnectionStringBuilder(connectionString);
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

        public ISqlDataType SqlDataType
        {
            get
            {
                return sqlDataTypes;
            }
        }

        public ISqlDefinitionSetting SqlDefinitionSetting
        {
            get
            {
                return sqlDefinitionSetting;
            }
        }
    }
}
