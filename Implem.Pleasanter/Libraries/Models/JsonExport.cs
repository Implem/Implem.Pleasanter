using Implem.Pleasanter.Interfaces;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Models
{
    public class JsonExport
    {
        public List<JsonExportColumn> Header;
        public List<IExportModel> Body;
    }
}