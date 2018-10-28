using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class BurnDown : List<BurnDownElement>
    {
        public DateTime MinTime;
        public DateTime MaxTime;
        public DateTime LatestUpdatedTime;
        public double Days;

        private struct Element
        {
            public string Day;
            public decimal? Total;
            public decimal? Planned;
            public decimal? Earned;
        }

        public BurnDown(Context context, SiteSettings ss, IEnumerable<DataRow> dataRows)
        {
            dataRows.ForEach(dataRow =>
            {
                var id = dataRow.Long(Rds.IdColumn(ss.ReferenceType));
                var ver = dataRow.Int("Ver");
                var target = this.Where(o => o.Id == id && o.Ver < ver).LastOrDefault();
                var workValue = dataRow.Decimal("WorkValue");
                var progressRate = dataRow.Decimal("ProgressRate");
                var progressRateAdditions = ProgressRateAdditions(target, progressRate);
                Add(new BurnDownElement(
                    context: context,
                    id: id,
                    ver: ver,
                    title: new Title(
                        context: context,
                        ss: ss,
                        dataRow: dataRow).DisplayValue,
                    workValue: workValue,
                    startTime: dataRow.DateTime("StartTime"),
                    completionTime: dataRow.DateTime("CompletionTime"),
                    progressRate: progressRate,
                    progressRateAdditions: progressRateAdditions,
                    status: dataRow.Int("Status"),
                    updatorId: dataRow.Int("Updator"),
                    earnedValueAddtions: EarnedValueAddtions(workValue, progressRateAdditions),
                    createdTime: dataRow.DateTime("CreatedTime"),
                    updatedTime: dataRow.DateTime("UpdatedTime")));
            });
            if (this.Any())
            {
                var latest = Targets(DateTime.MaxValue);
                MinTime = latest.Select(o => o.StartTime).Min();
                MaxTime = latest.Select(o => o.CompletionTime).Max();
                LatestUpdatedTime = latest.Select(o => o.UpdatedTime).Max();
                Days = Times.DateDiff(Times.Types.Days, MinTime, MaxTime);
            }
        }

        private decimal ProgressRateAdditions(BurnDownElement target, decimal progressRate)
        {
            return target != null
                ? progressRate - target.ProgressRate
                : progressRate;
        }

        private decimal EarnedValueAddtions(decimal workValue, decimal progressRateAddtions)
        {
            return workValue * progressRateAddtions / 100;
        }

        public string Json(Context context)
        {
            var elements = new List<Element>();
            if (this.Any())
            {
                var now = DateTime.Now.ToLocal(context: context).Date;
                for (var d = 0; d <= Days; d++)
                {
                    var currentTime = MinTime.AddDays(d);
                    var targets = Targets(currentTime);
                    var totalValue = targets.Select(o => o.WorkValue).Sum();
                    elements.Add(new Element()
                    {
                        Day = currentTime.ToLocal(
                            context: context,
                            format: Displays.YmdFormat(context: context)),
                        Total = TotalValueSummary(currentTime, now, totalValue),
                        Planned = PlannedValueSummary(currentTime, targets, totalValue),
                        Earned = EarnedValueSummary(currentTime, now, targets, totalValue)
                    });
                }
            }
            return Jsons.ToJson(elements);
        }

        public IEnumerable<BurnDownElement> Targets(DateTime currentTime)
        {
            var processed = new HashSet<long>();
            var ret = new List<BurnDownElement>();
            this.Where(o => o.UpdatedTime <= currentTime)
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

        private decimal? TotalValueSummary(
            DateTime currentTime,
            DateTime now,
            decimal total)
        {
            if (currentTime <= LatestUpdatedTime)
            {
                return Math.Round(total, 1);
            }
            else
            {
                return null;
            }
        }

        private decimal? PlannedValueSummary(
            DateTime currentTime,
            IEnumerable<BurnDownElement> targets,
            decimal totalValue)
        {
            var ret = targets.Select(o => o.PlannedValue(currentTime)).Sum();
            if (ret >= 0)
            {
                return Math.Round(ret, 1);
            }
            else
            {
                if (currentTime == LatestUpdatedTime.AddDays(1))
                {
                    return null;
                }
                else
                {
                    return 0;
                }
            }
        }

        private decimal? EarnedValueSummary(
            DateTime currentTime,
            DateTime now,
            IEnumerable<BurnDownElement> targets,
            decimal totalValue)
        {
            if (currentTime <= LatestUpdatedTime)
            {
                return Math.Round(totalValue - targets.Select(o => o.EarnedValue).Sum(), 1);
            }
            else
            {
                return null;
            }
        }
    }
}