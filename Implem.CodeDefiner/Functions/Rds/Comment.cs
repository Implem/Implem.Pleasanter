using System;
namespace Implem.CodeDefiner.Functions.Rds
{
    public class Comment
    {
        public int CommentId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public int Creator { get; set; }
        public int? Updator { get; set; }
        public string Body { get; set; }
    }
}