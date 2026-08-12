namespace Implem.Pleasanter.Models
{
    public static class BackgroundJobStatus
    {
        public const int Pending = 0;
        public const int Running = 1;
        public const int Completed = 2;
        public const int Failed = 3;
        public const int Cancelled = 4;
        public const int RunningOverdue = 5;

        public static int Parse(
            string name,
            int defaultValue = Failed)
        {
            switch (name)
            {
                case nameof(Pending): return Pending;
                case nameof(Running): return Running;
                case nameof(Completed): return Completed;
                case nameof(Failed): return Failed;
                case nameof(Cancelled): return Cancelled;
                case nameof(RunningOverdue): return RunningOverdue;
                default: return defaultValue;
            }
        }
    }
}
