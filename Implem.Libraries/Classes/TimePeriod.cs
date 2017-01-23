using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
namespace Implem.Libraries.Classes
{
    public class TimePeriod
    {
        public Types Type;
        public DateTime ReferenceDate;
        public DateTime From;
        public DateTime To;

        public enum Types
        {
            Day,
            Week,
            Month,
            Quarter,
            Half,
            Fy
        }

        public TimePeriod(Types type, DateTime referenceTime, int diff)
        {
            Type = type;
            ReferenceDate = referenceTime.Date;
            switch (Type)
            {
                case Types.Day: Day(); break;
                case Types.Week: Week(); break;
                case Types.Month: Month(); break;
                case Types.Quarter: Quarter(); break;
                case Types.Half: Half(); break;
                case Types.Fy: Fy(); break;
                default: break;
            }
            From = From.AddDays(diff);
            To = To.AddDays(diff);
        }

        public bool InRange(DateTime time)
        {
            return From <= time && To >= time;
        }

        private void Day()
        {
            From = ReferenceDate.Date;
            To = From.AddDays(1).AddMilliseconds(Parameters.Rds.MinimumTime * -1);
        }

        private void Week()
        {
            From = ReferenceDate.WeekFrom();
            To = From.AddDays(7).AddMilliseconds(Parameters.Rds.MinimumTime * -1);
        }

        private void Month()
        {
            From = ReferenceDate.MonthFrom();
            To = From.AddMonths(1).AddMilliseconds(Parameters.Rds.MinimumTime * -1);
        }

        private void Quarter()
        {
            From = ReferenceDate.QuarterFrom();
            To = From.AddMonths(3).AddMilliseconds(Parameters.Rds.MinimumTime * -1);
        }

        private void Half()
        {
            From = ReferenceDate.HalfFrom();
            To = From.AddMonths(6).AddMilliseconds(Parameters.Rds.MinimumTime * -1);
        }

        private void Fy()
        {
            From = ReferenceDate.FyFrom();
            To = From.AddYears(1).AddMilliseconds(Parameters.Rds.MinimumTime * -1);
        }
    }
}