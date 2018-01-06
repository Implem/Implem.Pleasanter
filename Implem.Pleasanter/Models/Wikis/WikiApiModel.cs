using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class WikiApiModel
    {
        public long? SiteId;
        public DateTime? UpdatedTime;
        public long? WikiId;
        public int? Ver;
        public string Title;
        public string Body;
        public string Comments;
        public int? Creator;
        public int? Updator;
        public DateTime? CreatedTime;
        public bool? VerUp;
    }
}
