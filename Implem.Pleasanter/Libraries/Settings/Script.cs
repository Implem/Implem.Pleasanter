using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Script : ISettingListItem
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

        public Script()
        {
        }

        public Script(
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

        public Script GetRecordingData()
        {
            var script = new Script();
            script.Id = Id;
            script.Title = Title;
            if (All == true)
            {
                script.All = true;
            }
            else
            {
                if (New == true) script.New = true;
                if (Edit == true) script.Edit = true;
                if (Index == true) script.Index = true;
                if (Calendar == true) script.Calendar = true;
                if (Crosstab == true) script.Crosstab = true;
                if (Gantt == true) script.Gantt = true;
                if (BurnDown == true) script.BurnDown = true;
                if (TimeSeries == true) script.TimeSeries = true;
                if (Kamban == true) script.Kamban = true;
                if (ImageLib == true) script.ImageLib = true;
            }
            script.Body = Body;
            return script;
        }
    }
}