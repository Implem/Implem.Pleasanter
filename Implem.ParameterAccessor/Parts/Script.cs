using System.ComponentModel;

namespace Implem.ParameterAccessor.Parts
{
    public class Script
    {
        [DefaultValue(true)]
        public bool ServerScript { get; set; }  = true;
        public bool BackgroundServerScript { get; set; }
        public bool DisableServerScriptHttpClient { get; set;}
        [DefaultValue(10000)]
        public long ServerScriptTimeOut { get; set; } = 10000;
        public bool ServerScriptTimeOutChangeable { get; set; }
        [DefaultValue(0)]
        public int ServerScriptTimeOutMin { get; set; } = 0;
        [DefaultValue(1000 * 60 * 60 * 24)]
        public int ServerScriptTimeOutMax { get; set; } = 1000 * 60 * 60 * 24;
        [DefaultValue(100 * 1000)]
        public int ServerScriptHttpClientTimeOut { get; set; } = 100 * 1000;
        [DefaultValue(0)]
        public int ServerScriptHttpClientTimeOutMin { get; set; } = 0;
        [DefaultValue(1000 * 60 * 60 * 24)]
        public int ServerScriptHttpClientTimeOutMax { get; set; } = 1000 * 60 * 60 * 24;
        [DefaultValue(10)]
        public int ServerScriptIncludeDepthLimit { get; set; } = 10;
        [DefaultValue(true)]
        public bool DisableServerScriptFile { get; set; } = true;
        [DefaultValue(1)]
        public long ServerScriptFileSizeMax { get; set; } = 1;
        public string ServerScriptFilePath { get; set; }
    }
}