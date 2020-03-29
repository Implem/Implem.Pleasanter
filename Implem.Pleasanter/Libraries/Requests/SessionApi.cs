using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class SessionApi: Api
    {
        public string SessionKey;
        public string SessionValue;
        public bool SavePerUser = false;
    }
}