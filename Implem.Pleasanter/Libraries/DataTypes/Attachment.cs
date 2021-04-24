using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Attachment
    {
        private string[] unit = new string[] { " B", " KB", " MB", " GB" };
        public string Guid;
        public string Name;
        public long? Size;
        public string Extention;
        public string ContentType;
        public bool? Added;
        public bool? Deleted;
        public string Base64;
        public string FileName;
        public string Base64Binary;
        public string HashCode;

        public Attachment()
        {
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            if (!Base64.IsNullOrEmpty() || !Base64Binary.IsNullOrEmpty())
            {
                var bin = GetBin();
                Guid = Strings.NewGuid();
                Size = bin.Length;
                Extention = Path.GetExtension(Name ?? FileName);
                ContentType = Strings.CoalesceEmpty(ContentType, "text/plain");
                Added = true;
                Files.Write(bin, Path.Combine(Directories.Temp(), Guid, Name ?? FileName));
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

        public void WriteToLocal(Context context)
        {
            if (Added == true)
            {
                new FileInfo(Path.Combine(Directories.Temp(), Guid, Name ?? FileName))
                   .CopyTo(Path.Combine(
                       Directories.BinaryStorage(),
                       "Attachments",
                       Guid), overwrite: true);
            }
            else if (Deleted == true && !Parameters.BinaryStorage.RestoreLocalFiles)
            {
                Files.DeleteFile(Path.Combine(
                    Directories.BinaryStorage(),
                    "Attachments",
                    Guid));
            }
        }

        public void SqlStatement(Context context, List<SqlStatement> statements, long referenceId, Column column)
        {
            if (Added == true)
            {
                var bin = IsStoreLocalFolder(column) ? default : GetBin();
                statements.Add(Rds.UpdateOrInsertBinaries(
                    param: Rds.BinariesParam()
                        .TenantId(context.TenantId)
                        .ReferenceId(referenceId, _using: referenceId != 0)
                        .ReferenceId(raw: Def.Sql.Identity, _using: referenceId == 0)
                        .Guid(Guid)
                        .Title(Name ?? FileName)
                        .BinaryType("Attachments")
                        .Bin(bin, _using: !IsStoreLocalFolder(column))
                        .FileName(Name ?? FileName)
                        .Extension(Extention)
                        .Size(Size)
                        .ContentType(ContentType),
                    where: Rds.BinariesWhere().Guid(Guid)));
                DataSources.File.DeleteTemp(Guid);
            }
            else if (Deleted == true)
            {
                statements.Add(Rds.DeleteBinaries(
                    factory: context,
                    where: Rds.BinariesWhere().Guid(Guid)));
            }
        }

        public void SetHashCode(Column column)
        {
            if (IsStoreLocalFolder(column))
            {
                var filename = Path.Combine(Directories.Temp(), Guid, Name ?? FileName);
                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    var sha = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                    HashCode = System.Convert.ToBase64String(sha.ComputeHash(stream));
                }
            }
            else
            {
                var bytes = GetBin();
                var sha = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                HashCode = System.Convert.ToBase64String(sha.ComputeHash(bytes));
            }
        }

        private byte[] GetBin()
        {
            var bin = Base64 ?? Base64Binary;
            return bin.IsNullOrEmpty()
                ? Files.Bytes(Path.Combine(Directories.Temp(), Guid, Name ?? FileName))
                : System.Convert.FromBase64String(bin);
        }

        public bool IsStoreLocalFolder(Column column)
        {
            return Pleasanter.Models.BinaryUtilities.BinaryStorageProvider(column, Size.GetValueOrDefault()) == "LocalFolder";
        }
    }
}