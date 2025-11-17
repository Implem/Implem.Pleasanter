using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using static Implem.Libraries.DataSources.SqlServer.Sqls;
namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    //MySQL用の処理のみを集めた部品クラス。
    //MySQLはSQLServer・PostgreSQLとは異なる制約を多く有しているため、専用のクラスに独自の処理をまとめた。
    internal static class MySqlColumns
    {
        internal static string Sql_Create(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition)
        {
            //MySQLの以下の制約下で、データ型・最大サイズ・Null制約指定のコマンドを生成する。
            //・varchar(1024)→該当のデータ型を多用しようとすると、テーブルの生成処理がエラー終了する。このエラーはvarchar(1024)ではなくtext型にすることで回避可能。
            //・text型→デフォルト値の指定は不可。また、インデックス指定時にサイズ制限が発生する。
            //2番目の制限に該当するカラムの場合はtext型を指定できないため、varchar型かつ最大サイズを元の定義よりも縮小することで問題を回避する。
            string dataTypeSize;
            if (columnDefinition.TypeName == "nvarchar" &&
                columnDefinition.MaxLength >= 1024)
            {
                dataTypeSize = NeedReduceByDefault(columnDefinition: columnDefinition) ||
                    NeedReduceByIndex(factory: factory, columnDefinition: columnDefinition)
                    ? "varchar(" + factory.SqlDefinitionSetting.ReducedVarcharLength.ToString() + ")"
                    : "text";
            }
            else if (columnDefinition.TypeName == "varbinary" && columnDefinition.MaxLength == -1)
            {
                dataTypeSize = "blob";
            }
            else
            {
                dataTypeSize = columnDefinition.TypeName;
                if (columnDefinition.MaxLength == -1)
                {
                    dataTypeSize += "(max)";
                }
                else if (columnDefinition.TypeName == "decimal")
                {
                    dataTypeSize += "(" + columnDefinition.Size + ")";
                }
                else
                {
                    if (columnDefinition.MaxLength != 0)
                    {
                        dataTypeSize += "({0})".Params(columnDefinition.MaxLength);
                    }
                }
            }
            var commandText = "\"{0}\" {1}".Params(columnDefinition.ColumnName, dataTypeSize);
            if (columnDefinition.Nullable)
            {
                commandText += " null";
            }
            else
            {
                commandText += " not null";
            }
            //MySQLの場合はここではなく、主キー制約追加後にデフォルト値指定と同じタイミングで、
            //identityに代えてauto_incrementを設定する。
            return factory.SqlDataType.Convert(commandText);
        }

        internal static bool NeedReduceByDefault(
            ColumnDefinition columnDefinition)
        {
            return !columnDefinition.Default.IsNullOrEmpty();
        }

        internal static bool NeedReduceByIndex(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition)
        {
            //_deleteおよび_historyでデータ型の差異が生じないよう通常テーブルのIndex情報を参照する。
            foreach (IndexInfo i in Indexes.IndexInfoCollection(
                factory: factory,
                generalTableName: columnDefinition.TableName,
                sourceTableName: columnDefinition.TableName,
                tableType: TableTypes.Normal))
            {
                foreach (IndexInfo.Column c in i.ColumnCollection)
                {
                    if (c.ColumnName == columnDefinition.ColumnName) return true;
                }
            }
            return false;
        }
    }
}
