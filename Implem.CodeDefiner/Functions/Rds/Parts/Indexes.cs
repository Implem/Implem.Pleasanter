using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class Indexes
    {
        internal static IEnumerable<IndexInfo> IndexInfoCollection(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType)
        {
            var tableIndexCollection = new List<IndexInfo>();
            switch (tableType)
            {
                case Sqls.TableTypes.Normal:
                    General(factory: factory,
                        generalTableName: generalTableName,
                        sourceTableName: sourceTableName,
                        tableIndexCollection: tableIndexCollection);
                    Unique(factory: factory,
                        generalTableName: generalTableName,
                        tableIndexCollection: tableIndexCollection);
                    break;
                case Sqls.TableTypes.Deleted:
                    General(factory: factory,
                        generalTableName: generalTableName,
                        sourceTableName: sourceTableName,
                        tableIndexCollection: tableIndexCollection);
                    break;
                case Sqls.TableTypes.History:
                    History(factory: factory,
                        generalTableName: generalTableName,
                        sourceTableName: sourceTableName,
                        tableIndexCollection: tableIndexCollection);
                    break;
            }
            return tableIndexCollection;
        }

        private static void General(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            List<IndexInfo> tableIndexCollection)
        {
            if (Def.ColumnDefinitionCollection.Any(o =>
                o.TableName == generalTableName && o.Pk > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Pk,
                    name: "Pk",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Pk > 0)
                        .OrderBy(o => o.Pk)
                        .Select(o => new IndexInfo.Column(
                            o.ColumnName, o.Pk, o.PkOrderBy))
                        .ToList()));
            }
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix1 > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix1",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Ix1 > 0)
                        .OrderBy(o => o.Ix1)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix1, o.Ix1OrderBy, o.Unique))
                        .ToList()));
            }
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix2 > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix2",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Ix2 > 0)
                        .OrderBy(o => o.Ix2)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix2, o.Ix2OrderBy, o.Unique))
                        .ToList()));
            }
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix3 > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix3",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Ix3 > 0)
                        .OrderBy(o => o.Ix3)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix3, o.Ix3OrderBy, o.Unique))
                        .ToList()));
            }
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix4 > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix4",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Ix4 > 0)
                        .OrderBy(o => o.Ix4)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix4, o.Ix4OrderBy, o.Unique))
                        .ToList()));
            }
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix5 > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix5",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Ix5 > 0)
                        .OrderBy(o => o.Ix5)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix5, o.Ix5OrderBy, o.Unique))
                        .ToList()));
            }
        }

        private static void Unique(ISqlObjectFactory factory, string generalTableName, List<IndexInfo> tableIndexCollection)
        {
            Def.ColumnDefinitionCollection
                .Where(o => o.TableName == generalTableName)
                .Where(o => o.Pk == 0)
                .Where(o => o.Unique)
                .Select(o => o.ColumnName)
                .ForEach(columnName =>
                    tableIndexCollection.Add(new IndexInfo(
                        factory: factory,
                        tableName: generalTableName,
                        type: IndexInfo.Types.Ix,
                        name: "Unique",
                        columnCollection: new IndexInfo.Column(
                            columnName, 1, "asc", unique: true).ToSingleList())));
        }

        private static void History(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            List<IndexInfo> tableIndexCollection)
        {
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Pk > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Pk,
                    name: "PkHistory",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.History > 0)
                        .Where(o => o.PkHistory > 0)
                        .OrderBy(o => o.PkHistory)
                        .Select(o => new IndexInfo.Column(
                            o.ColumnName, o.PkHistory, o.PkHistoryOrderBy))
                        .ToList()));
            }
        }

        public static IEnumerable<string> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory: factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Indexes.Replace("#TableName#", sourceTableName))
                    .AsEnumerable()
                    .Select(o => o["Name"].ToString())
                    .Distinct()
                    .OrderBy(o => o);
        }

        internal static bool HasChanges(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType)
        {
            return !Parameters.Rds.DisableIndexChangeDetection
                && IndexInfoCollection(
                    factory: factory,
                    generalTableName: generalTableName,
                    sourceTableName: sourceTableName,
                    tableType: tableType)
                        .Select(o => o.IndexName())
                        .Distinct()
                        .OrderBy(o => o)
                        .Join(",") != Get(
                            factory: factory,
                            sourceTableName: sourceTableName)
                        .Join(",");
        }

        private static string Sql_CreateIx(
            string sourceTableName,
            IndexInfo tableIndex,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            bool setOrderType = true)
        {
            if (tableIndex != null)
            {
                return tableIndex.ColumnCollection
                    .Where(o => o.No > 0)
                    .Select(o => $"\"{o.ColumnName}\"{GetSmallKeyLength(o.ColumnName, columnDefinitionCollection)}{(setOrderType ? $" {o.OrderType}" : string.Empty)}")
                    .Join(", ");
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetSmallKeyLength(string columnName, IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            if (Parameters.Rds.Dbms != "MySQL") { return string.Empty; }
            ColumnDefinition columnDefinition = columnDefinitionCollection
                .Where(o => o.ColumnName == columnName)
                .Where(o => o.TypeName == "nvarchar")
                .Where(o => o.MaxLength == 1024)
                .FirstOrDefault();
            if (columnDefinition == null) { return string.Empty; }
            //MySQLでtext型カラムをIndexに指定している場合に、短いkey lengthの記述を追加する。
            //テーブルとカラムごとの個別指定を行う。
            if (columnDefinition.TableName == "ExportSettings" && columnDefinition.ColumnName == "Title")
            {
                return "(512)";
            }
            return string.Empty;
        }

        internal static void CreatePk(
            this SqlStatement sqlStatement,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IEnumerable<IndexInfo> tableIndexCollection)
        {
            var tableIndex = tableIndexCollection
                .Where(o => o.Type == IndexInfo.Types.Pk).FirstOrDefault();
            if (tableIndex != null)
            {
                sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                    "#Pks#", Def.Sql.CreatePk
                        .Replace("#PkName#", tableIndex.IndexName())
                        .Replace("#PkColumns#", string.Join(",", Sql_CreateIx(
                            sourceTableName, tableIndex, columnDefinitionCollection)))
                        .Replace("#PkColumnsWithoutOrderType#", string.Join(",", Sql_CreateIx(
                            sourceTableName, tableIndex, columnDefinitionCollection, false)))
                        .Replace("#AutoIncrement#", GetAutoIncrement())
                        .Replace("#AutoIncrementValue#", GetAutoIncrementValue()));
            }
            else
            {
                sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                    "#Pks#", string.Empty);
            }
        }

        private static string GetAutoIncrement()
        {
            //GenerateIdentityの代替として、MySQLではAutoIncrementを記述する
            //PrimaryKey制約が付与されたカラムに指定しなければエラーが返却されるため、CreatePkの中で記述する
            return "";
        }

        private static string GetAutoIncrementValue()
        {
            return "";
        }

        internal static void CreateIx(
            this SqlStatement sqlStatement,
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType,
            IEnumerable<ColumnDefinition> columnDefinitionCollection)
        {
            IndexInfoCollection(factory: factory,
                generalTableName: generalTableName,
                sourceTableName: sourceTableName,
                tableType: tableType)
                    .Where(o => o.Type == IndexInfo.Types.Ix)
                    .ForEach(tableIndex => 
                        sqlStatement.CreateIx(sourceTableName, columnDefinitionCollection, tableIndex));
        }

        private static void CreateIx(
            this SqlStatement sqlStatement,
            string sourceTableName,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            IndexInfo tableIndex)
        {
            sqlStatement.CommandText += Def.Sql.CreateIx
                .Replace("#IxName#", tableIndex.IndexName())
                .Replace("#IxColumns#", Sql_CreateIx(sourceTableName, tableIndex, columnDefinitionCollection))
                .Replace("#Unique#", Sql_CreateIxUnique(tableIndex));
        }

        private static string Sql_CreateIxUnique(IndexInfo tableIndex)
        {
            if (tableIndex.ColumnCollection.All(o => o.Unique))
            {
                return "unique";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
