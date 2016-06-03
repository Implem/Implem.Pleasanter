using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseFileNames
    {
        public static string Csv(SiteModel siteModel)
        {
            return Files.ValidFileName("_".JoinParam(
                siteModel.Title.Value,
                (Sessions.PageSession(siteModel.Id, "ExportSettings_Title") as Title).Value,
                Implem.Libraries.Utilities.DateTimes.Full()) + ".csv");
        }
    }
}