namespace Implem.IRds
{
    public interface ISqlDataTypes
    {
        string Convert(string name);
        string ConvertBack(string name);
    }
}
