namespace Implem.IRds
{
    internal class PostgreSqlDefinitionSetting : ISqlDefinitionSetting
    {
        private string schemaName = null;
        private bool isCreatingDb = false;
        public int IdentifierPostfixLength { get; } = 32;
        public int NationalCharacterStoredSizeCoefficient { get; } = 4;
        public int ReducedVarcharLength { get; }
        public string SchemaName { get { return schemaName; } set { schemaName = value; } }
        public bool IsCreatingDb { get { return isCreatingDb; } set { isCreatingDb = value; } }
    }
}
