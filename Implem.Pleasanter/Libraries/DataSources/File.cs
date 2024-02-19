using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System.IO;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class File
    {
        public static void DeleteTemp(Context context, string guid)
        {
            if (Parameters.BinaryStorage.TemporaryBinaryStorageProvider == "Rds")
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.DeleteBinaries(
                        factory: context,
                        where: Rds.BinariesWhere()
                            .BinaryType("Temporary")
                            .Guid(guid)));
            }
            else
            {
                if (Directory.Exists(Path.Combine(Directories.Temp(), guid)))
                {
                    Directory.Delete(Path.Combine(Directories.Temp(), guid), true);
                }
            }
        }
        public static string Extension(this IHttpPostedFile file)
        {
            return Path.GetExtension(file.FileName);
        }

        public static byte[] Byte(this IHttpPostedFile file)
        {
            using (var inputStream = file.InputStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    inputStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public static string WriteToTemp(this IHttpPostedFile file)
        {
            var guid = Strings.NewGuid();
            if (Parameters.BinaryStorage.TemporaryBinaryStorageProvider == "Rds") return guid;
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