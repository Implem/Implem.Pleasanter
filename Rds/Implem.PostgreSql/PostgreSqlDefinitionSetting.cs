namespace Implem.IRds
{
    internal class PostgreSqlDefinitionSetting : ISqlDefinitionSetting
    {
        public int IdentifierPostfixLength { get; } = 32;
        public int NationalCharacterStoredSizeCoefficient { get; } = 4;
    }
}
