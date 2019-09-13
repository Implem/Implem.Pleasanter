namespace Implem.IRds
{
    public interface ISqlDataTypes
    {
        int MaxIdentifierLength { get; }
        int NationalCharacterSizeCoefficient { get; }
        string Convert(string name);
        string ConvertBack(string name);
    }
}
