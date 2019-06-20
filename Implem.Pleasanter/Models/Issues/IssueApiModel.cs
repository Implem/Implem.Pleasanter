using Implem.Pleasanter.Models.Shared;
using System;
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
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ItemTitle { get; set; }

        public IssueApiModel()
        {
        }
    }
}
