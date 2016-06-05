using System;
namespace Implem.Pleasanter.Libraries.Analysis
{
    public class TimeSeriesElement
    {
        public DateTime Time;
        public int Number;
        public string Name;
        public decimal Value;

        public TimeSeriesElement(DateTime time, int number, string name, decimal value)
        {
            Time = time;
            Number = number;
            Name = name;
            Value = value;
        }
    }
}