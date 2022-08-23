using Implem.DefinitionAccessor;
namespace Implem.Pleasanter.Libraries.Web
{
    public static class Mime
    {
        public static string Type(string fileDownloadName)
        {
            return MimeKit.MimeTypes.GetMimeType(fileDownloadName);
        }

        public static bool ValidateOnApi(string contentType)
        {
            if (Parameters.Security.MimeTypeCheckOnApi)
            {
                switch (contentType?.ToLower())
                {
                    case "application/json":
                        return true;
                    default:
                        return false;
                }
            }
            return true;
        }
    }
}