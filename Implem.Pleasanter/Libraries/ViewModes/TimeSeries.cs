using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Search;
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
        public string AggregationType;
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
            public string sumValue;//変更点 legend
            public string Style;
        }

        private struct Element
        {
            public int Index;
            public string Day;
            public decimal Value;
            public decimal Y;
        }


        /*
        横軸の範囲を項目にあわせて変化させる。
         */
        public TimeSeries(
            Context context,
            SiteSettings ss,
            Column groupBy,
            string aggregationType,
            Column value,
            string horizontalAxis,
            IEnumerable<DataRow> dataRows)
        {
            SiteSettings = ss;
            AggregationType = aggregationType;
            dataRows.ForEach(dataRow =>
                Add(new TimeSeriesElement(
                    context: context,
                    userColumn: groupBy?.Type == Column.Types.User,
                    id: dataRow["Id"].ToLong(),
                    ver: dataRow["Ver"].ToInt(),
                    updatedTime: dataRow["UpdatedTime"]
                        .ToDateTime()
                        .ToLocal(context: context)
                        .Date,
                    horizontalAxis: (horizontalAxis == "Histories"
                        ? DateTime.MinValue
                        : dataRow["HorizontalAxis"]
                            .ToDateTime()
                            .ToLocal(context: context)
                            .Date),
                    index: dataRow[groupBy.ColumnName].ToString(),
                    value: dataRow[value.ColumnName].ToDecimal(),
                    isHistory: (horizontalAxis == "Histories"
                        ? dataRow["IsHistory"].ToBool()
                        : false))));
            if (this.Any())
            {
                if (horizontalAxis == "Histories")
                {
                    MinTime = this.Select(o => o.UpdatedTime).Min().AddDays(-1);
                    MaxTime = DateTime.Today;
                }
                else
                {
                    var dateExists = this
                        .Where(o => o.HorizontalAxis != DateTime.Parse("1899/12/30 0:00:00"))
                        .Select(o => o.HorizontalAxis);
                    MinTime = (dateExists.Any())
                        ? dateExists.Min().AddDays(-1)//取り出したい値の一つ下の値
                        : DateTime.MinValue;
                    MaxTime = (dateExists.Any() && DateTime.Today <= dateExists.Max())
                        ? dateExists.Max()//取り出したい値の一つ下の値
                        : DateTime.Today;
                }
                //MaxTime = DateTime.Today;//ここを変える
                Days = Times.DateDiff(Times.Types.Days, MinTime, MaxTime);
                this
                    .OrderByDescending(o => o.Ver)
                    .GroupBy(o => o.Id)
                    .Select(o => o.First())
                    .ForEach(element =>
                    {
                        element.Latest = true;
                        if (element.IsHistory)
                        {
                            element.UpdatedTime = element.UpdatedTime.AddDays(-1);
                        }
                    });
            }
        }

        public string Json(
            Context context,
            Column groupBy,
            Column value,
            string horizontalAxis)
        {
            var elements = new List<Element>();
            var choices = groupBy
                ?.ChoiceHash
                ?.ToDictionary(o => o.Key, o => new ControlData(o.Value.Text))
                .Reverse()
                .Where(o => this.Select(p => p.Index).Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value)
                    ?? new Dictionary<string, ControlData>();
            var valueColumn = value;
            var choiceKeys = choices.Keys.ToList();
            //indexesの値
            var indexes = choices.Select((index, id) => new Index
            {
                Id = id,
                Key = index.Key,
                Text = IndexText(
                    context: context,
                    index: index,
                    valueColumn: valueColumn,
                    horizontalAxis: horizontalAxis),
                sumValue = IndexValue(//変更点
                    context: context,
                    index: index,
                    valueColumn: valueColumn,
                    horizontalAxis: horizontalAxis),
                Style = index.Value.Style
            }).ToList();
            if (this.Any())
            {
                for (var d = 0; d <= Days; d++)
                {
                    decimal y = 0;
                    var currentTime = MinTime.AddDays(d);
                    var targets = Targets(
                        currentTime,
                        horizontalAxis);
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
                            Day = currentTime.ToLocal(
                                context: context,
                                format: Displays.YmdFormat(context: context)),
                            Value = data,
                            Y = y
                        });
                    });
                }
            }
            return new Data()//TimeSeries.jsに送られるデータ
            {
                Indexes = indexes.OrderByDescending(o => o.Id).ToList(),//降順
                Elements = elements,
                Unit = AggregationType != "Count"
                    ? valueColumn.Unit
                    : string.Empty
            }.ToJson();
        }

        private string IndexText(
            Context context, KeyValuePair<string, ControlData> index, Column valueColumn, string horizontalAxis)
        {
            var data = GetData(Targets(MaxTime, horizontalAxis).Where(p => p.Index == index.Key));//同じ名前のもので取り出して件数などを返す
                return "{0}: {1}".Params(//Text:0hはここで設定　{0}を使って文字列に変数を埋め込む
                index.Value.Text,//名前・名称部分
                    AggregationType != "Count"
                        ? valueColumn.Display(
                            context: context,
                            value: data,//数値部分
                            unit: true)
                        : data.ToString());           
        }

        private string IndexValue(
            Context context, KeyValuePair<string, ControlData> index, Column valueColumn, string horizontalAxis)
        {
            var data = GetData(Targets(MaxTime, horizontalAxis).Where(p => p.Index == index.Key));
                return "{0}".Params(
                    AggregationType != "Count"
                        ? valueColumn.Display(
                            context: context,
                            value: data//数値部分
                            )
                        : data.ToString());
        }

        private IEnumerable<TimeSeriesElement> Targets(
            DateTime currentTime,
            string horizontalAxis = "")
        {
            var processed = new HashSet<long>();
            var ret = new List<TimeSeriesElement>();
            this.Where(o => (horizontalAxis == "Histories"
                    ? o.UpdatedTime <= currentTime
                    : o.HorizontalAxis == currentTime))
                .OrderByDescending(o => o.UpdatedTime)
                .ThenByDescending(o => o.Ver)
                .ForEach(data =>
                {
                    if (!processed.Contains(data.Id))
                    {
                        if (horizontalAxis == "Histories")
                        {
                            if (!(data.IsHistory && data.Latest && data.UpdatedTime != currentTime))
                            {
                                ret.Add(data);
                            }
                        }
                        else
                        {
                            if (!(data.IsHistory && data.Latest && data.HorizontalAxis != currentTime))
                            {
                                ret.Add(data);
                            }
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
                    default: return 0;//返されていると思われる箇所
                }
            }
            else
            {
                return 0;//返されていると思われる箇所
            }
        }
    }
}