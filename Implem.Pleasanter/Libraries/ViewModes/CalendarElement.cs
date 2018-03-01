using Implem.Libraries.Utilities;
using System;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    [Serializable]
    public class CalendarElement
    {
        public long Id;
        public string Title;
        public string Time;
        public DateTime From;
        public DateTime? To;
        public bool? Changed;
        [NonSerialized]
        public DateTime UpdatedTime;

        public CalendarElement(
            long id,
            string title,
            string time,
            DateTime from,
            DateTime to,
            long changedItemId,
            DateTime updatedTime)
        {
            Id = id;
            Title = title;
            Time = time;
            From = from;
            if (to.InRange()) To = to;
            if (id == changedItemId) Changed = true;
            UpdatedTime = updatedTime;
        }
    }
}