using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Utilities
{
    internal static class CodeWriter
    {
        internal static void Write(string codePath, string newCode, string existingCode = "")
        {
            if (existingCode == string.Empty || newCode != existingCode)
            {
                if (existingCode != string.Empty)
                {
                    CodeHistories.Create(codePath);
                }
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
