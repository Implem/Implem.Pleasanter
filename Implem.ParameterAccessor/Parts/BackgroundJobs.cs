namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundJobs
    {
        public bool BackgroundQueue = false;
        public int BackgroundJobDispatcherInterval = 60;
        public int BackgroundJobTimeout = 3600;
        public string FallbackLanguage = "ja";
        public string RecoverAction = "Failed";
        public string OutputFilePath = null;
    }
}
