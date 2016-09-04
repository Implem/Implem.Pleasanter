using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordInfo
    {
        public static HtmlBuilder RecordInfo(
            this HtmlBuilder hb, BaseModel baseModel, string tableName)
        {
            return hb
                .Div(action: () => hb
                    .P(action: () => hb
                        .Displays_Create())
                    .HtmlUser(baseModel.Creator.Id)
                    .RecordTime(
                        controlDateTime: Def.ColumnTable._Bases_CreatedTime.ControlDateTime,
                        controlId: tableName + "_CreatedTime",
                        time: baseModel.CreatedTime))
                .Div(action: () => hb
                    .P(action: () => hb
                        .Displays_Update())
                    .HtmlUser(baseModel.Updator.Id)
                    .RecordTime(
                        controlDateTime: Def.ColumnTable._Bases_UpdatedTime.ControlDateTime,
                        controlId: tableName + "_UpdatedTime",
                        time: baseModel.UpdatedTime));
        }

        private static HtmlBuilder RecordTime(
            this HtmlBuilder hb,
            string controlDateTime,
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
                                .Text(time.ToViewText(
                                    Displays.Get(controlDateTime + "Format")))))
                    .P(action: () => hb
                        .ElapsedTime(time.Value));
            }
            else
            {
                return hb.P(action: () => hb
                    .Displays_Hyphen());
            }
        }
    }
}