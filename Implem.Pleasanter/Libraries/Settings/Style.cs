using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Style : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
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

        public Style()
        {
        }

        public Style(
             int? id,
             string title,
             bool? all,
             bool? _new,
             bool? edit,
             bool? index,
             bool? calendar,
             bool? crosstab,
             bool? gantt,
             bool? burnDown,
             bool? timeSeries,
             bool? kamban,
             bool? imageLib,
             bool? disabled,
             string body)
        {
            Id = id.ToInt();
            Title = title;
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
            bool? all,
            bool? _new,
            bool? edit,
            bool? index,
            bool? calendar,
            bool? crosstab,
            bool? gantt,
            bool? burnDown,
            bool? timeSeries,
            bool? kamban,
            bool? imageLib,
            bool? disabled,
            string body)
        {
            Title = title;
            if (all != null) All = all;
            if (_new != null) New = _new;
            if (edit != null) Edit = edit;
            if (index != null) Index = index;
            if (calendar != null) Calendar = calendar;
            if (crosstab != null) Crosstab = crosstab;
            if (gantt != null) Gantt = gantt;
            if (burnDown != null) BurnDown = burnDown;
            if (timeSeries != null) TimeSeries = timeSeries;
            if (kamban != null) Kamban = kamban;
            if (imageLib != null) ImageLib = imageLib;
            if (disabled != null) Disabled = disabled;
            if (body != null) Body = body;
        }

        public void UpdateByApi(
           string title,
           bool? all,
           bool? _new,
           bool? edit,
           bool? index,
           bool? calendar,
           bool? crosstab,
           bool? gantt,
           bool? burnDown,
           bool? timeSeries,
           bool? kamban,
           bool? imageLib,
           bool? disabled,
           string body)
        {
            Title = title;
            if (all != null) All = all;
            if (_new != null) New = _new;
            if (edit != null) Edit = edit;
            if (index != null) Index = index;
            if (calendar != null) Calendar = calendar;
            if (crosstab != null) Crosstab = crosstab;
            if (gantt != null) Gantt = gantt;
            if (burnDown != null) BurnDown = burnDown;
            if (timeSeries != null) TimeSeries = timeSeries;
            if (kamban != null) Kamban = kamban;
            if (imageLib != null) ImageLib = imageLib;
            if (disabled != null) Disabled = disabled;
            if (body != null) Body = body;
        }

        public Style GetRecordingData()
        {
            var style = new Style();
            style.Id = Id;
            style.Title = Title;
            if (All == true)
            {
                style.All = true;
            }
            else
            {
                if (New == true) style.New = true;
                if (Edit == true) style.Edit = true;
                if (Index == true) style.Index = true;
                if (Calendar == true) style.Calendar = true;
                if (Crosstab == true) style.Crosstab = true;
                if (Gantt == true) style.Gantt = true;
                if (BurnDown == true) style.BurnDown = true;
                if (TimeSeries == true) style.TimeSeries = true;
                if (Kamban == true) style.Kamban = true;
                if (ImageLib == true) style.ImageLib = true;
            }
            if (Disabled == true) style.Disabled = true;
            style.Body = Body;
            return style;
        }
    }
}