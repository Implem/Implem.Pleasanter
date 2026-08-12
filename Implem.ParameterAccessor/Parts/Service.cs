namespace Implem.ParameterAccessor.Parts
{
    public class Service
    {
        public string Name { get; set; }
        public string EnvironmentName { get; set; }
        public string TimeZoneDefault { get; set; }
        public string DefaultPassword { get; set; }
        public string DeploymentEnvironment { get; set; }
        public bool WithoutChangeDefaultPassword { get; set; }
        public string DefaultLanguage { get; set; }
        public string AbsoluteUri { get; set; }
        public long MaxRequestBodySize { get; set; }
        public bool RequireHttps { get; set; }
        public long AnnouncementSiteId { get; set; }
        public bool ShowProfiles { get; set; }
        public bool ShowChangePassword { get; set; }
        public bool ShowStartGuide { get; set; }
        public bool Demo { get; set; }
        public bool DemoApi { get; set; }
        public int DemoUsagePeriod { get; set; }
        public bool RestrictNewFeatures { get; set; }
        public bool? DisableHtmlCache { get; set; } = null;
    }
}