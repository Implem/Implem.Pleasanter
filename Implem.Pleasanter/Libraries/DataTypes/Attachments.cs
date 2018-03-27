using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Attachments : List<Attachment>, IConvertable
    {
        public Attachments()
        {
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .Ol(action: () => ForEach(item => hb
                    .Li(action: () => hb
                        .A(
                            href: Locations.DownloadFile(item.Guid),
                            action: () => hb
                                .Text(text: item.Name))))));
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return this.ToJson();
        }

        public string ToExport(Column column, ExportColumn exportColumn)
        {
            return string.Empty;
        }

        public string ToResponse()
        {
            return string.Empty;
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
                    Size = item.Size
                }));
            return attachments.ToJson();
        }

        public void Write(List<SqlStatement> statements, long referenceId)
        {
            ForEach(attachment =>
            {
                if (Parameters.BinaryStorage.IsLocal())
                {
                    attachment.WriteToLocal();
                }
                attachment.SqlStatement(statements, referenceId);
            });
        }

        public bool InitialValue()
        {
            return this?.Any() != true;
        }
    }
}