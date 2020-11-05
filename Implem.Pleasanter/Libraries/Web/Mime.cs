﻿using Implem.DefinitionAccessor;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Web
{
    public static class Mime
    {
        public static string Type(string fileDownloadName)
        {
            switch (Extension(fileDownloadName).ToLower())
            {
                case "csv":
                    return "text/comma-separated-values";
                default:
                    return "application/octet-stream";
            }
        }

        private static string Extension(string fileDownloadName)
        {
            if (fileDownloadName.IndexOf(".") != -1)
            {
                return fileDownloadName.Split('.').Last();
            }
            else
            {
                return string.Empty;
            }
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