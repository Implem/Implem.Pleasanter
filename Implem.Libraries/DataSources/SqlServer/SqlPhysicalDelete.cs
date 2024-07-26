using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Linq;
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
            commandText.Append(Statement());
            SetMainQueryInfoForSub();
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

        private string Statement()
        {
            return "delete from " + GetTableBracketText() + " ";
        }

        private string GetTableBracketText()
        {
            switch (TableType)
            {
                case Sqls.TableTypes.History:
                    return HistoryTableBracket;
                case Sqls.TableTypes.Deleted:
                    return DeletedTableBracket;
                default:
                    return TableBracket;
            }
        }

        private void SetMainQueryInfoForSub()
        {
            //サブクエリのselect文生成を行う際に、メイン（本クラスのこと）のクエリの情報を取得できるように、
            //あらかじめ情報をセットする処理
            SqlWhereCollection
                .Where(o => o.Sub != null)
                .ForEach(o => o.Sub.SetMainQueryInfo(
                    sqlClass: GetType().ToString(),
                    tableBracket: GetTableBracketText()));
        }
    }
}
