using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds.Parts.PostgreSql
{
    //PostgreSQL用の処理のみを集めた部品クラス。
    internal static class PostgreSqlColumns
    {
        internal static string Sql_Create(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition,
            bool noIdentity)
        {
            var commandText = "\"{0}\" {1}".Params(columnDefinition.ColumnName, columnDefinition.TypeName);
            if (columnDefinition.TypeName == "varbinary" && columnDefinition.MaxLength == -1)
            {
                // varbinary(max) は PostgreSqlDataTypes.Convert で bytea に変換される
                // bytea型には型修正子を付けない
            }
            else if (columnDefinition.MaxLength == -1)
            {
                commandText += "(max)";
            }
            else if (columnDefinition.TypeName == "decimal")
            {
                commandText += "(" + columnDefinition.Size + ")";
            }
            else
            {
                if (columnDefinition.MaxLength != 0)
                {
                    commandText += "({0})".Params(columnDefinition.MaxLength);
                }
            }
            if (!noIdentity && columnDefinition.Identity)
            {
                commandText += factory.Sqls.GenerateIdentity.Params(columnDefinition.Seed == 0 ? 1 : columnDefinition.Seed);
            }
            if (columnDefinition.Nullable)
            {
                commandText += " null";
            }
            else
            {
                commandText += " not null";
            }
            return factory.SqlDataType.Convert(commandText);
        }
    }
}