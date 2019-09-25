namespace Implem.IRds
{
    public interface ISqlDefinitionSetting
    {
        int IdentifierPostfixLength { get; }
        int NationalCharacterStoredSizeCoefficient { get; }
    }
}
