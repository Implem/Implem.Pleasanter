using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.IO;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class PostedFile
    {
        public string Guid;
        public string FileName;
        public string Extension;
        public long Size;
        public string ContentType;

        public byte[] Byte()
        {
            return Files.Bytes(Path.Combine(Directories.Temp(), Guid, FileName));
        }
    }
}