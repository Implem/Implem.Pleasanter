using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class BackgroundServerScript : ServerScript
    {
        public bool? Disabled;
        public SettingList<BackgroundSchedule> backgoundSchedules = new SettingList<BackgroundSchedule>();
        public int UserId;
        public string ServerScriptSchedule1;
        public string ServerScriptSchedule2;

        public BackgroundServerScript()
        {
            Background = true;
            Disabled = false;
        }

        public BackgroundServerScript(
            int id,
            int userId,
            string title,
            string name,
            bool shared,
            bool disabled,
            string body,
            int? timeOut,
            IEnumerable<BackgroundSchedule> backgoundSchedules) : base(
                id: id,
                title: title,
                name: name,
                whenloadingSiteSettings: false,
                whenViewProcessing: false,
                whenloadingRecord: false,
                beforeFormula: false,
                afterFormula: false,
                beforeCreate: false,
                afterCreate: false,
                beforeUpdate: false,
                afterUpdate: false,
                beforeDelete: false,
                afterDelete: false,
                beforeOpeningPage: false,
                beforeOpeningRow: false,
                shared: shared,
                background: true,
                body: body,
                timeOut: timeOut)
        {
            Disabled = disabled;
            UserId = userId;
            this.backgoundSchedules = backgoundSchedules.ToJson().Deserialize<SettingList<BackgroundSchedule>>();
        }

        public BackgroundServerScript(int id, int userId, string title, string name, bool shared, bool disabled, string body, int timeOut)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Name = name;
            Shared = shared;
            Disabled = disabled;
            Body = body;
            TimeOut = timeOut;
        }

        public void Update(
            int userId,
            string title,
            string name,
            bool shared,
            bool disabled,
            string body,
            int? timeOut,
            IEnumerable<BackgroundSchedule> backgoundSchedules)
        {
            Update(
                title: title,
                name: name,
                whenloadingSiteSettings: false,
                whenViewProcessing: false,
                whenloadingRecord: false,
                beforeFormula: false,
                afterFormula: false,
                beforeCreate: false,
                afterCreate: false,
                beforeUpdate: false,
                afterUpdate: false,
                beforeDelete: false,
                afterDelete: false,
                beforeOpeningPage: false,
                beforeOpeningRow: false,
                shared: shared,
                background: true,
                body: body,
                timeOut: timeOut);
            Disabled = disabled;
            UserId = userId;
            this.backgoundSchedules = backgoundSchedules.ToJson().Deserialize<SettingList<BackgroundSchedule>>();
        }

        public new BackgroundServerScript GetRecordingData()
        {
            var script = this.GetRecordingData();
            if (Disabled == true) script.Disabled = true;
            script.UserId = UserId;
            script.backgoundSchedules = backgoundSchedules.Copy();
            return script;
        }
    }
}
