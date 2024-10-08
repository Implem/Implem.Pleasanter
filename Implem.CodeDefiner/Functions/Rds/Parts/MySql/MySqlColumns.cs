using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using static Implem.Libraries.DataSources.SqlServer.Sqls;
namespace Implem.CodeDefiner.Functions.Rds.Parts.MySql
{
    internal static class MySqlColumns
    {
        internal static string Sql_Create(
            ISqlObjectFactory factory,
            ColumnDefinition columnDefinition)
        {
            string dataTypeSize;
            if (columnDefinition.TypeName == "nvarchar" &&
                columnDefinition.MaxLength >= 1024)
            {
                dataTypeSize = NeedReduceByDefault(columnDefinition: columnDefinition) ||
                    NeedReduceByIndex(factory: factory, columnDefinition: columnDefinition)
                    ? "varchar(" + factory.SqlDefinitionSetting.ReducedVarcharLength.ToString() + ")"
                    : "text";
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
