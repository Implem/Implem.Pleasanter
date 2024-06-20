﻿using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class Constraints
    {
        internal static EnumerableRowCollection<DataRow> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Defaults
                .Replace("#InitialCatalog#", Environments.ServiceName)
                .Replace("#TableName#", sourceTableName))
                    .AsEnumerable();
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            return
                columnDefinitionCollection
                    .Where(o => !o.Default.IsNullOrEmpty())
                    .Where(o => !(sourceTableName.EndsWith("_history") && o.ColumnName == "Ver"))
                    .OrderBy(o => o.ColumnName)
                    .Select(o => o.ColumnName + "," + DefaultDefinition(factory: factory, columnDefinition: o))
                    .JoinReturn() !=
                Get(factory: factory, sourceTableName: sourceTableName)
                    .Where(o => !(sourceTableName.EndsWith("_history") && o["column_name"].ToString() == "Ver"))
                    .OrderBy(o => o["column_name"])
                    .Select(o => o["column_name"] + "," + (Parameters.Rds.Dbms == "MySQL"
                        ? factory.SqlDataType.DefaultDefinitionByDataType(o)
                        : factory.SqlDataType.DefaultDefinition(o["column_default"])))
                    .JoinReturn();
        }

        internal static void CreateDefault(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            string tableNameTemp = "")
        {
            //MySQLの場合はここではなくCreateModifyColumnで既定値を設定する。
            if (Parameters.Rds.Dbms == "MySQL") return;
            sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                "#Defaults#", columnDefinitionCollection
                    .Where(o => !o.Default.IsNullOrEmpty())
                    .Where(o => !(sourceTableName.EndsWith("_history") && o.ColumnName == "Ver"))
                    .Select(o => Sql_Create(factory, Def.Sql.CreateDefault, Strings.CoalesceEmpty(tableNameTemp, sourceTableName), o))
                    .JoinReturn());
        }

        private static string Sql_Create(
            ISqlObjectFactory factory,
            string sqlCreateDefault,
            string sourceTableName,
            ColumnDefinition columnDefinition)
        {
            return sqlCreateDefault
                .Replace("#DefaultName#", sourceTableName + Strings.NewGuid())
                .Replace("#ColumnName#", columnDefinition.ColumnName)
                .Replace("#DefaultValue#", DefaultDefinition(factory, columnDefinition));
        }

        internal static string DefaultDefinition(ISqlObjectFactory factory, ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TypeName.CsTypeSummary())
            {
                case Types.CsString:
                    return $"'{columnDefinition.Default}'";
                case Types.CsDateTime:
                    if (columnDefinition.Default?.ToLower() == "now")
                    {
                        return factory.Sqls.CurrentDateTime.Trim();
                    }
                    else
                    {
                        return columnDefinition.Default;
                    }
                case Types.CsBool:
                    return factory.Sqls.BooleanString(columnDefinition.Default);
                default:
                    return columnDefinition.Default;
            }
        }

        internal static void DropConstraint(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            sqlStatement.CommandText = sqlStatement.CommandText
                .Replace("#DropConstraint#", tableIndexCollection
                    .Where(o => Indexes.Get(
                        factory: factory,
                        sourceTableName: sourceTableName)
                        .Contains(o.IndexName()))
                    .Select(o => Sql_Drop(o)
                        .Replace("#SourceTableName#", sourceTableName)
                        .Replace("#IndexName#", o.IndexName()))
                    .Join("\r\n"));
            if (Parameters.Rds.Dbms == "MySQL")
            {
                //MySQLに、PKに紐づくインデックスの名称が'PRIMARY'固定になる仕様がある。
                //該当テーブルに'PRIMARY'というインデックスが存在する場合はPK制約削除用のコマンドを追記する。
                sqlStatement.CommandText = sqlStatement.CommandText.Replace("#DropPRIMARY#",
                    Indexes.Get(factory: factory, sourceTableName: sourceTableName).Contains("PRIMARY")
                        ? Def.Sql.DropConstraint.Replace("#SourceTableName#", sourceTableName)
                        : string.Empty);
                //auto_incrementがついているカラムについては、dropの前にalter tableでauto_incrementを除去する。
                sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                    "#DropAutoIncrement#", Columns.GetModifyColumnSqls(
                        factory: factory,
                        sourceTableName: sourceTableName,
                        columnDefinitionCollection: columnDefinitionCollection,
                        dropAutoIncrement: true));
            }
        }

        private static string Sql_Drop(IndexInfo o)
        {
            return o.Type == IndexInfo.Types.Pk
                ? Def.Sql.DropConstraint
                : Def.Sql.DropIndex;
        }
    }
}
