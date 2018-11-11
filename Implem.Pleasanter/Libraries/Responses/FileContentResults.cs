using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class FileContentResults
    {
        public static FileContentResult Download(Context context, string guid)
        {
            var dataRow = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn()
                        .Guid()
                        .BinaryType()
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
                        .TenantId(context.TenantId)
                        .Guid(guid)
                        .CanRead(
                            context: context,
                            idColumnBracket: "[Binaries].[ReferenceId]")))
                                .AsEnumerable()
                                .FirstOrDefault();
            return dataRow != null
                ? new ResponseFile(
                    new MemoryStream(Bytes(dataRow), false),
                    dataRow.String("FileName"),
                    dataRow.String("ContentType")).FileStream()
                : null;
        }

        private static byte[] Bytes(DataRow dataRow)
        {
            switch (Parameters.BinaryStorage.Provider)
            {
                case "Local":
                    return Implem.Libraries.Utilities.Files.Bytes(
                        Path.Combine(Directories.BinaryStorage(),
                        dataRow.String("BinaryType"),
                        dataRow.String("Guid")));
                default:
                    return dataRow.Bytes("Bin");
            }
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