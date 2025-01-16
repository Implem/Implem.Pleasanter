namespace Implem.IRds
{
    public interface ISqlObjectFactory
    {
        ISqlCommand CreateSqlCommand();

        ISqlDataAdapter CreateSqlDataAdapter(ISqlCommand sqlCommand);

        ISqlParameter CreateSqlParameter();

        ISqlParameter CreateSqlParameter(string name, object value);

        ISqlConnection CreateSqlConnection(string connectionString);

        ISqlConnectionStringBuilder CreateSqlConnectionStringBuilder(string connectionString);

        ISqls Sqls { get; }

        ISqlCommandText SqlCommandText { get; }

        ISqlResult SqlResult { get; }

        ISqlErrors SqlErrors { get; }

        ISqlDataType SqlDataType { get; }

        ISqlDefinitionSetting SqlDefinitionSetting { get; }
    }
}
