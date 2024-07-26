﻿using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlDelete : SqlStatement
    {
        public SqlDelete()
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
            Build_If(commandText: commandText);
            SetMainQueryInfoForSub();
            Build_DeleteStatement(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            AddParams_Where(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddParams_Param(
                factory: factory,
                sqlCommand: sqlCommand,
                commandCount: commandCount);
            AddTermination(commandText: commandText);
            Build_EndIf(commandText: commandText);
        }

        private void Build_DeleteStatement(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer, 
            ISqlCommand sqlCommand, 
            StringBuilder commandText, 
            int? commandCount)
        {
            commandText.Append(factory.SqlCommandText.CreateDelete(
                template: CommandText
                .Params(SqlWhereCollection.Sql(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount))));
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
