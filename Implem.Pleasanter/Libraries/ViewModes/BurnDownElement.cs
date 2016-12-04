using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class BurnDownElement
    {
        public long Id;
        public int Ver;
        public string Title;
        public decimal WorkValue;
        public DateTime StartTime;
        public DateTime CompletionTime;
        public decimal ProgressRate;
        public decimal ProgressRateAdditions;
        public int Status;
        public int Updator;
        public DateTime CreatedTime;
        public DateTime UpdatedTime;
        public decimal EarnedValue;
        public decimal EarnedValueAdditions;
        public double DayCount;
        public decimal PlannedValueOfDaily;

        public BurnDownElement(
            long id,
            int ver,
            string title,
            decimal workValue,
            DateTime startTime,
            DateTime completionTime,
            decimal progressRate,
            decimal progressRateAdditions,
            int status,
            int updatorId,
            decimal earnedValueAddtions,
            DateTime createdTime,
            DateTime updatedTime)
        {
            Id = id;
            Ver = ver;
            Title = title;
            WorkValue = workValue;
            StartTime = startTime.InRange()
                ? startTime.ToLocal().Date
                : createdTime.ToLocal().Date;
            CompletionTime = completionTime.ToLocal().Date;
            ProgressRate = progressRate;
            ProgressRateAdditions = progressRateAdditions;
            Status = status;
            Updator = updatorId;
            CreatedTime = createdTime.ToLocal().Date;
            UpdatedTime = updatedTime.ToLocal().AddDays(1).Date;
            EarnedValue = GetEarnedValue();
            EarnedValueAdditions = earnedValueAddtions;
            DayCount = Times.DateDiff(Times.Types.Days, StartTime, CompletionTime);
            PlannedValueOfDaily = DayCount != 0
                ? WorkValue / DayCount.ToDecimal()
                : 0;
        }

        public decimal PlannedValue(DateTime currentTime)
        {
            if (StartTime < currentTime)
            {
                if (CompletionTime >= currentTime)
                {
                    return WorkValue -
                        PlannedValueOfDaily *
                        Times.DateDiff(Times.Types.Days, StartTime, currentTime).ToDecimal();
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return WorkValue;
            }
        }

        public decimal GetEarnedValue()
        {
            return WorkValue * ProgressRate / 100;
        }
    }
}