using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Data;

namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    //MySQL用の処理のみを集めた部品クラス。
    //MySQLはSQLServer・PostgreSQLとは異なる制約を多く有しているため、専用のクラスに独自の処理をまとめた。
    internal static class MySqlColumnSize
    {
        internal static bool HasChanges(
            ISqlObjectFactory factory,
            DataRow rdsColumn,
            ColumnDefinition columnDefinition)
        {
            bool IsRdsReduced()
            {
                return rdsColumn["TypeName"].ToString() == "varchar" &&
                    rdsColumn["max_length"].ToInt() ==
                        factory.SqlDefinitionSetting.ReducedVarcharLength *
                        factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient;
            }
            bool VarcharMySql()
            {
                if (columnDefinition.MaxLength == -1)
                {
                    return rdsColumn["TypeName"].ToString() != "longtext";
                }
                else if (columnDefinition.MaxLength < 1024)
                {
                    return rdsColumn["TypeName"].ToString() != "varchar" ||
                        ColumnSize.Char(
                            columnDefinition: columnDefinition,
                            rdsColumn: rdsColumn,
                            coefficient: factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient);
                }
                else if (MySqlColumns.NeedReduceByDefault(columnDefinition: columnDefinition))
                {
                    return !IsRdsReduced();
                }
                else if (Parameters.Rds.DisableIndexChangeDetection)
                {
                    //前提：Definition_ColumnのMaxlengthがnvarchar(1024)以上かつデフォルト値の指定がない場合、
                    //MySQLのcreate tableではvarchar(760)又はtextをカラムのデータ型に指定する。
                    //→その分岐条件に「Index指定の有無」があるため、Indexの差分でMigrateさせない環境下では、
                    //現在のDBの状態がどちらかのデータ型であればMigrateを実施しないと判定する。
                    return !IsRdsReduced() &&
                        rdsColumn["TypeName"].ToString() != "text";
                }
                else
                {
                    return !(MySqlColumns.NeedReduceByIndex(factory: factory, columnDefinition: columnDefinition) &&
                        IsRdsReduced() ||
                            !MySqlColumns.NeedReduceByIndex(factory: factory, columnDefinition: columnDefinition) &&
                                rdsColumn["TypeName"].ToString() == "text");
                }
            }
            //MySQLの以下の制約下で、カラムの最大サイズを突合する。
            //・MySqlColumnsでも対策した通り、SQLServerのnvarchar(1024)のカラムは、MySQLのtext型に統一ができない。
            //そのため一部テーブルのカラムではvarchar(760)が指定されている。
            switch (columnDefinition.TypeName)
            {
                case "nchar":
                    return ColumnSize.Char(
                        columnDefinition, rdsColumn, coefficient: 1);
                case "nvarchar":
                    return VarcharMySql();
                case "decimal":
                    return ColumnSize.Decimal(
                        columnDefinition, rdsColumn);
                default:
                    return false;
            }
        }
    }
}
