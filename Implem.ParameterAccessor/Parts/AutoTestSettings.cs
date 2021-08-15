namespace Implem.ParameterAccessor.Parts
{
    public class AutoTestSettings
    {
        public string Url;
        public BrowserTypes BrowserType;
        public int Width;
        public int Height;
        public bool ScreenShot;
        public string ScreenShotPath;
        public string ResultsPath;
        public string LogFileName;
    }

    public enum BrowserTypes
    {
        Chrome,
        IE
    }
}
