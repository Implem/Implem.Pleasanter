using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.IO;
using System.Web;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class File
    {
        public static void DeleteTemp(string guid)
        {
            if (Directory.Exists(Path.Combine(Directories.Temp(), guid)))
            {
                Directory.Delete(Path.Combine(Directories.Temp(), guid), true);
            }
        }

        public static string Extension(this HttpPostedFileBase file)
        {
            return Path.GetExtension(file.FileName);
        }

        public static byte[] Byte(this HttpPostedFileBase file)
        {
            using (var inputStream = file.InputStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    inputStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public static string WriteToTemp(this HttpPostedFileBase file)
        {
            var guid = Strings.NewGuid();
            var folderPath = Path.Combine(Path.Combine(Directories.Temp(), guid));
            if (!folderPath.Exists()) Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(
                folderPath,
                Path.GetFileName(file.FileName));
            file.SaveAs(filePath);
            return guid;
        }
    }
}