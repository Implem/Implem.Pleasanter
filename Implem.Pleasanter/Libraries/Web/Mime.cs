using Implem.DefinitionAccessor;
namespace Implem.Pleasanter.Libraries.Web
{
    public static class Mime
    {
        public static string Type(string fileDownloadName)
        {
            return MimeKit.MimeTypes.GetMimeType(fileDownloadName);
        }

        public static bool ValidateOnApi(string contentType, bool isMultipart = false)
        {
            if (Parameters.Security.MimeTypeCheckOnApi)
            {
                switch (contentType?.ToLower())
                {
                    case "application/json":
                        if (isMultipart)
                        {
                            return false;
                        }
                        return true;
                    case "multipart/form-data":
                        if (isMultipart)
                        {
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }
            }
            return true;
        }
    }
}