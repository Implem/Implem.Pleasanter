using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Data;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class ColumnSize
    {
        internal static bool HasDifference(
            DataRow dbColumn, ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TypeName)
            {
                case "char":
                case "varchar":
                    return HasDifference(
                        columnDefinition, dbColumn, coefficient: 1);
                case "nchar":
                case "nvarchar":
                    return HasDifference(
                        columnDefinition, dbColumn, coefficient: 2);
                default:
                    return false;
            }
        }

        private static bool HasDifference(
            ColumnDefinition columnDefinition, DataRow dbColumn, int coefficient)
        {
            if (dbColumn["max_length"].ToInt() == -1 && columnDefinition.MaxLength == -1)
            {
                return false;
            }
            else
            {
                if (dbColumn["max_length"].ToInt() != columnDefinition.MaxLength * coefficient)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
