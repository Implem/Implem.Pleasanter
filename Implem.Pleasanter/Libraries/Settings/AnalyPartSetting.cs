using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class AnalyPartSetting : ISettingListItem
    {
        public int Id { get; set; }
        public string GroupBy { get; set; }
        public decimal Value { get; set; }
        public string Period { get; set; }
        public int PastOrFuture { get; set; }
        public string AggregationTarget { get; set; }
    }
}
