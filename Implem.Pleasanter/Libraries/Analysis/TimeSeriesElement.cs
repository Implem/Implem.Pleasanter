using System;
namespace Implem.Pleasanter.Libraries.Analysis
{
    public class TimeSeriesElement
    {
        public long Id;
        public int Ver;
        public DateTime UpdatedTime;
        public string Index;
        public decimal Value;

        public TimeSeriesElement(
            long id, int ver, DateTime updatedTime, string index, decimal value)
        {
            Id = id;
            Ver = ver;
            UpdatedTime = updatedTime;
            Index = index;
            Value = value;
        }
    }
}