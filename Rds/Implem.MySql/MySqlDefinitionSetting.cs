namespace Implem.IRds
{
    internal class MySqlDefinitionSetting : ISqlDefinitionSetting
    {
        public int IdentifierPostfixLength { get; } = 32;
        public int NationalCharacterStoredSizeCoefficient { get; } = 4;
        public int ReducedVarcharLength { get; } = 760;
        public string SchemaName { get => ""; set => _ = value; }
        public bool IsCreatingDb { get => false; set => _ = value; }
    }
}
