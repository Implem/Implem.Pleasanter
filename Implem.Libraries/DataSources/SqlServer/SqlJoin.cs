namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlJoin
    {
        public string TableName;
        public JoinTypes JoinType;
        public string JoinExpression;
        public string As;

        public enum JoinTypes
        {
            Inner,
            LeftOuter,
            RightOuter
        }

        public SqlJoin(
            string tableName,
            JoinTypes joinType = JoinTypes.Inner,
            string joinExpression = null,
            string _as = null)
        {
            TableName = tableName;
            JoinType = joinType;
            JoinExpression = joinExpression;
            As = _as;
        }
    }
}
