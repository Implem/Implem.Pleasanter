using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class DashboardSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public long[] QuickAccessSites { get; set; }
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
}
