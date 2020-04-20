using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Utilities
{
    internal static class CodeWriter
    {
        internal static void Write(string codePath, string newCode, string existingCode = "")
        {
            if (existingCode.IsNullOrEmpty() || newCode != existingCode)
            {
                Consoles.Write(
                    codePath.Substring(Directories.ServicePath().Length),
                    Consoles.Types.Info);
                newCode.Write(codePath);
            }
            else
            {
                Consoles.Write("-", Consoles.Types.Info);
            }
        }
    }
}
