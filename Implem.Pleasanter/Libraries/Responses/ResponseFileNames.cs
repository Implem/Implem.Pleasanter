using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseFileNames
    {
        public static string Csv(SiteModel siteModel, string name)
        {
            return Files.ValidFileName("_".JoinParam(
                siteModel.Title.Value,
                name,
                DateTime.Now.ToLocal(Displays.YmdhmsFormat())) + ".csv");
        }
    }
}