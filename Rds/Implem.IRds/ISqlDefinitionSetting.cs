namespace Implem.IRds
{
    public interface ISqlDefinitionSetting
    {
        int IdentifierPostfixLength { get; }
        int NationalCharacterStoredSizeCoefficient { get; }
        string SchemaName { get; set; }
        bool IsCreatingDb { get; set; }
    }
}
