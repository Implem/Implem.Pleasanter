using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Data;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class ProgressRate : IConvertable
    {
        public DateTime CreatedTime;
        public DateTime StartTime;
        public DateTime CompletionTime;
        public decimal Value;
        public DateTime UpdatedTime;
        public Versions.VerTypes VerType = Versions.VerTypes.Latest;

        public ProgressRate()
        {
        }

        public ProgressRate(
            DataRow dataRow,
            string createdTimeColumnName = "CreatedTime",
            string startTimeColumnName = "StartTime",
            string completionTimeColumnName = "CompletionTime",
            string progressRateColumnName = "ProgressRate")
        {
            CreatedTime = dataRow.DateTime(createdTimeColumnName);
            StartTime = dataRow.DateTime(startTimeColumnName);
            CompletionTime = dataRow.DateTime(completionTimeColumnName);
            Value = dataRow.Decimal(progressRateColumnName);
            UpdatedTime = dataRow["UpdatedTime"].ToDateTime();
            if (dataRow.Table.Columns.Contains("IsHistory"))
            {
                VerType = dataRow["IsHistory"].ToBool()
                    ? Versions.VerTypes.History
                    : Versions.VerTypes.Latest;
            }
        }

        public ProgressRate(
            Time createdTime,
            DateTime startTime,
            CompletionTime completionTime,
            decimal value)
        {
            CreatedTime = createdTime?.Value ?? 0.ToDateTime();
            StartTime = startTime;
            CompletionTime = completionTime?.Value ?? 0.ToDateTime();
            Value = value;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return column.Display(ss, Value);
        }

        public string ToResponse()
        {
            return Value.ToString();
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => Svg(hb, column));
        }

        public bool Delay(Status status)
        {
            if (!status.Incomplete()) return false;
            var now = VerType == Versions.VerTypes.Latest
                ? DateTime.Now.ToLocal()
                : UpdatedTime.ToLocal();
            var start = Start().ToLocal();
            var end = CompletionTime.ToLocal();
            var range = Times.DateDiff(Times.Types.Seconds, start, end);
            var plannedValue = PlannedValue(now, start, range);
            var earnedValue = EarnedValue();
            return plannedValue > earnedValue && Value < 100;
        }

        private HtmlBuilder Svg(HtmlBuilder hb, Column column)
        {
            var now = VerType == Versions.VerTypes.Latest
                ? DateTime.Now.ToLocal()
                : UpdatedTime.ToLocal();
            var start = Start().ToLocal();
            var end = CompletionTime.ToLocal();
            var range = Times.DateDiff(Times.Types.Seconds, start, end);
            var plannedValue = PlannedValue(now, start, range);
            var earnedValue = EarnedValue();
            var css = "svg-progress-rate" +
                (plannedValue > earnedValue && Value < 100
                    ? " warning"
                    : string.Empty);
            return hb.Svg(css: css, action: () => hb
                .SvgText(
                    text: column.Display(Value, unit: true),
                    x: 0,
                    y: Parameters.General.ProgressRateTextTop)
                .Rect(
                    x: 0,
                    y: Parameters.General.ProgressRateItemHeight * 2,
                    width: Parameters.General.ProgressRateWidth,
                    height: Parameters.General.ProgressRateItemHeight)
                .Rect(
                    x: 0,
                    y: Parameters.General.ProgressRateItemHeight * 2,
                    width: Convert.ToInt32(plannedValue * Parameters.General.ProgressRateWidth),
                    height: Parameters.General.ProgressRateItemHeight)
                .Rect(
                    x: 0,
                    y: Parameters.General.ProgressRateItemHeight * 3,
                    width: Convert.ToInt32(earnedValue * Parameters.General.ProgressRateWidth),
                    height: Parameters.General.ProgressRateItemHeight));
        }

        private DateTime Start()
        {
            return StartTime.InRange()
                ? StartTime
                : CreatedTime;
        }

        private double PlannedValue(DateTime now, DateTime start, double range)
        {
            return start < now && range > 0
                ? (Elapsed(now, start)) / range
                : 0;
        }

        private static double Elapsed(DateTime now, DateTime start)
        {
            return Times.DateDiff(Times.Types.Seconds, start, now);
        }

        private float EarnedValue()
        {
            return Value != 0
                ? (float)(Value / 100)
                : 0;
        }

        public string GridText(Column column)
        {
            return column.Display(Value, unit: true);
        }

        public string ToExport(Column column)
        {
            return Value.ToString();
        }

        public string ToNotice(
            decimal saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.Display(Value, unit: true).ToNoticeLine(
                column.Display(saved, unit: true),
                column,
                updated,
                update);
        }
    }
}