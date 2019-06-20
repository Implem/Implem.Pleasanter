using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlJoinCollection : ListEx<SqlJoin>
    {
        public SqlJoinCollection(params SqlJoin[] sqlJoinCollection)
        {
            sqlJoinCollection.ForEach(sqlJoin => Add(sqlJoin));
        }

        public SqlJoinCollection Add(
            string tableName,
            SqlJoin.JoinTypes joinType = SqlJoin.JoinTypes.Inner,
            string joinExpression = null,
            string _as = null)
        {
            Add(new SqlJoin(tableName, joinType, joinExpression, _as));
            return this;
        }

        public void BuildCommandText(StringBuilder commandText)
        {
            ForEach(sqlFrom =>
            {
                if (sqlFrom.JoinExpression != null)
                {
                    switch (sqlFrom.JoinType)
                    {
                        case SqlJoin.JoinTypes.Inner:
                            commandText.Append("inner join ");
                            break;
                        case SqlJoin.JoinTypes.LeftOuter:
                            commandText.Append("left outer join ");
                            break;
                        case SqlJoin.JoinTypes.RightOuter:
                            commandText.Append("right outer join ");
                            break;
                        default:
                            break;
                    }
                }
                commandText.Append(sqlFrom.TableBracket, " ");
                if (!sqlFrom.As.IsNullOrEmpty())
                {
                    commandText.Append(
                        "as [",
                        sqlFrom.As,
                        "] ");
                }
                if (sqlFrom.JoinExpression != null)
                {
                    commandText.Append(
                        "on ",
                        sqlFrom.JoinExpression,
                        " ");
                }
            });
        }
    }
}
