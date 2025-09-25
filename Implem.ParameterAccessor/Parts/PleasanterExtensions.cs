namespace Implem.ParameterAccessor.Parts
{
    public class PleasanterExtensions
    {
        public class SiteVisualizerData
        {
            public bool Disabled { get; set; } = false;
            public int ErdLinkDepth { get; set; } = 10;
            public int ErdLinkLimit { get; set; } = 60;
        }
        public SiteVisualizerData SiteVisualizer = new();
    }
}
