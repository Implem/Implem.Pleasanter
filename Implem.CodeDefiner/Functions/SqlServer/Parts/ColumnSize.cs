using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Data;
namespace Implem.CodeDefiner.Functions.SqlServer.Parts
{
    internal static class ColumnSize
    {
        internal static bool HasChanges(
            DataRow dbColumn, ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TypeName)
            {
                case "char":
                case "varchar":
                    return Char(
                        columnDefinition, dbColumn, coefficient: 1);
                case "nchar":
                case "nvarchar":
                    return Char(
                        columnDefinition, dbColumn, coefficient: 2);
                case "decimal":
                    return Decimal(
                        columnDefinition, dbColumn);
                default:
                    return false;
            }
        }

        private static bool Char(
            ColumnDefinition columnDefinition, DataRow dbColumn, int coefficient)
        {
            return dbColumn["max_length"].ToInt() == -1 && columnDefinition.MaxLength == -1
                ? false
                : dbColumn["max_length"].ToInt() != columnDefinition.MaxLength * coefficient
                    ? true
                    : false;
        }

        private static bool Decimal(
            ColumnDefinition columnDefinition, DataRow dbColumn)
        {
            return dbColumn["Size"].ToString() != columnDefinition.Size;
        }
    }
}