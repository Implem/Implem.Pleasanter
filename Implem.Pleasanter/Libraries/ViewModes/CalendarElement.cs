using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.ViewModes
{
    [Serializable]
    public class CalendarElement
    {
        public long Id;
        public long SiteId;
        public string Title;
        public string Time;
        public string DateFormat;
        public DateTime From;
        public DateTime? To;
        public bool? Changed;
        public string StatusHtml;
        [NonSerialized]
        public DateTime UpdatedTime;

        public CalendarElement(
            long id,
            long siteId,
            string title,
            string time,
            string dateFormat,
            DateTime from,
            DateTime to,
            long changedItemId,
            DateTime updatedTime,
            string statusHtml)
        {
            Id = id;
            SiteId = siteId;
            Title = title;
            Time = time;
            DateFormat = dateFormat;
            From = from;
            if (to.InRange()) To = to;
            if (id == changedItemId) Changed = true;
            UpdatedTime = updatedTime;
            StatusHtml = statusHtml;
        }
    }    
}