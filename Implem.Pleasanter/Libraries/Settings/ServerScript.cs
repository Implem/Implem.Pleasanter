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
        public bool? BeforeCreate;
        public bool? AfterCreate;
        public bool? BeforeUpdate;
        public bool? AfterUpdate;
        public bool? BeforeDelete;
        public bool? AfterDelete;
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
            bool beforeCreate,
            bool afterCreate,
            bool beforeUpdate,
            bool afterUpdate,
            bool beforeDelete,
            bool afterDelete,
            string body)
        {
            Id = id;
            Title = title;
            BeforeOpeningPage = beforeOpeningPage;
            WhenViewProcessing = whenViewProcessing;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            WhenloadingSiteSettings = whenloadingSiteSettings;
            BeforeCreate = beforeCreate;
            AfterCreate = afterCreate;
            BeforeUpdate = beforeUpdate;
            AfterUpdate = afterUpdate;
            BeforeDelete = beforeDelete;
            AfterDelete = afterDelete;
            Body = body;
        }

        public void Update(
            string title,
            bool beforeOpeningPage,
            bool whenViewProcessing,
            bool beforeFormula,
            bool afterFormula,
            bool whenloadingSiteSettings,
            bool beforeCreate,
            bool afterCreate,
            bool beforeUpdate,
            bool afterUpdate,
            bool beforeDelete,
            bool afterDelete,
            string body)
        {
            Title = title;
            BeforeOpeningPage = beforeOpeningPage;
            WhenViewProcessing = whenViewProcessing;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            WhenloadingSiteSettings = whenloadingSiteSettings;
            BeforeCreate = beforeCreate;
            AfterCreate = afterCreate;
            BeforeUpdate = beforeUpdate;
            AfterUpdate = afterUpdate;
            BeforeDelete = beforeDelete;
            AfterDelete = afterDelete;
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
            if (BeforeCreate == true) script.BeforeCreate = true;
            if (AfterCreate == true) script.AfterCreate = true;
            if (BeforeUpdate == true) script.BeforeUpdate = true;
            if (AfterUpdate == true) script.AfterUpdate = true;
            if (BeforeDelete == true) script.BeforeDelete = true;
            if (AfterDelete == true) script.AfterDelete = true;
            script.Body = Body;
            return script;
        }
    }
}