using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.ViewModes
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
            bool userColumn,
            long id,
            int ver,
            DateTime updatedTime,
            string index,
            decimal value,
            bool isHistory)
        {
            Id = id;
            Ver = ver;
            UpdatedTime = updatedTime;
            Index = userColumn && SiteInfo.User(index.ToInt()).Anonymous()
                ? "\t"
                : index == string.Empty
                    ? "\t"
                    : index;
            Value = value;
            IsHistory = isHistory;
        }
    }
}