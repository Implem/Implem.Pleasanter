using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlSelect : SqlStatement
    {
        public string As;
        public bool Distinct;
        public int Top;
        public int PageSize;
        public int Offset;
        public bool Union;

        public SqlSelect()
        {
        }

        public override void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount = null)
        {
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    AddTableTypeColumn("History");
                    BuildHistoryWithoutFlag(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.HistoryWithoutFlag:
                    BuildHistoryWithoutFlag(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.NormalAndDeleted:
                    BuildNormalAndDeleted(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    break;
                case Sqls.TableTypes.NormalAndHistory:
                    BuildNormalAndHistory(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    break;
                case Sqls.TableTypes.Deleted:
                    BuildDeleted(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.None);
                    break;
                case Sqls.TableTypes.All:
                    AddTableTypeColumn("Normal");
                    BuildNormal(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    BuildDeleted(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.Union);
                    AddTableTypeColumn("History");
                    BuildHistoryWithoutFlag(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount,
                        unionType: Sqls.UnionTypes.Union);
                    break;
                default:
                    BuildNormal(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandText: commandText,
                        commandCount: commandCount);
                    break;
            }
        }

        private void BuildHistoryWithoutFlag(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            Sqls.UnionTypes unionType)
        {
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.History,
                unionType: unionType,
                orderBy: true,
                commandCount: commandCount);
        }

        private void BuildNormalAndDeleted(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            AddTableTypeColumn("Normal");
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: Sqls.UnionTypes.None,
                orderBy: false,
                commandCount: commandCount);
            BuildDeleted(
                factory,
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Sqls.UnionTypes.Union);
        }

        private void BuildNormalAndHistory(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            AddTableTypeColumn("Normal");
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: Sqls.UnionTypes.None,
                orderBy: false,
                commandCount: commandCount);
            AddTableTypeColumn("History");
            BuildHistoryWithoutFlag(
                factory,
                sqlContainer,
                sqlCommand,
                commandText,
                commandCount,
                Sqls.UnionTypes.Union);
        }

        private void BuildDeleted(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            Sqls.UnionTypes unionType)
        {
            AddTableTypeColumn("Deleted");
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Deleted,
                unionType: unionType,
                orderBy: true,
                commandCount: commandCount);
        }

        private void BuildNormal(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount)
        {
            BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: Sqls.TableTypes.Normal,
                unionType: UnionType,
                orderBy: true,
                commandCount: commandCount);
        }

        private void AddTableTypeColumn(string type)
        {
            var history = type == "History" ? "1" : "0";
            var deleted = type == "Deleted" ? "1" : "0";
            switch (TableType)
            {
                case Sqls.TableTypes.NormalAndDeleted:
                    SqlColumnCollection?.Add(new SqlColumn(
                        deleted + " as [IsDeleted]", adHoc: true));
                    break;
                case Sqls.TableTypes.NormalAndHistory:
                    SqlColumnCollection?.Add(new SqlColumn(
                        history + " as [IsHistory]", adHoc: true));
                    break;
                case Sqls.TableTypes.All:
                    SqlColumnCollection?.Add(new SqlColumn(
                        deleted + " as [IsDeleted]", adHoc: true));
                    SqlColumnCollection?.Add(new SqlColumn(
                        history + " as [IsHistory]", adHoc: true));
                    break;
                default:
                    break;
            }
        }

        private void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            Sqls.UnionTypes unionType,
            bool orderBy,
            int? commandCount)
        {
            if (!Using) return;
            AddUnion(commandText, unionType);
            SqlColumnCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                distinct: Distinct,
                top: Top);
            var from = From(tableType, As);
            commandText.Append(from);
            SqlJoinCollection?.BuildCommandText(commandText: commandText);
            SqlWhereCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                select: true);
            SqlGroupByCollection?.BuildCommandText(
                commandText: commandText);
            SqlHavingCollection?.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            if (orderBy)
            {
                SqlOrderByCollection?.BuildCommandText(
                    commandText: commandText,
                    pageSize: PageSize,
                    tableType: TableType,
                    commandCount: commandCount);
            }
            AddTermination(commandText);
            AddParams_Where(factory, sqlCommand, commandCount);
            AddParams_Having(factory, sqlCommand, commandCount);
            AddParams_Paging(factory, sqlCommand, commandCount);
            AddParams_Param(factory, sqlCommand, commandCount);
        }

        private void AddParams_Paging(ISqlObjectFactory factory, ISqlCommand sqlCommand, int? commandCount)
        {
            if (PageSize != 0)
            {
                AddParam(factory, sqlCommand, "_Offset", Offset, commandCount);
                AddParam(factory, sqlCommand, "_PageSize", PageSize, commandCount);
            }
        }

        private string From(Sqls.TableTypes tableType, string _as)
        {
            var tableBlacket = TableBracket;
            switch (tableType)
            {
                case Sqls.TableTypes.History:
                    tableBlacket = HistoryTableBracket;
                    break;
                case Sqls.TableTypes.Deleted:
                    tableBlacket = DeletedTableBracket;
                    break;
            }
            return "from " + tableBlacket + (!_as.IsNullOrEmpty()
                ? " as [" + _as + "]"
                : " as " + TableBracket) + "\n";
        }
    }
}
