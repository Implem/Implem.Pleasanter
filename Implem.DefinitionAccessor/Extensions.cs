namespace Implem.DefinitionAccessor
{
    public static class Extensions
    {
        public static bool LowSchemaVersion(this ColumnDefinition columnDefinition)
        {
            switch (columnDefinition.TableName)
            {
                case "SysLogs":
                    if (columnDefinition.SchemaVersion > Parameters.Rds.SysLogsSchemaVersion)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}
