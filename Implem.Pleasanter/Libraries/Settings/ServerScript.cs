using Implem.Pleasanter.Interfaces;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class ServerScript : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
        public bool? BeforeOpeningPage;
        public bool? BeforeFormula;
        public bool? AfterFormula;

        public string Body;

        public ServerScript()
        {
        }

        public ServerScript(
            int id,
            string title,
            bool beforeOpeningPage,
            bool beforeFormula,
            bool afterFormula,
            string body)
        {
            Id = id;
            Title = title;
            BeforeOpeningPage = beforeOpeningPage;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            Body = body;
        }

        public void Update(
            string title,
            bool beforeOpeningPage,
            bool beforeFormula,
            bool afterFormula,
            string body)
        {
            Title = title;
            BeforeOpeningPage = beforeOpeningPage;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            Body = body;
        }

        public ServerScript GetRecordingData()
        {
            var script = new ServerScript();
            script.Id = Id;
            script.Title = Title;
            if (BeforeOpeningPage == true) script.BeforeOpeningPage = true;
            if (BeforeFormula == true) script.BeforeFormula = true;
            if (AfterFormula == true) script.AfterFormula = true;
            script.Body = Body;
            return script;
        }
    }
}