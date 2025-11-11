using System.Data;

namespace Implem.IRds
{
    public interface ISqlDataType
    {
        string Convert(string name);
        string ConvertBack(string name, bool isQrtzTable);
        string DefaultDefinition(DataRow dataRow);
    }
}
