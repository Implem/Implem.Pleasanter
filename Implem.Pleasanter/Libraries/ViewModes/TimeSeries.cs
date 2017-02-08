using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class TimeSeries : List<TimeSeriesElement>
    {
        public SiteSettings SiteSettings;
        public string GroupByColumn;
        public string AggregationType;
        public string ValueColumn;
        public DateTime MinTime;
        public DateTime MaxTime;
        public double Days;

        private struct Data
        {
            public List<Index> Indexes;
            public IEnumerable<Element> Elements;
            public string Unit;
        }

        private struct Index
        {
            public int Id;
            public string Key;
            public string Text;
            public string Style;
        }

        private struct Element
        {
            public int Index;
            public string Day;
            public decimal Value;
            public decimal Y;
        }

        public TimeSeries(
            SiteSettings ss,
            string groupBy,
            string aggregationType,
            string value,
            IEnumerable<DataRow> dataRows)
        {
            SiteSettings = ss;
            GroupByColumn = groupBy;
            AggregationType = aggregationType;
            ValueColumn = value;
            dataRows.ForEach(dataRow =>
                Add(new TimeSeriesElement(
                    SiteSettings.GetColumn(GroupByColumn)?.UserColumn == true,
                    dataRow["Id"].ToLong(),
                    dataRow["Ver"].ToInt(),
                    dataRow["UpdatedTime"].ToDateTime().ToLocal().Date,
                    dataRow["Index"].ToString(),
                    dataRow["Value"].ToDecimal(),
                    dataRow["IsHistory"].ToBool())));
            if (this.Any())
            {
                MinTime = this.Select(o => o.UpdatedTime).Min().AddDays(-1);
                MaxTime = DateTime.Today;
                Days = Times.DateDiff(Times.Types.Days, MinTime, MaxTime);
            }
            this.Select(o => o.Id).Distinct().ForEach(id =>
            {
                var element = this
                    .Where(o => o.Id == id)
                    .OrderByDescending(o => o.Ver)
                    .First();
                element.Latest = true;
                if (element.IsHistory)
                {
                    element.UpdatedTime = element.UpdatedTime.AddDays(-1);
                }
            });
        }

        public string Json()
        {
            var elements = new List<Element>();
            var groupByColumn = SiteSettings.GetColumn(GroupByColumn);
            var choices = groupByColumn
                .EditChoices(addNotSet: true)
                .Reverse()
                .Where(o => this.Select(p => p.Index).Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value);
            var valueColumn = SiteSettings.GetColumn(ValueColumn);
            var choiceKeys = choices.Keys.ToList();
            var indexes = choices.Select((o, i) => new Index
            {
                Id = i,
                Key = o.Key,
                Text = IndexText(o, valueColumn),
                Style = o.Value.Style
            }).ToList();
            if (this.Any())
            {
                for (var d = 0; d <= Days; d++)
                {
                    decimal y = 0;
                    var currentTime = MinTime.AddDays(d);
                    var targets = Targets(currentTime);
                    indexes.Select(o => o.Key).ForEach(index =>
                    {
                        var data = GetData(targets.Where(o => o.Index == index));
                        if (!choices.ContainsKey(index))
                        {
                            choices.Add(index, new ControlData("? " + index));
                        }
                        y += data;
                        elements.Add(new Element()
                        {
                            Index = choiceKeys.IndexOf(index),
                            Day = currentTime.ToLocal(Displays.YmdFormat()),
                            Value = data,
                            Y = y
                        });
                    });
                }
            }
            return new Data()
            {
                Indexes = indexes.OrderByDescending(o => o.Id).ToList(),
                Elements = elements,
                Unit = AggregationType != "Count"
                    ? valueColumn.Unit
                    : string.Empty
            }.ToJson();
        }

        private string IndexText(KeyValuePair<string, ControlData> index, Column valueColumn)
        {
            var data = GetData(Targets(MaxTime).Where(p => p.Index == index.Key));
            return "{0}({1})".Params(
                index.Value.Text,
                AggregationType != "Count"
                    ? valueColumn.Display(data)
                    : data.ToString());
        }

        private IEnumerable<TimeSeriesElement> Targets(DateTime currentTime)
        {
            var processed = new HashSet<long>();
            var ret = new List<TimeSeriesElement>();
            this.Where(o => o.UpdatedTime <= currentTime)
                .OrderByDescending(o => o.UpdatedTime)
                .ThenByDescending(o => o.Ver)
                .ForEach(data =>
                {
                    if (!processed.Contains(data.Id))
                    {
                        if (!(data.IsHistory && data.Latest && data.UpdatedTime != currentTime))
                        {
                            ret.Add(data);
                        }
                        processed.Add(data.Id);
                    }
                });
            return ret;
        }

        private decimal GetData(IEnumerable<TimeSeriesElement> targets)
        {
            if (targets.Any())
            {
                switch (AggregationType)
                {
                    case "Count": return targets.Count();
                    case "Total": return targets.Select(o => o.Value).Sum();
                    case "Average": return targets.Select(o => o.Value).Average();
                    case "Max": return targets.Select(o => o.Value).Max();
                    case "Min": return targets.Select(o => o.Value).Min();
                    default: return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}