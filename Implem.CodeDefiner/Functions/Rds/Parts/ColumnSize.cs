﻿using Implem.DefinitionAccessor;
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

        internal static bool Char(
            ColumnDefinition columnDefinition, DataRow rdsColumn, int coefficient)
        {
            return rdsColumn["max_length"].ToInt() == -1 && columnDefinition.MaxLength == -1
                ? false
                : rdsColumn["max_length"].ToInt() != columnDefinition.MaxLength * coefficient
                    ? true
                    : false;
        }

        internal static bool Decimal(
            ColumnDefinition columnDefinition, DataRow rdsColumn)
        {
            return rdsColumn["Size"].ToString() != columnDefinition.Size;
        }
    }
}