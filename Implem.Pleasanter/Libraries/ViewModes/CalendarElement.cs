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
        public string StatusHtml;
        [NonSerialized]
        public DateTime UpdatedTime;

        public CalendarElement(
            long id,
            string title,
            string time,
            DateTime from,
            DateTime to,
            long changedItemId,
            DateTime updatedTime,
            string statusHtml)
        {
            Id = id;
            Title = title;
            Time = time;
            From = from;
            if (to.InRange()) To = to;
            if (id == changedItemId) Changed = true;
            UpdatedTime = updatedTime;
            StatusHtml = statusHtml;
        }
    }

    [Serializable]
    public class FullCalendarElement
    {
        public long id;
        public string title;
        public string time;
        public DateTime start;
        public DateTime? end;
        public bool? Changed;
        public string StatusHtml;
        [NonSerialized]
        public DateTime UpdatedTime;

        public FullCalendarElement(
            long Id,
            string Title,
            string Time,
            DateTime from,
            DateTime to,
            long changedItemId,
            DateTime updatedTime,
            string statusHtml)
        {
            id = Id;
            title = Title;
            time = Time;
            start = from;
            if (to.InRange()) end = to;
            if (id == changedItemId) Changed = true;
            UpdatedTime = updatedTime;
            StatusHtml = statusHtml;
        }
    }
}