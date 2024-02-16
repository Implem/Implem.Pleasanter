using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;

namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class AnalyDataRow
    {
        public long Id { get; set; }
        public int Ver { get; set; }
        public string GroupBy { get; set; }
        public decimal Value { get; set; }

        public AnalyDataRow(
            AnalyPartSetting analyPartSetting,
            DataRow dataRow)
        {
            Id = dataRow.Long("Id");
            Ver = dataRow.Int("Ver");
            GroupBy = dataRow.String("GroupBy");
            Value = analyPartSetting.AggregationType == "Count"
                ? 1
                : dataRow.Decimal("Value");
        }
    }
}
