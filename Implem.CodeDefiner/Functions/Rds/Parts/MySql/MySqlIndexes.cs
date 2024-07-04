using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    //MySQL用の処理のみを集めた部品クラス。
    //MySQLはSQLServer・PostgreSQLとは異なる制約を多く有しているため、専用のクラスに独自の処理をまとめた。
    internal static class MySqlIndexes
    {
        internal static void General(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            List<IndexInfo> tableIndexCollection)
        {
            //MySQLの以下の制約下で、主キーおよびインデックス生成のコマンドを生成する。
            //・auto_incrementの制約→複合主キーかつ2番目以下に指定することは不可。
            //言い換えると、単独主キーとするか、複合主キーの先頭に指定する必要がある。
            //したがって、該当するカラムがあるテーブルでは、SQLServe・PostgresSQLとは異なる主キーを生成する。
            var needChangePk = Def.ColumnDefinitionCollection.Any(o =>
                o.TableName == generalTableName &&
                o.PkMySql > 0);
            if (needChangePk)
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Pk,
                    name: "Pk",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.PkMySql > 0)
                        .Select(o => new IndexInfo.Column(
                            o.ColumnName, o.PkMySql, o.PkOrderBy))
                        .ToList()));
            }
            else if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Pk > 0))
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
            //auto_incrementを指定するために単独主キーを生成するテーブルでは、インデックスの構成も他のDBMSとは異なり、
            //Definition_ColumnのPkの内容をIx1として生成し、Definition_ColumnのIx1～の内容を+1ずつシフトして生成する。
            if (needChangePk)
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix1",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Pk > 0)
                        .OrderBy(o => o.Pk)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Pk, o.PkOrderBy, o.Unique))
                        .ToList()));
            }
            if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Ix1 > 0))
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Ix,
                    name: "Ix" + (needChangePk ? "2" : "1"),
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
                    name: "Ix" + (needChangePk ? "3" : "2"),
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
                    name: "Ix" + (needChangePk ? "4" : "3"),
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
                    name: "Ix" + (needChangePk ? "5" : "4"),
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
                    name: "Ix" + (needChangePk ? "6" : "5"),
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.Ix5 > 0)
                        .OrderBy(o => o.Ix5)
                        .Select(o => new IndexInfo.Column(o.ColumnName, o.Ix5, o.Ix5OrderBy, o.Unique))
                        .ToList()));
            }
        }

        internal static void History(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            List<IndexInfo> tableIndexCollection)
        {
            //SQLServe・PostgresSQLとは異なる主キーを指定したテーブルは、_historyテーブルでもMySQL専用の主キーを指定する。
            var needChangePk = Def.ColumnDefinitionCollection.Any(o =>
                o.TableName == generalTableName &&
                o.PkMySql > 0);
            if (needChangePk)
            {
                tableIndexCollection.Add(new IndexInfo(
                    factory: factory,
                    tableName: sourceTableName,
                    type: IndexInfo.Types.Pk,
                    name: "PkHistory",
                    columnCollection: Def.ColumnDefinitionCollection
                        .Where(o => o.TableName == generalTableName)
                        .Where(o => o.PkHistoryMySql > 0)
                        .Select(o => new IndexInfo.Column(
                            o.ColumnName, o.PkHistoryMySql, o.PkHistoryOrderBy))
                        .ToList()));
            }
            else if (Def.ColumnDefinitionCollection.Any(o => o.TableName == generalTableName && o.Pk > 0))
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

        private static IEnumerable<DataRow> Get(ISqlObjectFactory factory, string sourceTableName)
        {
            return Def.SqlIoByAdmin(factory: factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.Indexes
                    .Replace("#TableName#", sourceTableName)
                    .Replace("#InitialCatalog#", Environments.ServiceName))
                        .AsEnumerable();

        }

        internal static bool HasChangesPkIx(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            Sqls.TableTypes tableType)
        {
            bool PkHasChange(
                IEnumerable<IndexInfo> defIndexColumnCollection,
                IEnumerable<DataRow> dbIndexColumnCollection)
            {
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
                IEnumerable<DataRow> dbIndexColumnCollection)
            {
                return defIndexColumnCollection
                    .Where(o => !o.IndexName().StartsWith("Pk"))
                    .Select(o => o.IndexName())
                    .OrderBy(o => o)
                    .Join(",") != dbIndexColumnCollection
                        .Where(o => o["Name"].ToString() != "PRIMARY")
                        .Where(o => o["Name"].ToString() != "ftx")
                        .Select(o => o["Name"].ToString())
                        .OrderBy(o => o)
                        .Join(",");
            }
            //MySQLの以下の制約下で、主キーおよびインデックスを突合する。
            //・主キー制約の名称→'PRIMARY'という固定名称で命名され、DBのインデックス情報に登録される。
            //・fulltext indexはDBのインデックス情報に登録される。
            //→上記1点目の通り、主キーではSha512Cngハッシュを使用した命名ができないため、カラム名＋OrderBy値を並べた文字列を生成して変更を検知する。
            if (Parameters.Rds.DisableIndexChangeDetection) return false;
            var defIndexColumnCollection = Indexes.IndexInfoCollection(
                factory: factory,
                generalTableName: generalTableName,
                sourceTableName: sourceTableName,
                tableType: tableType);
            var dbIndexColumnCollection = Get(
                factory: factory,
                sourceTableName: sourceTableName);
            return PkHasChange(defIndexColumnCollection: defIndexColumnCollection,
                dbIndexColumnCollection: dbIndexColumnCollection) ||
                    IxHasChange(defIndexColumnCollection: defIndexColumnCollection,
                        dbIndexColumnCollection: dbIndexColumnCollection);
        }
    }
}
