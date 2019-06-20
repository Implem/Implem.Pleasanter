using System.IO;
using System.Text;
namespace Implem.Libraries.Utilities
{
    public static class Bytes
    {
        public static byte[] ToBytes(this string self, Encoding encoding = null)
        {
            var _encoding = encoding ?? Encoding.UTF8;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, _encoding))
            {
                streamWriter.Write(self);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
