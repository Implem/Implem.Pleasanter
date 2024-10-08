namespace Implem.IRds
{
    internal class SqlServerDefinitionSetting : ISqlDefinitionSetting
    {
        public int IdentifierPostfixLength { get; } = 64;
        public int NationalCharacterStoredSizeCoefficient { get; } = 2;
        public int ReducedVarcharLength { get; }
        public string SchemaName { get => ""; set => _ = value; }
        public bool IsCreatingDb { get => false; set => _ = value; }
    }
}
