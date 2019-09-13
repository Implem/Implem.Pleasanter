using Implem.IRds;
namespace Implem.SqlServer
{
    internal class SqlServerDataTypes : ISqlDataTypes
    {
        public int MaxIdentifierLength { get; } = 64;

        public int NationalCharacterSizeCoefficient { get; } = 2;

        public string Convert(string name)
        {
            return name;
        }

        public string ConvertBack(string name)
        {
            return name;
        }
    }
}
