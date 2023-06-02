namespace Implem.DefinitionAccessor
{
    public static class Extensions
    {
        public static bool CheckSchemaVersion(this ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TableName)
            {
                case "SysLogs":
                    if (columnDefinition.SchemaVersion > Parameters.Rds.SysLogsSchemaVersion)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
