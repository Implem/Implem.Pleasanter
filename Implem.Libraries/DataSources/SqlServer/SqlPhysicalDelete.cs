using Implem.IRds;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlPhysicalDelete : SqlStatement
    {
        public SqlPhysicalDelete()
        {
        }

        public override void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            if (!Using) return;
            Build_If(commandText);
            commandText.Append(Statement(commandCount));
            SqlWhereCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            AddParams_Where(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddTermination(commandText);
            Build_EndIf(commandText);
        }

        private string Statement(int? commandCount)
        {
            switch (TableType)
            {
                case Sqls.TableTypes.Normal:
                    return "delete from " + TableBracket + " ";
                case Sqls.TableTypes.History:
                    return "delete from " + HistoryTableBracket + " ";
                case Sqls.TableTypes.Deleted:
                    return "delete from " + DeletedTableBracket + " ";
                default:
                   return string.Empty;
            }
        }
    }
}
