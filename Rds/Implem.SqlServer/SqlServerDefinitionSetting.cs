namespace Implem.IRds
{
    internal class SqlServerDefinitionSetting : ISqlDefinitionSetting
    {
        public int IdentifierPostfixLength { get; } = 64;
        public int NationalCharacterStoredSizeCoefficient { get; } = 2;
    }
}
