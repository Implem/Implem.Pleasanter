using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class IssueApiModel : _BaseApiModel
    {
        public long? SiteId { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? IssueId { get; set; }
        public int? Ver { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public decimal? WorkValue { get; set; }
        public decimal? ProgressRate { get; set; }
        public decimal? RemainingWorkValue { get; set; }
        public int? Status { get; set; }
        public int? Manager { get; set; }
        public int? Owner { get; set; }
        public bool? Locked { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ItemTitle { get; set; }
        public int? ProcessId { get; set; }
        public int?[] ProcessIds { get; set; }

        public IssueApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "SiteId": return SiteId;
                case "UpdatedTime": return UpdatedTime;
                case "IssueId": return IssueId;
                case "Ver": return Ver;
                case "Title": return Title;
                case "Body": return Body;
                case "StartTime": return StartTime;
                case "CompletionTime": return CompletionTime;
                case "WorkValue": return WorkValue;
                case "ProgressRate": return ProgressRate;
                case "RemainingWorkValue": return RemainingWorkValue;
                case "Status": return Status;
                case "Manager": return Manager;
                case "Owner": return Owner;
                case "Locked": return Locked;
                case "Comments": return Comments;
                case "Creator": return Creator;
                case "Updator": return Updator;
                case "CreatedTime": return CreatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
