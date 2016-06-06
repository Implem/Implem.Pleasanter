using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Analysis
{
    public class TimeSeries : List<TimeSeriesElement>
    {
        public SiteSettings SiteSettings;
        public string GroupByColumn;
        public string AggregationType;
        public DateTime MinTime;
        public DateTime MaxTime;
        public int Days;

        private struct Data
        {
            public List<Index> Indexes;
            public IEnumerable<Element> Elements;
        }

        private struct Index
        {
            public int Id;
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
            SiteSettings siteSettings,
            string groupByColumn,
            string aggregationType,
            IEnumerable<DataRow> dataRows)
        {
            SiteSettings = siteSettings;
            GroupByColumn = groupByColumn;
            AggregationType = aggregationType;
            dataRows.ForEach(dataRow =>
            {
                Add(new TimeSeriesElement(
                    dataRow["Id"].ToLong(),
                    dataRow["Ver"].ToInt(),
                    dataRow["UpdatedTime"].ToDateTime(),
                    dataRow["Index"].ToString(),
                    dataRow["Value"].ToDecimal()));
            });
            if (this.Count > 0)
            {
                MinTime = this.Select(o => o.UpdatedTime).Min();
                MaxTime = this.Select(o => o.UpdatedTime).Max();
                Days = Times.DateDiff(Times.Types.Days, MinTime, MaxTime);
            }
        }

        public string ChartJson()
        {
            var elements = new List<Element>();
            var choices = SiteSettings.AllColumn(GroupByColumn).EditChoices(SiteSettings.SiteId);
            var choiceKeys = choices.Keys.ToList();
            if (this.Count > 0)
            {
                var now = DateTime.Now.ToLocal().Date;
                for (var d = 0; d <= Days; d++)
                {
                    decimal y = 0;
                    this.Select(o => o.Index).Distinct().ForEach(index =>
                    {
                        var currentTime = MinTime.AddDays(d);
                        var targets = Targets(currentTime, index);
                        var value = GetValue(targets);
                        if (!choices.ContainsKey(index))
                        {
                            choices.Add(index, new ControlData("? " + index));
                        }
                        y += value;
                        elements.Add(new Element()
                        {
                            Index = choiceKeys.IndexOf(index),
                            Day = currentTime.ToLocal(Displays.YmdFormat()),
                            Value = value,
                            Y = y
                        });
                    });
                }
            }
            return new Data()
            {
                Indexes = choices.Select((o, i) => new Index
                {
                    Id = i,
                    Text = o.Value.Text,
                    Style = o.Value.Style
                }).OrderByDescending(o => o.Id).ToList(),
                Elements = elements
            }.ToJson();
        }

        private IEnumerable<TimeSeriesElement> Targets(DateTime currentTime, string index)
        {
            var processed = new HashSet<long>();
            var ret = new List<TimeSeriesElement>();
            this.Where(o => o.Index == index)
                .Where(o => o.UpdatedTime <= currentTime)
                .OrderByDescending(o => o.UpdatedTime)
                .ThenByDescending(o => o.Ver)
                .ForEach(data =>
                {
                    if (!processed.Contains(data.Id))
                    {
                        ret.Add(data);
                        processed.Add(data.Id);
                    }
                });
            return ret;
        }

        private decimal GetValue(IEnumerable<TimeSeriesElement> targets)
        {
            if (targets.Count() > 0)
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