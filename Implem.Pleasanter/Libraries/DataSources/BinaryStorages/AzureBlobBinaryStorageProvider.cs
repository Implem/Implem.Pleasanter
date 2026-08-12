using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Implem.DefinitionAccessor;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;

namespace Implem.Pleasanter.Libraries.DataSources.BinaryStorages
{
    public class AzureBlobBinaryStorageProvider : IBinaryStorageProvider
    {
        public string Name => BinaryStorageProviderNames.AzureBlob;
        private static readonly object _circuitLock = new();
        private static DateTime _openUntilUtc = DateTime.MinValue;
        private static int _consecutiveFailures = 0;
        private static bool _halfOpenTrialInProgress = false;
        private static readonly TimeSpan[] CircuitOpenDurations =
        {
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(2),
            TimeSpan.FromMinutes(10)
        };
        private const int MaxRetries = 3;

        private static void EnterOrThrow()
        {
            lock (_circuitLock)
            {
                var now = DateTime.UtcNow;
                if (_openUntilUtc == DateTime.MinValue)
                {
                    return;
                }
                if (now < _openUntilUtc)
                {
                    throw new IOException(
                        "AzureBlob is temporarily unavailable (circuit open). "
                            + "Connection or authentication failed recently.");
                }
                if (_halfOpenTrialInProgress)
                {
                    throw new IOException(
                        "AzureBlob is temporarily unavailable (circuit half-open). "
                            + "A trial request is already in progress.");
                }
                _halfOpenTrialInProgress = true;
            }
        }

        private static void OpenCircuit()
        {
            lock (_circuitLock)
            {
                var now = DateTime.UtcNow;
                _halfOpenTrialInProgress = false;
                if (now < _openUntilUtc)
                {
                    return;
                }
                var index = Math.Min(_consecutiveFailures, CircuitOpenDurations.Length - 1);
                _openUntilUtc = now.Add(CircuitOpenDurations[index]);
                _consecutiveFailures++;
            }
        }

        private static void RecordSuccess()
        {
            lock (_circuitLock)
            {
                _consecutiveFailures = 0;
                _openUntilUtc = DateTime.MinValue;
                _halfOpenTrialInProgress = false;
            }
        }

        private static void AbortHalfOpenTrial()
        {
            lock (_circuitLock)
            {
                _halfOpenTrialInProgress = false;
            }
        }

        private static readonly Lazy<BlobContainerClient> _container =
            new(CreateContainerClient, LazyThreadSafetyMode.ExecutionAndPublication);

        private static BlobContainerClient Container => _container.Value;

        private static BlobContainerClient CreateContainerClient()
        {
            var uri = Parameters.BinaryStorage.AzureBlobStorageAccountUri;
            var containerName = Parameters.BinaryStorage.AzureBlobContainerName;
            if (string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(containerName))
            {
                var msg = "AzureBlobStorageAccountUri / AzureBlobContainerName is unconfigured.";
                WriteSysLog(
                    method: nameof(CreateContainerClient),
                    message: msg);
                throw new InvalidOperationException(msg);
            }
            var options = new BlobClientOptions
            {
                Retry =
                {
                    MaxRetries = MaxRetries,
                    Mode = Azure.Core.RetryMode.Fixed,
                    Delay = TimeSpan.FromSeconds(1),
                    NetworkTimeout = TimeSpan.FromSeconds(10)
                }
            };
            var credentialOptions = new DefaultAzureCredentialOptions
            {
                Retry =
                {
                    MaxRetries = MaxRetries,
                    NetworkTimeout = TimeSpan.FromSeconds(10)
                }
            };
            var service = new BlobServiceClient(new Uri(uri), new DefaultAzureCredential(credentialOptions), options);
            return service.GetBlobContainerClient(containerName);
        }

        private static void ValidateObjectName(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new ArgumentException(
                    "objectName is required.",
                    nameof(objectName));
            }
        }

