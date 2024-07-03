using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    //MySQL用の処理のみを集めた部品クラス。
    //MySQLはSQLServer・PostgreSQLとは異なる制約を多く有しているため、専用のクラスに独自の処理をまとめた。
    internal static class MySqlConstraints
    {
        internal static string CreateModifyColumnCommand(
            ISqlObjectFactory factory,
            string tableNameTemp,
            IEnumerable<ColumnDefinition> columnDefinitionCollection,
            string command)
        {
            bool NeedsDefault(ColumnDefinition o)
            {
                return !o.Default.IsNullOrEmpty() &&
                    !(tableNameTemp.EndsWith("_history") && o.ColumnName == "Ver");
            }
            bool NeedsAutoIncrement(ColumnDefinition o)
            {
                return o.Identity &&
                    !tableNameTemp.EndsWith("_history") &&
                    !tableNameTemp.EndsWith("_deleted");
            }
            //MySQLの以下の制約下で、デフォルト値と自動採番機能を追加するためのコマンドを生成する。
            //・デフォルト値→デフォルト値が関数の場合はalter table ... alter columnの実行時にエラーになる。
            //・自動採番機能(auto_increment)→alter table ... alter columnでは追加や削除が不可。modify columnで追加する必要がある。
            //・自動採番機能(auto_increment)→主キー制約がない状態で追加しようとすると実行時にエラーになる。
            //以上の制約をクリアするため、主キーとインデックスの生成コマンドの実行後、modify columnを実行してデフォルト値と自動採番機能を追加する。
            return command.Replace(
                "#ModifyColumn#", columnDefinitionCollection
                    .Where(o => NeedsDefault(o) || NeedsAutoIncrement(o))
                    .Select(o => GetModifyColumnSqls(
                        factory: factory,
                        columnDefinition: o,
                        needsDefault: NeedsDefault(o),
                        needsAutoIncrement: NeedsAutoIncrement(o),
                        seed: o.Seed == 0 ? 1 : o.Seed))
                    .JoinReturn());
        }

        private static string GetModifyColumnSqls(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition,
            bool needsDefault,
            bool needsAutoIncrement,
            int seed)
        {
            //alter table ... modify columnは、カラム再定義のコマンドである。
            //alter table ... alter columnでは追加する属性のみコマンドに設定すれば実行可能であるのに対し、
            //modify columnの場合は追加する属性だけでなく、データ型を含む全属性の情報を記述する必要がある。
            return Def.Sql.ModifyColumn
                .Replace("#ColumnDefinition#", MySqlColumns.Sql_Create(
                    factory: factory,
                    columnDefinition: columnDefinition))
                .Replace("#Default#", needsDefault
                    ? " default " + Constraints.DefaultDefinition(factory, columnDefinition)
                    : string.Empty)
                .Replace("#AutoIncrement#", needsAutoIncrement
                    ? " auto_increment"
                    : string.Empty)
                .Replace("#SetSeed#", needsAutoIncrement
                    ? $"\r\nalter table \"#TableName#\" auto_increment = {seed};"
                    : string.Empty);
        }

        internal static string DropConstraintCommand(
            ISqlObjectFactory factory,
            string sourceTableName,
            IEnumerable<IndexInfo> tableIndexCollection,
            string command)
        {
            //MySQLの以下の制約下で、古い定義のテーブルから主キー制約とインデックスを削除するコマンドを生成する。
            //・主キー制約の名称→'PRIMARY'という固定名称で命名される。
            //・主キー制約削除→auto_incrementがついているカラムは、主キー制約を削除する前にauto_incrementの削除が必要。
            return command
                .Replace("#DropConstraint#", tableIndexCollection
                    .Where(o => Indexes.Get(
                        factory: factory,
                        sourceTableName: sourceTableName)
                        .Contains(o.IndexName()))
                    .Select(o => Constraints.Sql_Drop(o)
                        .Replace("#SourceTableName#", sourceTableName)
                        .Replace("#IndexName#", o.IndexName()))
                    .Join("\r\n"))
                .Replace("#DropPRIMARY#",
                    Indexes.Get(factory: factory, sourceTableName: sourceTableName).Contains("PRIMARY")
                        ? Def.Sql.DropConstraint.Replace("#SourceTableName#", sourceTableName)
                        : string.Empty)
                .Replace("#DropAutoIncrement#",""); //処理の見直し。
        }
    }
}
