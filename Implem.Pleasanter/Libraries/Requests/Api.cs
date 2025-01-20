using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class Api
    {
        public decimal ApiVersion { get; set; } = Parameters.Api.Version;
        public string ApiKey { get; set; }
        public View View { get; set; }
        public List<string> Keys { get; set; }
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public Sqls.TableTypes TableType { get; set; }
        public string Token { get; set; }

        public Api()
        {
        }
    }
}