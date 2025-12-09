using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Form : ISettingListItem
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

        public string StartDate;
        public string EndDate;
        public string ClosingMessage;
        public string OutOfTermMessage;
        public string ErrorMessage;


        public Form()
        {
        }

        public Form(
        string startDate,
        string endDate,
        string closingMessage,
        string outOfTermMessage,
        string errorMessage
)
        {
            StartDate = startDate;
            EndDate = endDate;
            ClosingMessage = closingMessage;
            OutOfTermMessage = outOfTermMessage;
            ErrorMessage = errorMessage;
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

    }
}