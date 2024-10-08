using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Data;

namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
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
            switch (columnDefinition.TypeName)
            {
                case "nchar":
                    return ColumnSize.Char(
                        columnDefinition: columnDefinition,
                        rdsColumn: rdsColumn,
                        coefficient: factory.SqlDefinitionSetting.NationalCharacterStoredSizeCoefficient);
                case "nvarchar":
                    return VarcharMySql();
                case "decimal":
                    return ColumnSize.Decimal(
                        columnDefinition: columnDefinition,
                        rdsColumn: rdsColumn);
                default:
                    return false;
            }
        }
    }
}
