using Newtonsoft.Json;
using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class ContentSecurityPolicyValues
    {
        [JsonProperty(PropertyName = "default-src")]
        public string DefaultSrc { get; init; }

        [JsonProperty(PropertyName = "script-src")]
        public string ScriptSrc { get; init; }

        [JsonProperty(PropertyName = "script-src-attr")]
        public string ScriptSrcAttr { get; init; }

        [JsonProperty(PropertyName = "style-src")]
        public string StyleSrc { get; init; }

        [JsonProperty(PropertyName = "style-src-attr")]
        public string StyleSrcAttr { get; init; }

        [JsonProperty(PropertyName = "style-src-elem")]
        public string StyleSrcElem { get; init; }

        [JsonProperty(PropertyName = "img-src")]
        public string ImgSrc { get; init; }

        [JsonProperty(PropertyName = "font-src")]
        public string FontSrc { get; init; }

        [JsonProperty(PropertyName = "object-src")]
        public string ObjectSrc { get; init; }

        [JsonProperty(PropertyName = "connect-src")]
        public string ConnectSrc { get; init; }

        [JsonProperty(PropertyName = "frame-src")]
        public string FrameSrc { get; init; }

        [JsonProperty(PropertyName = "base-uri")]
        public string BaseUri { get; init; }

        [JsonProperty(PropertyName = "form-action")]
        public string FormAction { get; init; }

        [JsonProperty(PropertyName = "report-uri")]
        public string ReportUri { get; init; }
    }
}