        private static BlobClient BeginOperation(string objectName)
        {
            ValidateObjectName(objectName: objectName);
            EnterOrThrow();
            try
            {
                return Container.GetBlobClient(objectName);
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "BeginOperation", objectName);
                throw;
            }
        }

        private static void ThrowAsIO(
        AuthenticationFailedException ex,
        string operation,
        string objectName)
        {
            OpenCircuit();
            var message = $"AzureBlob {operation} failed. "
                + $"Blob={objectName} Message={ex.Message}";
            WriteSysLog(
                method: nameof(ThrowAsIO),
                message: message,
                errStackTrace: ex.StackTrace);
            throw new IOException(message, ex);
        }

        private static void ThrowAsIO(
        RequestFailedException ex,
        string operation,
        string objectName)
        {
            OpenCircuit();
            var message = $"AzureBlob {operation} failed. "
                + $"Blob={objectName} Status={ex.Status} ErrorCode={ex.ErrorCode} Message={ex.Message}";
            WriteSysLog(
                method: nameof(ThrowAsIO),
                message: message,
                errStackTrace: ex.StackTrace);
            throw new IOException(message, ex);
        }

        private static void ThrowAsNotFound(
        RequestFailedException ex,
        string operation,
        string objectName)
        {
            RecordSuccess();
            throw new IOException(
                $"AzureBlob {operation} failed. " +
                $"Blob={objectName} Status={ex.Status} ErrorCode={ex.ErrorCode} Message={ex.Message}",
                ex);
        }

        private static void ThrowAsIO(
        Exception ex,
        string operation,
        string objectName)
        {
            AbortHalfOpenTrial();
            throw new IOException(
                $"AzureBlob {operation} failed. " +
                $"Blob={objectName} Message={ex.Message}",
                ex);
        }

        private static void WriteSysLog(
            string method,
            string message,
            string errStackTrace = null)
        {
            try
            {
                var context = new Context(
                    sessionStatus: false,
                    sessionData: false,
                    item: false,
                    setPermissions: false);
                new SysLogModel(
                    context: context,
                    method: method,
                    message: message,
                    errStackTrace: errStackTrace,
                    sysLogType: SysLogModel.SysLogTypes.Exception);
            }
            catch
            {
            }
        }

        public void Upload(string objectName, byte[] data, string contentType)
            => UploadAsync(objectName, data, contentType).GetAwaiter().GetResult();

        public void Upload(string objectName, Stream stream, string contentType)
            => UploadAsync(objectName, stream, contentType).GetAwaiter().GetResult();

        public byte[] Download(string objectName)
            => DownloadAsync(objectName).GetAwaiter().GetResult();

        public Stream OpenRead(string objectName)
            => OpenReadAsync(objectName).GetAwaiter().GetResult();

        public void Delete(string objectName)
            => DeleteAsync(objectName).GetAwaiter().GetResult();

        public bool Exists(string objectName)
            => ExistsAsync(objectName).GetAwaiter().GetResult();

        public DateTime LastWriteTime(string objectName)
            => LastWriteTimeAsync(objectName).GetAwaiter().GetResult();

        public async Task UploadAsync(string objectName, byte[] data, string contentType)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                using var memoryStream = new MemoryStream(data);
                await blob.UploadAsync(
                    content: memoryStream,
                    options: new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                    });
                RecordSuccess();
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "Upload", objectName);
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "Upload", objectName);
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "Upload", objectName);
            }
        }

        public async Task UploadAsync(string objectName, Stream stream, string contentType)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                await blob.UploadAsync(
                    content: stream,
                    options: new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                    });
                RecordSuccess();
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "Upload", objectName);
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "Upload", objectName);
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "Upload", objectName);
            }
        }

        public async Task<byte[]> DownloadAsync(string objectName)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                var response = await blob.DownloadContentAsync();
                RecordSuccess();
                return response.Value.Content.ToArray();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                RecordSuccess();
                return null;
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "Download", objectName);
                throw;
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "Download", objectName);
                throw;
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "Download", objectName);
                throw;
            }
        }

        public async Task<Stream> OpenReadAsync(string objectName)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                var stream = await blob.OpenReadAsync();
                AbortHalfOpenTrial();
                return new ReadStream(
                    inner: stream,
                    objectName: objectName);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                RecordSuccess();
                return null;
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "OpenRead", objectName);
                throw;
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "OpenRead", objectName);
                throw;
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "OpenRead", objectName);
                throw;
            }
        }

        public async Task DeleteAsync(string objectName)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                await blob.DeleteIfExistsAsync();
                RecordSuccess();
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                RecordSuccess();
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "Delete", objectName);
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "Delete", objectName);
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "Delete", objectName);
            }
        }

        public async Task<bool> ExistsAsync(string objectName)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                var response = await blob.ExistsAsync();
                RecordSuccess();
                return response.Value;
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "Exists", objectName);
                throw;
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "Exists", objectName);
                throw;
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "Exists", objectName);
                throw;
            }
        }

        public async Task<DateTime> LastWriteTimeAsync(string objectName)
        {
            var blob = BeginOperation(objectName: objectName);
            try
            {
                var props = await blob.GetPropertiesAsync();
                RecordSuccess();
                return props.Value.LastModified.UtcDateTime;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                RecordSuccess();
                return DateTime.FromOADate(0);
            }
            catch (RequestFailedException ex)
            {
                ThrowAsIO(ex, "LastWriteTime", objectName);
                throw;
            }
            catch (AuthenticationFailedException ex)
            {
                ThrowAsIO(ex, "LastWriteTime", objectName);
                throw;
            }
            catch (Exception ex)
            {
                ThrowAsIO(ex, "LastWriteTime", objectName);
                throw;
            }
        }

        private sealed class ReadStream : Stream
        {
            private readonly Stream _inner;
            private readonly string _objectName;
            private bool _completed;

            public ReadStream(Stream inner, string objectName)
            {
                _inner = inner;
                _objectName = objectName;
            }

            public override bool CanRead => _inner.CanRead;

            public override bool CanSeek => _inner.CanSeek;

            public override bool CanWrite => false;

            public override long Length => _inner.Length;

            public override long Position
            {
                get => _inner.Position;
                set => _inner.Position = value;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                try
                {
                    var read = _inner.Read(buffer, offset, count);
                    Complete(read: read);
                    return read;
                }
                catch (Exception ex)
                {
                    ThrowAsReadFailure(ex: ex);
                    throw;
                }
            }

            public override async Task<int> ReadAsync(
                byte[] buffer,
                int offset,
                int count,
                CancellationToken cancellationToken)
            {
                try
                {
                    var read = await _inner.ReadAsync(buffer, offset, count, cancellationToken);
                    Complete(read: read);
                    return read;
                }
                catch (Exception ex)
                {
                    ThrowAsReadFailure(ex: ex);
                    throw;
                }
            }

            public override async ValueTask<int> ReadAsync(
                Memory<byte> buffer,
                CancellationToken cancellationToken = default)
            {
                try
                {
                    var read = await _inner.ReadAsync(buffer, cancellationToken);
                    Complete(read: read);
                    return read;
                }
                catch (Exception ex)
                {
                    ThrowAsReadFailure(ex: ex);
                    throw;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _inner.Seek(offset, origin);
            }

            public override void Flush()
            {
                _inner.Flush();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _inner.Dispose();
                }
                base.Dispose(disposing);
            }

            public override async ValueTask DisposeAsync()
            {
                await _inner.DisposeAsync();
                await base.DisposeAsync();
            }

            private void Complete(int read)
            {
                if (read == 0 && !_completed)
                {
                    _completed = true;
                    RecordSuccess();
                }
            }

            private void ThrowAsReadFailure(Exception ex)
            {
                switch (ex)
                {
                    case RequestFailedException requestFailed when requestFailed.Status == 404:
                        ThrowAsNotFound(requestFailed, "OpenRead", _objectName);
                        break;
                    case RequestFailedException requestFailed:
                        ThrowAsIO(requestFailed, "OpenRead", _objectName);
                        break;
                    case AuthenticationFailedException authenticationFailed:
                        ThrowAsIO(authenticationFailed, "OpenRead", _objectName);
                        break;
                    case OperationCanceledException:
                        AbortHalfOpenTrial();
                        break;
                    default:
                        ThrowAsIO(ex, "OpenRead", _objectName);
                        break;
                }
            }
        }
    }
}
