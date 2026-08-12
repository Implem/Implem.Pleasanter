using System;
using System.IO;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;

namespace Implem.Pleasanter.Libraries.DataSources.BinaryStorages
{
    public class LocalFolderBinaryStorageProvider : IBinaryStorageProvider
    {
        public string Name => BinaryStorageProviderNames.LocalFolder;

        private static string ResolvePath(string objectName)
        {
            ValidateObjectName(objectName);
            var relevantPath = objectName.Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Directories.BinaryStorage(), relevantPath);
        }

        private static void ValidateObjectName(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentException(
                    "objectName is required.",
                    nameof(objectName));
            }
            if (Path.IsPathRooted(objectName))
            {
                throw new ArgumentException(
                    "Absolute paths are not allowed.",
                    nameof(objectName));
            }
            if (objectName.Length >= 2
                && objectName[1] == ':'
                && char.IsLetter(objectName[0]))
            {
                throw new ArgumentException(
                    "Absolute paths are not allowed.",
                    nameof(objectName));
            }
            if (objectName.StartsWith(@"\\")
                || objectName.StartsWith("//"))
            {
                throw new ArgumentException(
                    "UNC paths are not allowed.",
                    nameof(objectName));
            }
            if (objectName.Contains(".."))
            {
                throw new ArgumentException(
                    "Path traversal is not allowed.",
                    nameof(objectName));
            }
        }

        public void Upload(string objectName, byte[] data, string contentType)
        {
            data.Write(ResolvePath(objectName));
        }

        public void Upload(string objectName, Stream stream, string contentType)
        {
            var filePath = ResolvePath(objectName);
            var dir = new FileInfo(filePath).Directory;
            if (!dir.Exists)
            {
                Directory.CreateDirectory(dir.FullName);
            }
            using var fs = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);
            stream.CopyTo(fs);
        }

        public byte[] Download(string objectName)
        {
            return Files.Bytes(ResolvePath(objectName));
        }

        public Stream OpenRead(string objectName)
        {
            return System.IO.File.OpenRead(ResolvePath(objectName));
        }

        public void Delete(string objectName)
        {
            var filePath = ResolvePath(objectName);
            if (System.IO.File.Exists(filePath))
            {
                Files.DeleteFile(filePath);
            }
        }

        public bool Exists(string objectName)
        {
            return System.IO.File.Exists(ResolvePath(objectName));
        }

        public DateTime LastWriteTime(string objectName)
        {
            var filePath = ResolvePath(objectName);
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Exists
                    ? fileInfo.LastWriteTimeUtc // UTC時刻を使用
                    : DateTime.FromOADate(0);
        }

        public async Task UploadAsync(string objectName, byte[] data, string contentType)
        {
            var filePath = ResolvePath(objectName);
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            await System.IO.File.WriteAllBytesAsync(
                filePath,
                data);
        }

        public async Task UploadAsync(string objectName, Stream stream, string contentType)
        {
            var filePath = ResolvePath(objectName);
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using var fs = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);
            await stream.CopyToAsync(fs);
        }

        public async Task<byte[]> DownloadAsync(string objectName)
        {
            return await System.IO.File.ReadAllBytesAsync(ResolvePath(objectName));
        }

        public Task<Stream> OpenReadAsync(string objectName)
        {
            Stream stream = System.IO.File.OpenRead(ResolvePath(objectName));
            return Task.FromResult(stream);
        }

        public Task DeleteAsync(string objectName)
        {
            Delete(objectName);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string objectName)
        {
            return Task.FromResult(Exists(objectName));
        }

        public Task<DateTime> LastWriteTimeAsync(string objectName)
        {
            return Task.FromResult(LastWriteTime(objectName));
        }
    }
}
