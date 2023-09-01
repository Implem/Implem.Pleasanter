﻿using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.ViewModes
{
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