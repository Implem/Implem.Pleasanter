using System;
namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class CalendarElement
    {
        public long Id;
        public string Title;
        public DateTime Date;
        public DateTime Time;

        public CalendarElement(long id, string title, DateTime time)
        {
            Id = id;
            Title = title;
            Date = time.Date;
            Time = time;
        }
    }
}