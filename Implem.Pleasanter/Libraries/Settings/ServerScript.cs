using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class ServerScript : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
        public bool? BeforeOpeningPage;
        public bool? WhenViewProcessing;
        public bool? BeforeFormula;
        public bool? AfterFormula;
        public bool? WhenloadingSiteSettings;
        public string Body;

        public ServerScript()
        {
        }

        public ServerScript(
            int id,
            string title,
            bool beforeOpeningPage,
            bool whenViewProcessing,
            bool beforeFormula,
            bool afterFormula,
            bool whenloadingSiteSettings,
            string body)
        {
            Id = id;
            Title = title;
            BeforeOpeningPage = beforeOpeningPage;
            WhenViewProcessing = whenViewProcessing;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            WhenloadingSiteSettings = whenloadingSiteSettings;
            Body = body;
        }

        public void Update(
            string title,
            bool beforeOpeningPage,
            bool whenViewProcessing,
            bool beforeFormula,
            bool afterFormula,
            bool whenloadingSiteSettings,
            string body)
        {
            Title = title;
            BeforeOpeningPage = beforeOpeningPage;
            WhenViewProcessing = whenViewProcessing;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            WhenloadingSiteSettings = whenloadingSiteSettings;
            Body = body;
        }

        public ServerScript GetRecordingData()
        {
            var script = new ServerScript();
            script.Id = Id;
            script.Title = Title;
            if (BeforeOpeningPage == true) script.BeforeOpeningPage = true;
            if (WhenViewProcessing == true) script.WhenViewProcessing = true;
            if (BeforeFormula == true) script.BeforeFormula = true;
            if (AfterFormula == true) script.AfterFormula = true;
            if (WhenloadingSiteSettings == true) script.WhenloadingSiteSettings = true;
            script.Body = Body;
            return script;
        }
    }
}