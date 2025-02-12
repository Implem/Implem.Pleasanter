using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Requests
{
    [Serializable]
    public class ExportServerScript : ExportApi
    {
        public string Encoding { get; set; }

        public ExportServerScript()
        {
        }
    }
}