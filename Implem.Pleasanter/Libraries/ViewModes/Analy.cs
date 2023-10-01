using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class Analy : List<AnalyPart>
    {
        public Analy(
            Context context,
            SiteSettings ss,
            List<AnalyData> analyDataList)
        {
            analyDataList.ForEach(analyData =>
            {
                var analyPart = new AnalyPart(setting: analyData.AnalyPartSetting);
                var groupBy = ss.GetColumn(
                    context: context,
                    columnName: analyData.AnalyPartSetting.GroupBy);
                analyData.Data.Values
                    .GroupBy(o => o.GroupBy)
                    .Select(o => new
                    {
                        GroupValue = o.First().GroupBy,
                        GroupTitle = groupBy.ChoiceHash.Get(o.First().GroupBy)?.Text
                            ?? "? " + o.First().GroupBy,
                        Value = aggregate(
                            aggregationType: analyData.AnalyPartSetting.AggregationType,
                            values: o)
                    })
                    .OrderByDescending(o => o.Value)
                    .ForEach(data =>
                    {
                        var analyPartElement = new AnalyPartElement(
                            groupValue: data.GroupValue,
                            groupTitle: data.GroupTitle,
                            value: data.Value);
                        analyPart.Elements.Add(analyPartElement);
                    });
                Add(analyPart);
            });
        }

        private decimal aggregate(string aggregationType, IGrouping<string, AnalyDataRow> values)
        {
            switch (aggregationType)
            {
                case "Count":
                    return values.Count();
                case "Total":
                    return values.Sum(value => value.Value);
                case "Average":
                    return values.Average(value => value.Value);
                case "Min":
                    return values.Min(value => value.Value);
                case "Max":
                    return values.Max(value => value.Value);
                default:
                    return default;
            }
        }
    }
}