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
                commandText: Def.Sql.Indexes
                    .Replace("#TableName#", sourceTableName)
                    .Replace("#InitialCatalog#", Environments.ServiceName))
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
            //MySQLの場合はここではなくHasChangesMySqlメソッドでインデックスの変更を検知する。
            if (Parameters.Rds.Dbms == "MySQL") return false;
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

        internal static bool HasChangesMySql(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType)
        {
            bool PkHasChange(
                IEnumerable<IndexInfo> defIndexColumnCollection,
                EnumerableRowCollection<DataRow> dbIndexColumnCollection)
            {
                //PkのColumnName,OrderType, ... の列挙で生成した文字列が一致するか比較し、差分を検知する。
                return defIndexColumnCollection
                    .Where(o => o.IndexName().StartsWith("Pk"))
                    .FirstOrDefault()
                    .IndexInfoString() != dbIndexColumnCollection
                        .Where(o => o["Name"].ToString() == "PRIMARY")
                        .OrderBy(o => o["No"].ToInt())
                        .Select(o => o["ColumnName"] + "," + o["OrderType"].ToString())
                        .Join(",");
            }
            bool IxHasChange(
                IEnumerable<IndexInfo> defIndexColumnCollection,
                EnumerableRowCollection< DataRow > dbIndexColumnCollection)
            {
                //非Pkのインデックス名, ... の列挙で生成した文字列が一致するか比較し、差分を検知する。
                return defIndexColumnCollection
                    .Where(o => !o.IndexName().StartsWith("Pk"))
                    .Select(o => o.IndexName())
                    .Distinct()
                    .OrderBy(o => o)
                    .Join(",") != dbIndexColumnCollection
                        .Where(o => o["Name"].ToString() != "PRIMARY")
                        .Select(o => o["Name"].ToString())
                        .Distinct()
                        .OrderBy(o => o)
                        .Join(",");
            }
            if (Parameters.Rds.Dbms != "MySQL" || Parameters.Rds.DisableIndexChangeDetection) return false;
            var defIndexColumnCollection = IndexInfoCollection(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: sourceTableName,
                tableType: tableType);
            var dbIndexColumnCollection = Def.SqlIoByAdmin(factory: factory)
                .ExecuteTable(factory: factory,
                    commandText: Def.Sql.Indexes
                        .Replace("#TableName#", sourceTableName)
                        .Replace("#InitialCatalog#", Environments.ServiceName))
                .AsEnumerable();
            //MySQLの場合はDB上の主キー名が'PRIMARY'固定となり、Sha512Cngハッシュを使用した命名ができない。
            //そのためSQLServerおよびPostgreSQLとは異なり、
            //Pk情報比較処理（PkHasChange）と、非PkのIndex名比較処理（IxHasChange）に分割して差分を検知をする。
            return PkHasChange(defIndexColumnCollection: defIndexColumnCollection,
                dbIndexColumnCollection: dbIndexColumnCollection) ||
                    IxHasChange(defIndexColumnCollection: defIndexColumnCollection,
                        dbIndexColumnCollection: dbIndexColumnCollection);
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
                    .Select(o => $"\"{o.ColumnName}\"{(setOrderType ? $" {o.OrderType}" : string.Empty)}")
                    .Join(", ");
            }
            else
            {
                return string.Empty;
            }
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
                            sourceTableName, tableIndex, columnDefinitionCollection, false))));
            }
            else
            {
                sqlStatement.CommandText = sqlStatement.CommandText.Replace(
                    "#Pks#", string.Empty);
            }
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
