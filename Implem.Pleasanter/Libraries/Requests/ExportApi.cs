using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class ExportApi : Api
    {
        public int ExportId { get; set; }
        public Export Export { get; set; }

        public ExportApi()
        {
        }
    }
}