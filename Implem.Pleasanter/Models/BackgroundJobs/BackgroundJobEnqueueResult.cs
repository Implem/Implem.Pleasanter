namespace Implem.Pleasanter.Models
{
    public class BackgroundJobEnqueueResult
    {
        public long BackgroundJobId { get; set; }

        public bool Succeeded { get; set; }

        public string FailureMessageDisplayId { get; set; }
    }
}
