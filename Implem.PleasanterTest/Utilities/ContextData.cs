using Implem.Pleasanter.Libraries.Requests;
namespace Implem.PleasanterTest.Utilities
{
    public static class ContextData
    {
        public static Context Get(
            string httpMethod,
            string absolutePath,
            Forms forms = null)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            context.HttpMethod = httpMethod;
            context.Forms = forms ?? new Forms();
            context.Server = "http://localhost:59802";
            context.ApplicationPath = "/";
            context.AbsoluteUri = $"http://localhost:59802{absolutePath}";
            context.AbsolutePath = absolutePath;
            context.UserHostName = "::1";
            context.UserHostAddress = "::1";
            context.UserAgent = "Implem.PleasanterTest";
            return context;
        }
    }
}
