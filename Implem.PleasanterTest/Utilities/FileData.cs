using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class FileData
    {
        public static string WriteToTemp(this byte[] bytes, string fileName)
        {
            var guid = Strings.NewGuid();
            var folderPath = Path.Combine(Path.Combine(Directories.Temp(), guid));
            if (!folderPath.Exists()) Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(
                folderPath,
                Path.GetFileName(fileName));
            using (var bw = new BinaryWriter(File.OpenWrite(filePath)))
            {
                bw.Write(bytes);
            }
            return guid;
        }

        public static List<FileTest> Tests(params FileTest[] tests)
        {
            return tests.ToList();
        }

        public static FileTest Exists()
        {
            return new FileTest()
            {
                Type = FileTest.Types.Exists
            };
        }
    }
}
