using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Data;
namespace Implem.CodeDefiner.Functions.Rds.Parts
{
    internal static class ColumnSize
    {
        internal static bool HasChanges(
            ISqlObjectFactory factory,
            DataRow rdsColumn,
            ColumnDefinition columnDefinition)
        {
            //PostgreSQLとSQLServerの場合のみ判定結果を戻す。
            //MySQLの場合ここではなく、HasChangesMySqlで判定する。
            if (Parameters.Rds.Dbms == "MySQL") return false;
            switch (columnDefinition.TypeName)
            {
                case "char":
                case "varchar":
                    return Char(
                        columnDefinition, rdsColumn, coefficient: 1);
                case "nchar":
                case "nvarchar":
                    return Char(
                        columnDefinition, rdsColumn,
                        coefficient: factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient);
                case "decimal":
                    return Decimal(
                        columnDefinition, rdsColumn);
                default:
                    return false;
            }
        }

        internal static bool HasChangesMySql(
            ISqlObjectFactory factory,
            DataRow rdsColumn,
            ColumnDefinition columnDefinition)
        {
            bool IsRdsReduced() {
                return rdsColumn["TypeName"].ToString() == "varchar" &&
                    rdsColumn["max_length"].ToInt() == 760 * factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient;
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
                        Char(
                            columnDefinition: columnDefinition,
                            rdsColumn: rdsColumn,
                            coefficient: factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient);
                }
                else if (Columns.NeedReduceByDefault(columnDefinition: columnDefinition))
                {
                    return !IsRdsReduced();
                }
                else if (Parameters.Rds.DisableIndexChangeDetection)
                {
                    //前提：Definition_ColumnのMaxlengthがnvarchar(1024)以上かつデフォルト値の指定がない場合、
                    //MySQLのcreate tableではvarchar(760)又はtextをカラムのデータ型に指定する。
                    //→その分岐条件は「Index指定の有無」であるため、Indexの差分でMigrateさせない環境下では、
                    //現在のDBの状態がどちらかのデータ型であればMigrateを実施しないと判定する。
                    return !IsRdsReduced() &&
                        rdsColumn["TypeName"].ToString() != "text";
                }
                else
                {
                    return !(Columns.NeedReduceByIndex(factory: factory, columnDefinition: columnDefinition) &&
                        IsRdsReduced() ||
                            !Columns.NeedReduceByIndex(factory: factory, columnDefinition: columnDefinition) &&
                                rdsColumn["TypeName"].ToString() == "text");
                }
            }
            //MySQL専用サイズ変更有無の判定をここで行う。
            if (Parameters.Rds.Dbms != "MySQL") return false;
            switch (columnDefinition.TypeName)
            {
                case "nchar":
                    return Char(
                        columnDefinition: columnDefinition,
                        rdsColumn: rdsColumn,
                        coefficient: factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient);
                case "decimal":
                    return Decimal(
                        columnDefinition, rdsColumn);
                case "nvarchar":
                    return VarcharMySql();
                default:
                    return false;
            }
        }

        private static bool Char(
            ColumnDefinition columnDefinition, DataRow rdsColumn, int coefficient)
        {
            return rdsColumn["max_length"].ToInt() == -1 && columnDefinition.MaxLength == -1
                ? false
                : rdsColumn["max_length"].ToInt() != columnDefinition.MaxLength * coefficient
                    ? true
                    : false;
        }

        private static bool Decimal(
            ColumnDefinition columnDefinition, DataRow rdsColumn)
        {
            return rdsColumn["Size"].ToString() != columnDefinition.Size;
        }
    }
}