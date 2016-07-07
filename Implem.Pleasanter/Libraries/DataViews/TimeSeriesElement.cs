using System;
namespace Implem.Pleasanter.Libraries.DataViews
{
    public class TimeSeriesElement
    {
        public long Id;
        public int Ver;
        public DateTime UpdatedTime;
        public string Index;
        public decimal Value;
        public bool IsHistory;
        public bool Latest;

        public TimeSeriesElement(
            long id, int ver, DateTime updatedTime, string index, decimal value, bool isHistory)
        {
            Id = id;
            Ver = ver;
            UpdatedTime = updatedTime;
            Index = index;
            Value = value;
            IsHistory = isHistory;
        }
    }
}