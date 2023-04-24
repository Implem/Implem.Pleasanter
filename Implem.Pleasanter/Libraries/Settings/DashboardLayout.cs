using Newtonsoft.Json;

namespace Implem.Pleasanter.Libraries.Settings
{
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
}
