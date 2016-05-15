using Implem.Libraries.Utilities;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public static class HtmlRecordHeader
    {
        public static HtmlBuilder RecordHeader(
            this HtmlBuilder hb,
            long id,
            BaseModel baseModel,
            string tableName,
            List<long> switchTargets = null,
            bool switcher = true)
        {
            return baseModel.AccessStatus == Databases.AccessStatuses.Selected
                ? hb.Div(css: "record-header", action: () => hb
                    .Div(id: "RecordInfo", css: "record-info", action: () => hb
                        .RecordInfo(baseModel: baseModel, tableName: tableName))
                    .Div(css: "record-switchers", action: () => hb
                        .RecordSwitchers(
                            id: id,
                            switchTargets: switchTargets,
                            switcher: switcher))
                    .Div(
                        id: "RecordHistories",
                        css: "record-histories",
                        action: () => hb
                            .RecordHistories(ver: baseModel.Ver, verType: baseModel.VerType)))
                : hb;
        }
    }
}