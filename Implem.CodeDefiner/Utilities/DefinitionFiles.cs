using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Implem.CodeDefiner.Utilities
{
    internal static class DefinitionFiles
    {
        internal const string CodeDefinitionFileName = "definition_Code.xlsm";

        internal static Dictionary<string, XlsIo> Collection()
        {
            return Directory
                .EnumerateFiles(Directories.Definitions(), "definition_*.xlsm")
                .Select(o => Path.GetFileName(o))
                .ToDictionary(
                    o => o.Replace("definition_", string.Empty).FileNameOnly(),
                    o => Initializer.DefinitionFile(o));
        }
    }
}
