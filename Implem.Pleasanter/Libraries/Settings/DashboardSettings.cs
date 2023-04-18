using Implem.Pleasanter.Libraries.DataTypes;
using Newtonsoft.Json;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class DashboardSettings
    {
        public DashbordType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public QuickAccess QuickAccess { get; set; }
        public TimeLine TimeLine { get; set; }
    }

    public enum DashbordType
    {
        QuickAccess,
        TimeLine
    }

    public class QuickAccess
    {
        public long[] Sites { get; set; }
    }

    public class TimeLine
    {
        public int NumberOfItems { get; set; } = 20;
    }

    public class DashboardLayout
    {
        [JsonProperty(PropertyName = "x")]
        public int? X { get; set; } = default;

        [JsonProperty(PropertyName = "y")]
        public int? Y { get; set; } = default;

        [JsonProperty(PropertyName = "w")]
        public int? W { get; set; } = default;

        [JsonProperty(PropertyName = "h")]
        public int? H { get; set; } = default;

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; } = default;
    }

    public class TimeLineItem
    {
        public long Id { get; set; }
        public Title Title { get; set; }
        public string Body { get; set; }
        public Time CreatedTime { get; set; }
        public Time UpdatedTime { get; set; }
        public User Creator { get; set; }
        public User Updator { get; set; }
    }
}
