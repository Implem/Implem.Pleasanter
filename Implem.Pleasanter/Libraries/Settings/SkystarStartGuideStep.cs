using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class SkystarStartGuideStep
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Placements
        {
            Top,
            Bottom
        }

        public string Target { get; set; }
        public Placements Placement { get; set; } = Placements.Bottom;
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
