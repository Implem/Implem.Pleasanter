using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class AnalyPartSetting : ISettingListItem
    {
        public int Id { get; set; }
        public string GroupBy { get; set; }
        public decimal TimePeriodValue { get; set; }
        public string TimePeriod { get; set; }
        public string AggregationType { get; set; }
        public string AggregationTarget { get; set; }
    }
}
