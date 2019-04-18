using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Api
    {
        public string ApiKey;
        public View View;
        public int Offset;
        public Sqls.TableTypes TableType;
    }
}