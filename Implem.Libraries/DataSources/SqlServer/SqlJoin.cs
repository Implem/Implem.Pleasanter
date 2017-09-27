namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlJoin
    {
        public string TableBracket;
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
            string tableBracket,
            JoinTypes joinType = JoinTypes.Inner,
            string joinExpression = null,
            string _as = null)
        {
            TableBracket = tableBracket;
            JoinType = joinType;
            JoinExpression = joinExpression;
            As = _as;
        }
    }
}
