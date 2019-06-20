using Implem.CodeDefiner.Utilities;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.IO;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class Merger
    {
        internal static void Merge(string fileName, string code, bool margeToExisting)
        {
            var newCode = code;
            var path = Path.Combine(Directories.ServicePath(), fileName);
            if (path.Exists())
            {
                var existingCode = Files.Read(path);
                if (path.FileExtension() == ".cs")
                {
                    newCode = MergedCode(newCode, existingCode, fileName, margeToExisting);
                }
                CodeWriter.Write(path, newCode, existingCode);
            }
            else
            {
                CodeWriter.Write(path, newCode);
            }
        }

        private static string MergedCode(
            string newCode, string existingCode, string fileName, bool margeToExisting)
        {
            var newCsParser = new Parser(newCode);
            var existingCsParser = new Parser(existingCode);
            if (margeToExisting)
            {
                existingCsParser.MergeCode(newCsParser, margeToExisting);
                return existingCsParser.Code();
            }
            else
            {
                newCsParser.MergeCode(existingCsParser, margeToExisting);
                return newCsParser.Code();
            }
        }
    }
}
