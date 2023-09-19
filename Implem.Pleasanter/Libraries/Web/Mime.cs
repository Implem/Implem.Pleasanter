using Implem.DefinitionAccessor;
namespace Implem.Pleasanter.Libraries.Web
{
    public static class Mime
    {
        public static string Type(string fileDownloadName)
        {
            return MimeKit.MimeTypes.GetMimeType(fileDownloadName);
        }

        public static bool ValidateOnApi(string contentType, bool multipart = false)
        {
            if (Parameters.Security.MimeTypeCheckOnApi)
            {
                switch (contentType?.ToLower())
                {
                    case "application/json":
                        return !multipart;
                    case "multipart/form-data":
                        return multipart;
                    default:
                        return false;
                }
            }
            return true;
        }
    }
}