using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordInfo
    {
        public static HtmlBuilder RecordInfo(
            this HtmlBuilder hb, BaseModel baseModel, string tableName)
        {
            return hb
                .RecordedTime(
                    controlId: tableName + "_CreatedTime",
                    labelText: Displays.CreatedTime(),
                    format: Def.ColumnTable._Bases_CreatedTime.EditorFormat,
                    userId: baseModel.Creator.Id,
                    time: baseModel.CreatedTime)
                .RecordedTime(
                    controlId: tableName + "_UpdatedTime",
                    labelText: Displays.UpdatedTime(),
                    format: Def.ColumnTable._Bases_UpdatedTime.EditorFormat,
                    userId: baseModel.Updator.Id,
                    time: baseModel.UpdatedTime);
        }

        private static HtmlBuilder RecordedTime(
            this HtmlBuilder hb,
            string controlId,
            string labelText,
            string format,
            int userId,
            Time time)
        {
            return hb.Div(action: () => hb
                .P(action: () => hb
                    .Text(labelText))
                .HtmlUser(userId)
                .RecordedTime(
                    format: format,
                    controlId: controlId,
                    time: time));
        }

        private static HtmlBuilder RecordedTime(
            this HtmlBuilder hb,
            string format,
            string controlId,
            Time time)
        {
            if (time != null && time.Value.InRange())
            {
                return hb
                    .P(action: () => hb
                        .Time(
                            attributes: new HtmlAttributes()
                                .Id(controlId)
                                .DateTime(time.Value)
                                .Class("time"),
                            action: () => hb
                                .Text(time.DisplayValue.ToViewText(
                                    Displays.Get(format + "Format")))))
                    .P(action: () => hb
                        .ElapsedTime(time.DisplayValue));
            }
            else
            {
                return hb.P(action: () => hb
                    .Text(text: Displays.Hyphen()));
            }
        }
    }
}