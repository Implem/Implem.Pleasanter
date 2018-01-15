using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class FileContentResults
    {
        public static FileContentResult Download(string guid)
        {
            var dataRow = Rds.ExecuteTable(statements:
                Rds.SelectBinaries(
                    column: Rds.BinariesColumn()
                        .Bin()
                        .FileName()
                        .ContentType(),
                    join: Rds.BinariesJoinDefault()
                        .Add(new SqlJoin(
                            tableBracket: "[Items]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Binaries].[ReferenceId]=[Items].[ReferenceId]"))
                        .Add(new SqlJoin(
                            tableBracket: "[Sites]",
                            joinType: SqlJoin.JoinTypes.Inner,
                            joinExpression: "[Items].[SiteId]=[Sites].[SiteId]")),
                    where: Rds.BinariesWhere()
                        .TenantId(Sessions.TenantId())
                        .Guid(guid)
                        .CanRead("[Binaries].[ReferenceId]")))
                            .AsEnumerable()
                            .FirstOrDefault();
            return dataRow != null
                ? new ResponseFile(
                    new MemoryStream(dataRow.Bytes("Bin"), false),
                    dataRow.String("FileName"),
                    dataRow.String("ContentType")).FileStream()
                : null;
        }

        public static FileContentResult DownloadTemp(string guid)
        {
            var folderPath = Path.Combine(Path.Combine(Directories.Temp(), guid));
            var files = Directory.GetFiles(folderPath);
            using (var fs = new FileStream(files[0], FileMode.Open, FileAccess.Read))
            {
                var res = new ResponseFile(fs, Path.GetFileName(files[0]));
                return res.FileStream();
            }
        }
    }
}