using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class WikiApiModel
    {
        public long? WikiId;
        public string Title;
        public string Body;
        public string Timestamp;
        public string Comments;
        public bool? VerUp;
    }
}
