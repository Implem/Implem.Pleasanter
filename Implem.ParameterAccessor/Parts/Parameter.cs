using Implem.DefinitionAccessor;
using System.Runtime.Serialization;
namespace Implem.ParameterAccessor.Parts
{
    public class Parameter
    {
        public string SqlParameterPrefix;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            //PostgreSQLとMySQLは同じプレフィクスを使用する
            SqlParameterPrefix = string.IsNullOrWhiteSpace(SqlParameterPrefix)
                ? Parameters.Rds.Dbms == "SQLServer"
                    ? "@_"
                    : "@ip"
                : Parameters.Parameter.SqlParameterPrefix;
        }
    }
}
