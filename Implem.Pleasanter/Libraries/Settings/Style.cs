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
        public string Body;

        public Style()
        {
        }

        public Style(
            int id,
            string title,
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
            string body)
        {
            Id = id;
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
            Body = body;
        }

        public void Update(
            string title,
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
            string body)
        {
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
            Body = body;
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
            style.Body = Body;
            return style;
        }
    }
}