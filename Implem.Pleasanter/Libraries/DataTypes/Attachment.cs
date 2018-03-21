using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Server;
using System.Collections.Generic;
using System.IO;
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

        public string DisplaySize()
        {
            string strSize = "0" + unit[0];
            var size = Size;
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
                            : size?.ToString("#.#0") + unit[index];
                        break;
                    }
                }
            }
            return strSize;
        }

        public void WriteToLocal()
        {
            if (Added == true)
            {
                GetBin().Write(Path.Combine(
                    Directories.BinaryStorage(),
                    "Attachments",
                    Guid));
            }
            else if (Deleted == true)
            {
                Files.DeleteFile(Path.Combine(
                    Directories.BinaryStorage(),
                    "Attachments",
                    Guid));
            }
        }

        public void SqlStatement(List<SqlStatement> statements, long referenceId)
        {
            if (Added == true)
            {
                var bin = GetBin();
                if (bin != null)
                {
                    statements.Add(Rds.InsertBinaries(
                        param: Rds.BinariesParam()
                            .TenantId(Sessions.TenantId())
                            .ReferenceId(referenceId, _using: referenceId != 0)
                            .ReferenceId(raw: Def.Sql.Identity, _using: referenceId == 0)
                            .Guid(Guid)
                            .Title(Name)
                            .BinaryType("Attachments")
                            .Bin(bin, _using: !Parameters.BinaryStorage.IsLocal())
                            .FileName(Name)
                            .Extension(Extention)
                            .Size(Size)
                            .ContentType(ContentType)));
                }
                Directory.Delete(Path.Combine(Directories.Temp(), Guid), recursive: true);
            }
            else if (Deleted == true)
            {
                statements.Add(Rds.DeleteBinaries(
                    where: Rds.BinariesWhere().Guid(Guid)));
            }
        }

        private byte[] GetBin()
        {
            return Files.Bytes(Path.Combine(Directories.Temp(), Guid, Name));
        }
    }
}