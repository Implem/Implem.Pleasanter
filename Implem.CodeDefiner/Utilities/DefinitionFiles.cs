using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using System.Collections.Generic;
namespace Implem.CodeDefiner.Utilities
{
    internal static class DefinitionFiles
    {
        internal const string CodeDefinitionFileName = "definition_Code.xlsm";

        internal static Dictionary<string, XlsIo> Collection()
        {
            var ret = new Dictionary<string, XlsIo>()
            {
                { "Code", Def.CodeXls },
                { "Column", Def.ColumnXls },
                { "Demo", Def.DemoXls },
                { "Template", Def.TemplateXls },
                { "ViewMode", Def.ViewModeXls },
                { "Sql", Def.SqlXls }
            };
            return ret;
        }
    }
}
