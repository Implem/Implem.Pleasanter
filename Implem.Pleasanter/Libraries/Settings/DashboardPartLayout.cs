using Newtonsoft.Json;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class DashboardPartLayout
    {
        [JsonProperty(PropertyName = "x")]
        public int X { get; set; } = 0;

        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; } = 0;

        [JsonProperty(PropertyName = "w")]
        public int W { get; set; } = 1;

        [JsonProperty(PropertyName = "h")]
        public int H { get; set; } = 1;

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; } = string.Empty;
    }
}
