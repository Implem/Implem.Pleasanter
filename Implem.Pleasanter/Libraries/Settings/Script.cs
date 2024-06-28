using Implem.Libraries.Utilities;
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
        public bool? Analy;
        public bool? Kamban;
        public bool? ImageLib;
        public bool? Disabled;
        public string Body;

        public Script()
        {
        }

        public Script(
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
            bool? analy,
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
            Analy = analy;
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
            bool? analy,
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
            if (analy != null) Analy = analy;
            if (kamban != null) Kamban = kamban;
            if (imageLib != null) ImageLib = imageLib;
            if (disabled != null) Disabled = disabled;
            if (body != null) Body = body;
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
                if (Analy == true) script.Analy = true;
                if (Kamban == true) script.Kamban = true;
                if (ImageLib == true) script.ImageLib = true;
            }
            if (Disabled == true) script.Disabled = true;
            script.Body = Body;
            return script;
        }
    }
}