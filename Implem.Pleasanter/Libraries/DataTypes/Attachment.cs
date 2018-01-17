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

        public void SqlStatement(List<SqlStatement> statements, long referenceId)
        {
            if (Added == true)
            {
                var bin = Files.Bytes(Path.Combine(Directories.Temp(), Guid, Name));
                if (bin != null)
                {
                    statements.Add(Rds.InsertBinaries(
                        selectIdentity: true,
                        param: Rds.BinariesParam()
                            .TenantId(Sessions.TenantId())
                            .ReferenceId(referenceId, _using: referenceId != 0)
                            .ReferenceId(raw: Def.Sql.Identity, _using: referenceId == 0)
                            .Guid(Guid)
                            .Title(Name)
                            .BinaryType("Attachments")
                            .Bin(bin)
                            .FileName(Name)
                            .Extension(Extention)
                            .Size(Size)
                            .ContentType(ContentType)));
                }
                Directory.Delete(Path.Combine(Directories.Temp(), Guid), true);
            }
            if (Deleted == true)
            {
                statements.Add(Rds.DeleteBinaries(
                    where: Rds.BinariesWhere().Guid(Guid)));
            }
        }
    }
}