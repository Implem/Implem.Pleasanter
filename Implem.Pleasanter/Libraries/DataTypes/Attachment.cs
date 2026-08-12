using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataSources.BinaryStorages;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Attachment
    {
        private string[] unit = new string[] { " B", " KB", " MB", " GB" };
        public string Guid;
        public string Name;
        public long? ReferenceId;
        public long? Size;
        public string Extension;
        public string ContentType;
        public bool? Added;
        public bool? Deleted;
        public string Base64;
        public string Action;
        public string FileName;
        public string Base64Binary;
        public string HashCode;
        public bool? Overwritten;

        public Attachment()
        {
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            if (Guid is not null && Deleted == true)
                return;

            if (Base64 is null && Base64Binary is null)
                return;

            var bin = GetBin();
            Guid = Strings.NewGuid();
            Size = bin.Length;
            Extension = Path.GetExtension(Name ?? FileName);
            ContentType = Strings.CoalesceEmpty(ContentType, "text/plain");
            Added = true;
            if (Files.ValidateFileName(Name ?? FileName))
            {
                Files.Write(bin, Path.Combine(Directories.Temp(), Guid, Name ?? FileName));
            }
            else
            {
                var context = new Context(
                    sessionStatus: false,
                    sessionData: false,
                    item: false,
                    setPermissions: false);
                new SysLogModel(
                    context: context,
                    method: $"{nameof(Attachment)}.{nameof(OnDeserialized)}",
                    message: $"Invalid File Name: '{Name ?? FileName}'",
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
        }

        public string DisplaySize()
        {
            string strSize = "0" + unit[0];
            var size = Size?.ToDecimal() ?? 0;
            if (size != 0)
            {
                for (int index = 0; index < unit.Length; index++)
                {
                    if (size > 1024)
                    {
                        size = size / 1024;
                    }
                    else
                    {
                        strSize = (index == 0)
                            ? size.ToString() + unit[index]
                            : size.ToString("#.#0") + unit[index];
                        break;
                    }
                }
            }
            return strSize;
        }

        public void WriteToLocal(Context context, Column column = null)
        {
            var providerName = BinaryUtilities.BinaryStorageProvider(
                column: column,
                size: Size.GetValueOrDefault());
            var provider = BinaryStorageProviderFactory.Create(providerName);
            if (provider == null) return;  // DB 保存経路（ファイル I/O 不要）
            var tempPath = Path.Combine(Directories.Temp(), Guid, Name ?? FileName);
            try
            {
                using var stream = System.IO.File.OpenRead(tempPath);
                provider.Upload(
                    objectName: $"Attachments/{Guid}",
                    stream: stream,
                    contentType: ContentType);
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(WriteToLocal),
                    message: $"Upload failed before commit. Blob=Attachments/{Guid} Error={e.Message}",
                    errStackTrace: e.StackTrace,
                    sysLogType: SysLogModel.SysLogTypes.Exception);
                throw;
            }
        }

        public void DeleteFromLocal(
            Context context,
            SiteSettings ss,
            Column column,
            long referenceId,
            bool verUp)
        {
            if (column?.NotDeleteExistHistory != true
                || (verUp == false
                    && !ExistsHistory(
                        context: context,
                        ss: ss,
                        column: column,
                        referenceId: referenceId)))
            {
                var providerName = BinaryUtilities.BinaryStorageProvider(
                    column: column,
                    size: Size.GetValueOrDefault());
                var provider = BinaryStorageProviderFactory.Create(providerName);
                if (provider == null) return;  // DB 保存経路（ファイル I/O 不要）
                provider.Delete(
                    objectName: $"Attachments/{Guid}");
            }
        }

        public void SqlStatement(
            Context context,
            SiteSettings ss,
            Column column,
            List<SqlStatement> statements,
            long referenceId,
            bool verUp)
        {
            if (Added == true)
            {
                var providerName = BinaryUtilities.BinaryStorageProvider(
                    column: column,
                    size: Size.GetValueOrDefault());
                var provider = BinaryStorageProviderFactory.Create(providerName);
                if (Parameters.BinaryStorage.TemporaryBinaryStorageProvider == ParameterAccessor.Parts.BinaryStorageProviderNames.Rds
                    && context.Api != true)
                {
                    if (provider != null)
                    {
                        MoveBinFromRdsToExternal(
                            context: context,
                            provider: provider);
                    }
                    statements.Add(Rds.UpdateBinaries(
                        param: Rds.BinariesParam()
                            .TenantId(context.TenantId)
                            .ReferenceId(referenceId, _using: referenceId != 0)
                            .ReferenceId(raw: Def.Sql.Identity, _using: referenceId == 0)
                            .Guid(Guid)
                            .Title(Name ?? FileName)
                            .BinaryType("Attachments")
                            .Bin(raw: "NULL", _using: provider != null)
                            .FileName(Name ?? FileName)
                            .Extension(Extension)
                            .Size(Size)
                            .ContentType(ContentType),
                        where: Rds.BinariesWhere().Guid(Guid)));
                }
                else
                {
                    var bin = provider != null ? default : GetBin(context);
                    statements.Add(Rds.UpdateOrInsertBinaries(
                        param: Rds.BinariesParam()
                            .TenantId(context.TenantId)
                            .ReferenceId(referenceId, _using: referenceId != 0)
                            .ReferenceId(raw: Def.Sql.Identity, _using: referenceId == 0)
                            .Guid(Guid)
                            .Title(Name ?? FileName)
                            .BinaryType("Attachments")
                            .Bin(bin, _using: provider == null)
                            .Bin(raw: "NULL", _using: provider != null)
                            .FileName(Name ?? FileName)
                            .Extension(Extension)
                            .Size(Size)
                            .ContentType(ContentType),
                        where: Rds.BinariesWhere().Guid(Guid)));
                }
            }
            else if (Deleted == true && !Overwritten.HasValue)
            {
                if (column?.NotDeleteExistHistory != true
                    || (verUp == false
                        && !ExistsHistory(
                            context: context,
                            ss: ss,
                            column: column,
                            referenceId: referenceId)))
                {
                    statements.Add(Rds.DeleteBinaries(
                        factory: context,
                        where: Rds.BinariesWhere().Guid(Guid)));
                }
            }
            else
            {
                switch (Action)
                {
                    case "Linked":
                        statements.Add(Rds.UpdateBinaries(
                            param: Rds.BinariesParam()
                                .ReferenceId(referenceId),
                            where: Rds.BinariesWhere().Guid(Guid)));
                        break;
                    default:
                        break;
                }
            }
        }

        private void MoveBinFromRdsToExternal(
            Context context,
            IBinaryStorageProvider provider)
        {
            var dataRow = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn().Bin(),
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .Guid(Guid)
                        .BinaryType("Temporary")))
                .AsEnumerable()
                .FirstOrDefault();
            if (dataRow == null) return;
            var bin = dataRow.Bytes("Bin");
            if (bin == null) return;
            try
            {
                provider.Upload(
                    objectName: $"Attachments/{Guid}",
                    data: bin,
                    contentType: ContentType);
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(MoveBinFromRdsToExternal),
                    message: $"Temp->External upload failed. Blob=Attachments/{Guid} Error={e.Message}",
                    errStackTrace: e.StackTrace,
                    sysLogType: SysLogModel.SysLogTypes.Exception);
                throw;
            }
        }

        private bool ExistsHistory(
            Context context,
            SiteSettings ss,
            Column column,
            long referenceId)
        {
            if (ss == null)
            {
                return false;
            }
            switch (ss.ReferenceType)
            {
                case "Issues":
                    return Rds.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectIssues(
                            tableType: Sqls.TableTypes.History,
                            column: Rds.IssuesColumn().IssuesCount(),
                            where: Rds.IssuesWhere()
                                .SiteId(ss.SiteId)
                                .IssueId(referenceId)
                                .Add(
                                    tableName: "Issues",
                                    columnBrackets: $"\"{column.ColumnName}\"".ToSingleArray(),
                                    value: $"%\"Guid\":\"{Guid}\"%",
                                    name: Strings.NewGuid(),
                                    _operator: context.Sqls.LikeWithEscape))) >= 1;
                case "Results":
                    return Rds.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectResults(
                            tableType: Sqls.TableTypes.History,
                            column: Rds.ResultsColumn().ResultsCount(),
                            where: Rds.ResultsWhere()
                                .SiteId(ss.SiteId)
                                .ResultId(referenceId)
                                .Add(
                                    tableName: "Results",
                                    columnBrackets: $"\"{column.ColumnName}\"".ToSingleArray(),
                                    value: $"%\"Guid\":\"{Guid}\"%",
                                    name: Strings.NewGuid(),
                                    _operator: context.Sqls.LikeWithEscape))) >= 1;
                default:
                    return false;
            }
        }

        public ContentResultInheritance Create(Context context)
        {
            if (!context.ContractSettings.Attachments())
            {
                return null;
            }
            var invalid = BinaryValidators.OnUploading(
                context: context,
                attachments: new Attachments() { this });
            if (invalid != Error.Types.None)
            {
                return ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: invalid));
            }
            var providerName = BinaryUtilities.BinaryStorageProvider(
                column: null,
                size: Size.GetValueOrDefault());
            var provider = BinaryStorageProviderFactory.Create(providerName);
            if (provider != null)
            {
                WriteToLocal(context: context, column: null);
            }
            var bin = provider != null
                ? default
                : GetBin(context: context);
            var statements = new List<SqlStatement>();
            statements.Add(Rds.InsertBinaries(
                selectIdentity: true,
                param: Rds.BinariesParam()
                    .TenantId(context.TenantId)
                    .ReferenceId(ReferenceId)
                    .Guid(Guid)
                    .Title(Name ?? FileName)
                    .BinaryType("Attachments")
                    .Bin(bin, _using: provider == null)
                    .Bin(raw: "NULL", _using: provider != null)
                    .FileName(Name ?? FileName)
                    .Extension(Extension)
                    .Size(Size)
                    .ContentType(ContentType)));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            return ApiResults.Success(
                id: response.Id.ToLong(),
                message: this.Guid);
        }

        public void SetHashCode(
            Context context,
            Column column,
            byte[] bin = null)
        {
            static string GetAlgorithmParam()
            {
                switch (Parameters.Rds.Dbms)
                {
                    case "SQLServer":
                        return "sha2_256";
                    case "PostgreSQL":
                        return "sha256";
                    case "MySQL":
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
            var providerName = BinaryUtilities.BinaryStorageProvider(
                column: column,
                size: Size.GetValueOrDefault());
            var provider = BinaryStorageProviderFactory.Create(providerName);
            if (provider != null)
            {
                var tempFile = Path.Combine(
                    Directories.Temp(),
                    Guid,
                    Name ?? FileName);
                using var sha = System.Security.Cryptography.SHA256.Create();
                if (System.IO.File.Exists(tempFile))
                {
                    using var stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
                    HashCode = System.Convert.ToBase64String(sha.ComputeHash(stream));
                }
                else
                {
                    using var stream = provider.OpenRead($"Attachments/{Guid}");
                    HashCode = System.Convert.ToBase64String(sha.ComputeHash(stream));
                }
            }
            else
            {
                if (Parameters.BinaryStorage.TemporaryBinaryStorageProvider == ParameterAccessor.Parts.BinaryStorageProviderNames.Rds
                    && context.Api != true)
                {
                    var hash = Repository.ExecuteScalar_bytes(
                        context: context,
                        statements: new SqlStatement(
                            commandText: context.Sqls.GetBinaryHash(algorithm: "sha256"),
                            param: new SqlParamCollection{
                                { "Algorithm", GetAlgorithmParam() },
                                { "Guid", Guid }
                            }));
                    HashCode = System.Convert.ToBase64String(hash);
                }
                else
                {
                    var bytes = GetBin(context) ?? bin;
                    var sha = System.Security.Cryptography.SHA256.Create();
                    HashCode = System.Convert.ToBase64String(sha.ComputeHash(bytes));
                }
            }
        }

        public byte[] GetBin(Context context = null)
        {
            var bin = Base64 ?? Base64Binary;
            if(bin is null)
                return Files.Bytes(Path.Combine(Directories.Temp(), Guid, Name ?? FileName));
            return System.Convert.FromBase64String(bin);
        }

        public bool IsStoreLocalFolder(Column column)
        {
            return BinaryUtilities.BinaryStorageProvider(column, Size.GetValueOrDefault()) == ParameterAccessor.Parts.BinaryStorageProviderNames.LocalFolder;
        }

        public bool Exists(Context context)
        {
            var localPath = Path.Combine(Directories.BinaryStorage(), "Attachments", Guid);
            var temp = Path.Combine(Directories.Temp(), Guid, Name);
            var providerName = BinaryUtilities.BinaryStorageProvider(
                column: null,
                size: Size.GetValueOrDefault());
            var provider = BinaryStorageProviderFactory.Create(providerName);
            if ((provider != null && provider.Exists($"Attachments/{Guid}"))
                || Files.Exists(localPath)
                || Files.Exists(temp))
            {
                return true;
            }
            else
            {
                if (Guid.IsNullOrEmpty()) return false;
                return Repository.ExecuteScalar_int(
                    context: context,
                    transactional: false,
                    statements: Rds.SelectCount(
                        tableName: "Binaries",
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .Guid(Guid)
                            .Add(raw: $"(\"Bin\" is not null)"))) == 1;
            }
        }

        internal void AttachmentAction(Context context, Column column, Attachments oldAttachments)
        {
            switch (Action)
            {
                case "Linked":
                    FileContentResults.LinkBinary(
                        context: context,
                        attachment: this,
                        column: column);
                    BinaryUtilities.OverwriteSameFileName(
                        attachments: oldAttachments,
                        fileName: Name);
                    break;
                default:
                    break;
            }
        }
    }
}