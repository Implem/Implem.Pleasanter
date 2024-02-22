using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBulkUpdate
    {
        public static HtmlBuilder BulkUpdateSelectorDialog(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Dictionary<string, string> optionCollection,
            Action action)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("BulkUpdateSelectorForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () =>
                {
                    hb.FieldDropDown(
                       context: context,
                       controlId: "BulkUpdateColumnName",
                       controlCss: " always-send",
                       labelText: Displays.Target(context: context),
                       onChange: "$p.send($(this));",
                       action: "BulkUpdateSelectChanged",
                       method: "post",
                       optionCollection: optionCollection)
                   .Div(id: "BulkUpdateSelectedField", action: () =>
                        action.Invoke())
                   .P(css: "message-dialog")
                   .Div(css: "command-center", action: () => hb
                       .Button(
                           text: Displays.BulkUpdate(context: context),
                           controlId: "BulkUpdate",
                           controlCss: "button-icon validate button-positive",
                           accessKey: "s",
                           onClick: "$p.bulkUpdate();",
                           icon: "ui-icon-copy",
                           action: "BulkUpdate",
                           method: "post")
                       .Button(
                           text: Displays.Cancel(context: context),
                           controlCss: "button-icon button-neutral",
                           onClick: "$p.closeDialog($(this));",
                           icon: "ui-icon-cancel"));
                });
        }
    }
}