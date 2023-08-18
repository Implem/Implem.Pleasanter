﻿using Implem.Pleasanter.Interfaces;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class Html : ISettingListItem
    {
        public enum PositionTypes: int
        {
            HeadTop = 1000,
            HeadBottom = 1010,
            BodyScriptTop = 9000,
            BodyScriptBottom = 9010,
        }

        public int Id { get; set; }
        public string Title;
        public PositionTypes PositionType;
        public bool? All;
        public bool? New;
        public bool? Edit;
        public bool? Index;
        public bool? Calendar;
        public bool? Crosstab;
        public bool? Gantt;
        public bool? BurnDown;
        public bool? TimeSeries;
        public bool? Kamban;
        public bool? ImageLib;
        public bool? Disabled;
        public string Body;

        public Html()
        {
                
        }

        public Html(
            int id,
            string title,
            PositionTypes positionType,
            bool all,
            bool _new,
            bool edit,
            bool index,
            bool calendar,
            bool crosstab,
            bool gantt,
            bool burnDown,
            bool timeSeries,
            bool kamban,
            bool imageLib,
            bool disabled,
            string body)
        {
            Id = id;
            Title = title;
            PositionType = positionType;
            All = all;
            New = _new;
            Edit = edit;
            Index = index;
            Calendar = calendar;
            Crosstab = crosstab;
            Gantt = gantt;
            BurnDown = burnDown;
            TimeSeries = timeSeries;
            Kamban = kamban;
            ImageLib = imageLib;
            Disabled = disabled;
            Body = body;
        }

        public void Update(
           string title,
           PositionTypes positionType,
           bool all,
           bool _new,
           bool edit,
           bool index,
           bool calendar,
           bool crosstab,
           bool gantt,
           bool burnDown,
           bool timeSeries,
           bool kamban,
           bool imageLib,
           bool disabled,
           string body)
        {
            Title = title;
            PositionType = positionType;
            All = all;
            New = _new;
            Edit = edit;
            Index = index;
            Calendar = calendar;
            Crosstab = crosstab;
            Gantt = gantt;
            BurnDown = burnDown;
            TimeSeries = timeSeries;
            Kamban = kamban;
            ImageLib = imageLib;
            Disabled = disabled;
            Body = body;
        }

        public Html GetRecordingData()
        {
            var html = new Html();
            html.Id = Id;
            html.Title = Title;
            html.PositionType = PositionType;
            if (All == true)
            {
                html.All = true;
            }
            else
            {
                if (New == true) html.New = true;
                if (Edit == true) html.Edit = true;
                if (Index == true) html.Index = true;
                if (Calendar == true) html.Calendar = true;
                if (Crosstab == true) html.Crosstab = true;
                if (Gantt == true) html.Gantt = true;
                if (BurnDown == true) html.BurnDown = true;
                if (TimeSeries == true) html.TimeSeries = true;
                if (Kamban == true) html.Kamban = true;
                if (ImageLib == true) html.ImageLib = true;
            }
            if (Disabled == true) html.Disabled = true;
            html.Body = Body;
            return html;
        }
    }
}