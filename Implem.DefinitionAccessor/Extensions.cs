namespace Implem.DefinitionAccessor
{
    public static class Extensions
    {
        public const int SysLogsTenantIdSchemaVersion = 3;

        public const int SysLogsLegacyUserHostAddressMaxLength = 16;

        public static bool LowSchemaVersion(this ColumnDefinition columnDefinition)
        {
            if (columnDefinition == null)
            {
                return false;
            }
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
