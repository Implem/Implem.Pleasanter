namespace Implem.IRds
{
    public interface ISqlObjectFactory
    {
        ISqlCommand CreateSqlCommand();
        ISqlDataAdapter CreateSqlDataAdapter(ISqlCommand sqlCommand);
        ISqlParameter CreateSqlParameter();
        ISqlParameter CreateSqlParameter(string name, object value);
        ISqlConnection CreateSqlConnection(string connectionString);
        ISqls Sqls { get; }
        ISqlCommandText SqlCommandText { get; }
        ISqlErrors SqlErrors { get; }
    }
}
