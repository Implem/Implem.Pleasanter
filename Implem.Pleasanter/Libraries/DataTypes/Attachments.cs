using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Attachments : List<Attachment>, IConvertable
    {
        public Attachments()
        {
        }

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return this.ToJson();
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return string.Empty;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return string.Empty;
        }

        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return string.Empty;
            }
        }

        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            return hb.Td(
                css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                action: () => hb
                    .Ol(action: () => ForEach(item => hb
                        .Li(action: () => hb
                            .A(
                                href: Locations.DownloadFile(
                                    context: context,
                                    guid: item.Guid),
                                action: () => hb
                                    .Text(text: item.Name))))));
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn)
        {
            return string.Empty;
        }

        public bool InitialValue(Context context)
        {
            return this?.Any() != true;
        }

        public string RecordingJson()
        {
            var attachments = new Attachments();
            this
                .Where(o => o.Deleted != true)
                .ForEach(item => attachments.Add(new Attachment()
                {
                    Guid = item.Guid,
                    Name = item.Name,
                    Size = item.Size,
                    HashCode = item.HashCode
                }));
            return attachments.ToJson();
        }

        public void Write(Context context, List<SqlStatement> statements, long referenceId, Column column)
        {
            this
                .Where(o => !o.Guid.IsNullOrEmpty())
                .ForEach(attachment =>
                {
                    if (attachment.Added == true)
                    {
                        if (BinaryUtilities.BinaryStorageProvider(column, (decimal)attachment.Size) == "LocalFolder")
                        {
                            attachment.WriteToLocal(context: context);
                        }
                    }
                    else if(attachment.Deleted == true && !attachment.Overwritten.HasValue)
                    {
                        attachment.DeleteFromLocal(context: context);
                    }
                    attachment.SqlStatement(
                        context: context,
                        statements: statements,
                        referenceId: referenceId,
                        column: column);
                });
        }
    }
}