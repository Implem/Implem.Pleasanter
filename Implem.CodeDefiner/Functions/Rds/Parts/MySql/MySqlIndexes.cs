using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    //MySQL用の処理のみ集めた部品クラス。
    //MySQLはSQLServer・PostgreSQLとは異なる制約を多く有しているため、専用のクラスに独自の処理をまとめた。
    internal class MySqlIndexes
    {
        internal static void General(
            ISqlObjectFactory factory,
            string generalTableName,
            string sourceTableName,
            List<IndexInfo> tableIndexCollection)
        {
            //MySQLではauto_incrementの制約により、いくつかのテーブルの主キーを他のDBMSと異なる定義※にする必要がある。
            //(※…auto_incrementを設定するカラムによる単独の主キー)
            //他のDBMSと異なる主キーを定義するかは、PkMySqlを設定済みのカラムが存在するか否かで判定する。
            //主キーを変える必要がないテーブルは、他のDBMSと同様の方法で主キーを定義する。
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
            //他のDBMSと異なる主キーを定義するテーブルでは、インデックスの構成も他のDBMSとは異なり、
            //Definition_ColumnのPkの内容をIx1に定義し、Definition_ColumnのIx1～の内容を+1ずつシフトして定義する。
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
            //MySQLではauto_incrementの制約により、いくつかのテーブルの主キーを他のDBMSと異なる定義※にする必要がある。
            //(※…auto_incrementを設定するカラムによる単独の主キー)
            //該当テーブルの_historyテーブルは、PkHistoryMySqlの値を参照して独自の主キーを定義する。
            //主キーを変える必要がないテーブルは、他のDBMSと同様の方法で主キーを定義する。
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
                        .Where(o => o.PkHistoryMySql < 0 || o.PkHistoryMySql > 0)
                        .Select(o => new IndexInfo.Column(
                            o.ColumnName, o.PkHistoryMySql < 0 ? -(o.PkHistoryMySql) : o.PkHistoryMySql, o.PkHistoryOrderBy))
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
    }
}
