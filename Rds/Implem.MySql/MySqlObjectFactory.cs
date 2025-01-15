using Implem.IRds;
using System.Data;
namespace Implem.MySql
{
    public class MySqlObjectFactory : ISqlObjectFactory
    {
        private static ISqlErrors sqlErrors = new MySqlErrors();
        private static ISqls sqls = new MySqlSqls();
        private static ISqlCommandText sqlCommandText = new MySqlCommandText();
        private static ISqlResult sqlResult = new MySqlResult();
        private static ISqlDataType sqlDataTypes = new MySqlDataType();
        private static ISqlDefinitionSetting sqlDefinitionSetting = new MySqlDefinitionSetting();

        public ISqlErrors SqlErrors
        {
            get
            {
                return sqlErrors;
            }
        }

        public ISqlCommand CreateSqlCommand()
        {
            return new MySqlCommand();
        }

        public ISqlCommand CreateSqlCommand(
            string cmdText,
            IDbConnection connection)
        {
            return new MySqlCommand(cmdText, connection);
        }

        public ISqlConnection CreateSqlConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public ISqlConnectionStringBuilder CreateSqlConnectionStringBuilder(string connectionString)
        {
            return new MySqlConnectionStringBuilder(connectionString);
        }

        public ISqlDataAdapter CreateSqlDataAdapter(ISqlCommand sqlCommand)
        {
            return new MySqlDataAdapter(sqlCommand);
        }

        public ISqlParameter CreateSqlParameter()
        {
            return new MySqlParameter();
        }

        public ISqlParameter CreateSqlParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
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
