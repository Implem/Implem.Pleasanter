using System;
using System.IO;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.DataSources.BinaryStorages
{
    public interface IBinaryStorageProvider
    {
        string Name { get; }  // BinaryStorageProvider() の戻り値に対応


        void Upload(string objectName, byte[] data, string contentType);
        void Upload(string objectName, Stream stream, string contentType);
        byte[] Download(string objectName);
        Stream OpenRead(string objectName);
        void Delete(string objectName);
        bool Exists(string objectName);
        DateTime LastWriteTime(string objectName);

        Task UploadAsync(string objectName, byte[] data, string contentType);
        Task UploadAsync(string objectName, Stream stream, string contentType);
        Task<byte[]> DownloadAsync(string objectName);
        Task<Stream> OpenReadAsync(string objectName);
        Task DeleteAsync(string objectName);
        Task<bool> ExistsAsync(string objectName);
        Task<DateTime> LastWriteTimeAsync(string objectName);
    }
}
