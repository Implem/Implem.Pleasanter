using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class TenantApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public int? Ver { get; set; }
        public string TenantName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ContractSettings { get; set; }
        public DateTime? ContractDeadline { get; set; }
        public bool? DisableAllUsersPermission { get; set; }
        public bool? DisableApi { get; set; }
        public bool? DisableStartGuide { get; set; }
        public int? LogoType { get; set; }
        public string HtmlTitleTop { get; set; }
        public string HtmlTitleSite { get; set; }
        public string HtmlTitleRecord { get; set; }
        public string TopStyle { get; set; }
        public string TopScript { get; set; }
        public string TopDashboards { get; set; }
        public string Theme { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string TenantSettings { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public TenantApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "TenantId": return TenantId;
                case "Ver": return Ver;
                case "TenantName": return TenantName;
                case "Title": return Title;
                case "Body": return Body;
                case "ContractSettings": return ContractSettings;
                case "ContractDeadline": return ContractDeadline;
                case "DisableAllUsersPermission": return DisableAllUsersPermission;
                case "DisableApi": return DisableApi;
                case "DisableStartGuide": return DisableStartGuide;
                case "LogoType": return LogoType;
                case "HtmlTitleTop": return HtmlTitleTop;
                case "HtmlTitleSite": return HtmlTitleSite;
                case "HtmlTitleRecord": return HtmlTitleRecord;
                case "TopStyle": return TopStyle;
                case "TopScript": return TopScript;
                case "TopDashboards": return TopDashboards;
                case "Theme": return Theme;
                case "Language": return Language;
                case "TimeZone": return TimeZone;
                case "TenantSettings": return TenantSettings;
                case "Comments": return Comments;
                case "Creator": return Creator;
                case "Updator": return Updator;
                case "CreatedTime": return CreatedTime;
                case "UpdatedTime": return UpdatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
