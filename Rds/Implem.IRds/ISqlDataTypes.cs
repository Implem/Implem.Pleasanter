using System.Data;

namespace Implem.IRds
{
    public interface ISqlDataType
    {
        string Convert(string name);
        string ConvertBack(string name, string definitionTypeName, bool isQrtzTable);

        string DefaultValue(DataRow dataRow);

        string StringLiteral(string value);
    }
}
