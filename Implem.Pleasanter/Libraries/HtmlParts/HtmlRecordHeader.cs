using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordHeader
    {
        public static HtmlBuilder RecordHeader(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            BaseModel baseModel,
            string tableName,
            bool switcher = true)
        {
            return baseModel.AccessStatus == Databases.AccessStatuses.Selected
                ? hb.Div(id: "RecordHeader", action: () => hb
                    .Div(id: "RecordInfo", action: () => hb
                        .RecordInfo(
                            context: context,
                            baseModel: baseModel,
                            tableName: tableName))
                    .Div(id: "RecordSwitchers", action: () => hb
                        .RecordSwitchers(ss: ss, switcher: switcher)))
                    .Notes(
                        context: context,
                        ss: ss,
                        verType: baseModel.VerType)
                : hb;
        }
    }
}