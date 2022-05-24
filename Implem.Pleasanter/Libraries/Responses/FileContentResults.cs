﻿using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.IO;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class FileContentResults
    {
        public static ResponseFile Download(Context context, string guid)
        {
            var dataRow = GetBinariesTable(context, guid);
            switch (dataRow.String("BinaryType"))
            {
                case "Images":
                    if (dataRow == null)
                    {
                        return null;
                    }
                    return Bytes(
                        dataRow: dataRow,
                        thumbnail: context.QueryStrings.Bool("thumbnail"));
                default:
                    if (dataRow != null)
                    {
                        return Bytes(dataRow: dataRow);
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        public static ContentResultInheritance DownloadByApi(Context context, string guid)
        {
            DataRow dataRow = GetBinariesTable(context, guid);
            return dataRow != null
                ? Bytes(dataRow).ToContentResult(
                    id: dataRow.Long("BinaryId"),
                    referenceId: dataRow.Long("ReferenceId"),
                    binaryType: dataRow.String("BinaryType"),
                    guid: dataRow.String("Guid"),
                    extension: dataRow.String("Extension"),
                    size: dataRow.Long("Size"),
                    creator: dataRow.Long("Creator"),
                    updator: dataRow.Long("Updator"),
                    createdTime: dataRow.String("CreatedTime"),
                    updatedTime: dataRow.String("UpdatedTime"))
                : ApiResults.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
        }

        public static long GetReferenceId(Context context, string guid)
        {
            var referenceId = Rds.ExecuteScalar_long(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectBinaries(
                        column: Rds.BinariesColumn()
                            .ReferenceId(),
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
                                idColumnBracket: "\"Binaries\".\"ReferenceId\"",
                                _using: !context.Publish))
                });
            return referenceId;
        }

        public static Attachment LinkBinary(Context context, Attachment attachment, Column column)
        {
            DataRow dataRow = GetBinariesTable(
                context: context,
                guid: attachment.Guid);
            if (dataRow != null)
            {
                attachment.ReferenceId = dataRow.Long("ReferenceId");
                attachment.Name = dataRow.String("FileName");
                attachment.Size = dataRow.Long("Size");
                attachment.SetHashCode(column, dataRow.Bytes("Bin"));
            }
            return attachment;
        }

        private static DataRow GetBinariesTable(Context context, string guid)
        {
            if (guid.IsNullOrEmpty()) return null;
            return Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectBinaries(
                        column: Rds.BinariesColumn()
                            .BinaryId()
                            .ReferenceId()
                            .Guid()
                            .BinaryType()
                            .Bin()
                            .Thumbnail()
                            .FileName()
                            .ContentType()
                            .Extension()
                            .Size()
                            .Creator()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        join: Rds.BinariesJoinDefault()
                            .Add(new SqlJoin(
                                tableBracket: "\"Items\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"Binaries\".\"ReferenceId\"=\"Items\".\"ReferenceId\""))
                            .Add(new SqlJoin(
                                tableBracket: "\"Sites\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\"")),
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .Guid(guid)
                            .CanRead(
                                context: context,
                                idColumnBracket: "\"Binaries\".\"ReferenceId\"",
                                _using: !context.Publish)),
                    Rds.SelectBinaries(
                        column: Rds.BinariesColumn()
                            .BinaryId()
                            .ReferenceId()
                            .Guid()
                            .BinaryType()
                            .Bin()
                            .Thumbnail()
                            .FileName()
                            .ContentType()
                            .Extension()
                            .Size()
                            .Creator()
                            .Updator()
                            .CreatedTime()
                            .UpdatedTime(),
                        join: Rds.BinariesJoinDefault()
                            .Add(new SqlJoin(
                                tableBracket: "\"Items\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"Binaries\".\"ReferenceId\"=\"Items\".\"ReferenceId\""))
                            .Add(new SqlJoin(
                                tableBracket: "\"Sites\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"Items\".\"SiteId\"=\"Sites\".\"SiteId\"")),
                        where: Rds.BinariesWhere()
                            .TenantId(context.TenantId)
                            .Guid(guid)
                            .Add(raw: $"(\"Binaries\".\"CreatedTime\"=\"Binaries\".\"UpdatedTime\" and \"Binaries\".\"Creator\"={context.UserId})"),
                        unionType: Sqls.UnionTypes.UnionAll)})
                            .AsEnumerable()
                            .FirstOrDefault();
        }

        private static ResponseFile Bytes(DataRow dataRow, bool thumbnail = false)
        {
            var isThumbnail = thumbnail && dataRow["Thumbnail"] != DBNull.Value;
            var contentType = dataRow.String("ContentType");
            if (isThumbnail)
            {
                contentType = contentType.IsNullOrEmpty()
                    ? "image/bmp"
                    : "image/png";
            }
            var bin = isThumbnail
                ? dataRow.Bytes("Thumbnail")
                : dataRow.Bytes("Bin");
            if (bin != null)
            {
                return new ResponseFile(
                    fileContent: new MemoryStream(bin, false),
                    fileDownloadName: dataRow.String("FileName"),
                    contentType: contentType);
            }
            else
            {
                return new ResponseFile(
                    fileContent: new FileInfo(
                        Path.Combine(Directories.BinaryStorage(),
                            dataRow.String("BinaryType"),
                            dataRow.String("Guid"))),
                        fileDownloadName: dataRow.String("FileName"),
                    contentType: contentType);
            }
        }

        public static ResponseFile DownloadTemp(string guid)
        {
            var folderPath = Path.Combine(Path.Combine(Directories.Temp(), guid));
            var files = Directory.GetFiles(folderPath);
            return new ResponseFile(new FileInfo(files[0]), Path.GetFileName(files[0]));
        }

        public static FileContentResult ToFileContentResult(this FileContentResult content)
        {
            return new FileContentResult(content.FileContents, content.ContentType)
            {
                FileDownloadName = content.FileDownloadName
            };
        }

        public static FileStreamResult FileStreamResult(ResponseFile file)
        {
            if (file == null)
            {
                return null;
            }
            else if (file.IsFileInfo())
            {
                return new FileStreamResult(System.IO.File.OpenRead(file.FileInfo.FullName), file.ContentType)
                {
                    FileDownloadName = file.FileDownloadName
                };
            }
            else
            {
                return new FileStreamResult(file.FileContentsStream, file.ContentType)
                {
                    FileDownloadName = file.FileDownloadName
                };
            }
        }
    }
}