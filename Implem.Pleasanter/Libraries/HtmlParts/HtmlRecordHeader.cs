using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordHeader
    {
        public static HtmlBuilder RecordHeader(
            this HtmlBuilder hb,
            Permissions.Types permissionType,
            BaseModel baseModel,
            string tableName,
            bool switcher = true)
        {
            return baseModel.AccessStatus == Databases.AccessStatuses.Selected
                ? hb.Div(id: "RecordHeader", action: () => hb
                    .Div(id: "RecordInfo", action: () => hb
                        .RecordInfo(baseModel: baseModel, tableName: tableName))
                    .Div(id: "RecordSwitchers", action: () => hb
                        .RecordSwitchers(switcher: switcher)))
                    .Notes(permissionType: permissionType, verType: baseModel.VerType)
                : hb;
        }
    }
}