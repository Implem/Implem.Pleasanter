using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class ProcessApiSettingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ScreenType { get; set; }
        public int? CurrentStatus { get; set; }
        public int? ChangedStatus { get; set; }
        public string Description { get; set; }
        public string Tooltip { get; set; }
        public string ConfirmationMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string OnClick { get; set; }
        public string ExecutionType { get; set; }
        public string ActionType { get; set; }
        public bool? AllowBulkProcessing { get; set; }
        public string ValidationType { get; set; }
        public SettingList<ValidateInput> ValidateInputs { get; set; }
        public List<int> Depts { get; set; }
        public List<int> Groups { get; set; }
        public List<int> Users { get; set; }
        public View View { get; set; }
        public string ErrorMessage { get; set; }
        public SettingList<DataChange> DataChanges { get; set; }
        public AutoNumbering AutoNumbering { get; set; }
        public ApiSiteSettingPermission Permission { get; set; }
        public SettingList<Notification> Notifications { get; set; }
        [NonSerialized]
        public bool? MatchConditions;
        public int? Delete;

        public ProcessApiSettingModel()
        {
        }
    }
}
