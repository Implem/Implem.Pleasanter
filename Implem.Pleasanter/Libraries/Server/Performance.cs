using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.IO;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Performance
    {
        public static DateTime GeneratedTime = DateTime.Now;
        public static DateTime PreviousTime = DateTime.Now;

        public static void WriteLog(string line)
        {
            var fileName = GeneratedTime.ToString("yyyyMMdd_HHmmss") + ".csv";
            var filePath = Path.Combine(
                Directories.Logs(),
                fileName);
            var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            var diff = (DateTime.Now - PreviousTime).TotalMilliseconds;
            Files.WriteLine(
                filePath: filePath,
                line: $"{time},{diff},{line}");
        }
    }
}