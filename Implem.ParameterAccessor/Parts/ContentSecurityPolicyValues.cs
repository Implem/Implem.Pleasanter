using Newtonsoft.Json;

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

        [JsonProperty(PropertyName = "script-src-elem")]
        public string ScriptSrcElem { get; init; }

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

        [JsonProperty(PropertyName = "manifest-src")]
        public string ManifestSrc { get; init; }

        [JsonProperty(PropertyName = "media-src")]
        public string MediaSrc { get; init; }

        [JsonProperty(PropertyName = "worker-src")]
        public string WorkerSrc { get; init; }

        [JsonProperty(PropertyName = "base-uri")]
        public string BaseUri { get; init; }

        [JsonProperty(PropertyName = "form-action")]
        public string FormAction { get; init; }

        [JsonProperty(PropertyName = "frame-ancestors")]
        public string FrameAncestors { get; init; }

        [JsonProperty(PropertyName = "report-uri")]
        public string ReportUri { get; init; }

        [JsonProperty(PropertyName = "report-to")]
        public string ReportTo { get; init; }

        [JsonProperty(PropertyName = "sandbox")]
        public string Sandbox { get; init; }

        [JsonProperty(PropertyName = "upgrade-insecure-requests")]
        public bool UpgradeInsecureRequests { get; init; }
    }
}
