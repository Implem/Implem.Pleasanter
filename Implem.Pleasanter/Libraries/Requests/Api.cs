using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class Api
    {
        public decimal ApiVersion { get; set; } = 1.000M;
        public string ApiKey { get; set; }
        public View View { get; set; }
        public int Offset { get; set; }
        public Sqls.TableTypes TableType { get; set; }

        public Api()
        {
        }
    }
}