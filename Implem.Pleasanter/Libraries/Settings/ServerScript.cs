using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class ServerScript : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
        public string Name;
        public bool? WhenloadingSiteSettings;
        public bool? WhenViewProcessing;
        public bool? WhenloadingRecord;
        public bool? BeforeFormula;
        public bool? AfterFormula;
        public bool? BeforeCreate;
        public bool? AfterCreate;
        public bool? BeforeUpdate;
        public bool? AfterUpdate;
        public bool? BeforeDelete;
        public bool? AfterDelete;
        public bool? BeforeOpeningPage;
        public bool? BeforeOpeningRow;
        public bool? Shared;
        public string Body;
        public bool? Functionalize;
        public bool? TryCatch;
        public int? TimeOut;
        [NonSerialized]
        public bool Debug;
        public bool? Background;

        public ServerScript()
        {
        }

        public ServerScript(
            int? id,
            string title,
            string name,
            bool? whenloadingSiteSettings,
            bool? whenViewProcessing,
            bool? whenloadingRecord,
            bool? beforeFormula,
            bool? afterFormula,
            bool? beforeCreate,
            bool? afterCreate,
            bool? beforeUpdate,
            bool? afterUpdate,
            bool? beforeDelete,
            bool? afterDelete,
            bool? beforeOpeningPage,
            bool? beforeOpeningRow,
            string body,
            bool? functionalize,
            bool? tryCatch,
            bool? shared,
            bool? background,
            int? timeOut)
        {
            Id = id.ToInt();
            Title = title;
            Name = name;
            WhenloadingSiteSettings = whenloadingSiteSettings;
            WhenViewProcessing = whenViewProcessing;
            WhenloadingRecord = whenloadingRecord;
            BeforeFormula = beforeFormula;
            AfterFormula = afterFormula;
            BeforeCreate = beforeCreate;
            AfterCreate = afterCreate;
            BeforeUpdate = beforeUpdate;
            AfterUpdate = afterUpdate;
            BeforeDelete = beforeDelete;
            AfterDelete = afterDelete;
            BeforeOpeningPage = beforeOpeningPage;
            BeforeOpeningRow = beforeOpeningRow;
            Body = body;
            Functionalize = functionalize;
            TryCatch = tryCatch;
            Shared = shared;
            TimeOut = timeOut;
            Background = background;
        }

        public void Update(
            string title,
            string name,
            bool? whenloadingSiteSettings,
            bool? whenViewProcessing,
            bool? whenloadingRecord,
            bool? beforeFormula,
            bool? afterFormula,
            bool? beforeCreate,
            bool? afterCreate,
            bool? beforeUpdate,
            bool? afterUpdate,
            bool? beforeDelete,
            bool? afterDelete,
            bool? beforeOpeningPage,
            bool? beforeOpeningRow,
            bool? shared,
            bool? background,
            string body,
            bool? functionalize,
            bool? tryCatch,
            int? timeOut)
        {
            Title = title;
            if (name != null) Name = name;
            if (whenloadingSiteSettings != null) WhenloadingSiteSettings = whenloadingSiteSettings;
            if (whenViewProcessing != null) WhenViewProcessing = whenViewProcessing;
            if (whenloadingRecord != null) WhenloadingRecord = whenloadingRecord;
            if (beforeFormula != null) BeforeFormula = beforeFormula;
            if (afterFormula != null) AfterFormula = afterFormula;
            if (beforeCreate != null) BeforeCreate = beforeCreate;
            if (afterCreate != null) AfterCreate = afterCreate;
            if (beforeUpdate != null) BeforeUpdate = beforeUpdate;
            if (afterUpdate != null) AfterUpdate = afterUpdate;
            if (beforeDelete != null) BeforeDelete = beforeDelete;
            if (afterDelete != null) AfterDelete = afterDelete;
            if (beforeOpeningPage != null) BeforeOpeningPage = beforeOpeningPage;
            if (beforeOpeningRow != null) BeforeOpeningRow = beforeOpeningRow;
            if (shared != null) Shared = shared;
            if (background != null) Background = background;
            if (body != null) Body = body;
            if (functionalize != null) Functionalize = functionalize;
            if (tryCatch != null) TryCatch = tryCatch;
            if (timeOut != null) TimeOut = timeOut;
        }

        public ServerScript GetRecordingData()
        {
            var script = new ServerScript();
            script.Id = Id;
            script.Title = Title;
            script.Name = Name;
            if (WhenloadingSiteSettings == true) script.WhenloadingSiteSettings = true;
            if (WhenViewProcessing == true) script.WhenViewProcessing = true;
            if (WhenloadingRecord == true) script.WhenloadingRecord = true;
            if (BeforeFormula == true) script.BeforeFormula = true;
            if (AfterFormula == true) script.AfterFormula = true;
            if (BeforeCreate == true) script.BeforeCreate = true;
            if (AfterCreate == true) script.AfterCreate = true;
            if (BeforeUpdate == true) script.BeforeUpdate = true;
            if (AfterUpdate == true) script.AfterUpdate = true;
            if (BeforeDelete == true) script.BeforeDelete = true;
            if (AfterDelete == true) script.AfterDelete = true;
            if (BeforeOpeningPage == true) script.BeforeOpeningPage = true;
            if (BeforeOpeningRow == true) script.BeforeOpeningRow = true;
            if (Shared == true) script.Shared = true;
            if (Background == true) script.Background = true;
            script.Body = Body;
            if (Functionalize == true) script.Functionalize = true;
            if (TryCatch == true) script.TryCatch = true;
            if (TimeOut != Parameters.Script.ServerScriptTimeOut && Parameters.Script.ServerScriptTimeOutChangeable)
            {
                script.TimeOut = TimeOut;
            }
            return script;
        }

        public void SetDebug()
        {
            Debug = Body?.StartsWith("//debug//") == true;
        }
    }
}