namespace Implem.ParameterAccessor.Parts
{
    /// <summary>
    /// Quartz.NET設定クラス
    /// </summary>
    public class Quartz
    {
        public QuartzClustering Clustering;
    }

    /// <summary>
    /// Quartzクラスタリング設定
    /// </summary>
    public class QuartzClustering
    {
        public bool Enabled;
        public string SchedulerName;
        public string InstanceId;
        public int CheckinInterval;
        public int MaxMisfireThreshold;
        public string TablePrefix;
        public string Serializer;
        public int MaxConcurrency;
        public string ThreadPriority;

        public QuartzClustering()
        {
            Enabled = false;
            SchedulerName = "PleasanterScheduler";
            InstanceId = "AUTO";
            CheckinInterval = 15000;
            MaxMisfireThreshold = 60000;
            TablePrefix = "QRTZ_";
            Serializer = "json";
            MaxConcurrency = 10;
            ThreadPriority = "Normal";
        }
    }
}
