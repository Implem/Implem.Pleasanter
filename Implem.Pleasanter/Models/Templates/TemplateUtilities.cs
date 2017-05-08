using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class TemplateUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder TemplateDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("TemplateDialog")
                .Class("dialog")
                .Title(Displays.Templates()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string OpenTemplateDialog(long id)
        {
            var hb = new HtmlBuilder();
            return new ResponseCollection().Html("#TemplateDialog", hb.Div(action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("TemplateDialogForm")
                        .Action(Locations.ItemAction(id)),
                    action: () => hb
                        .FieldSelectable(
                            controlId: "TemplateSelector",
                            fieldCss: "field-vertical w350",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h300",
                            controlCss: " single",
                            listItemCollection: Templates())
                        .FieldTextBox(
                            controlId: "SiteTitle",
                            controlCss: " always-send",
                            labelText: Displays.Title(),
                            validateRequired: true)
                        .P(css: "message-dialog")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                text: Displays.Create(),
                                controlCss: "button-icon validate",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-disk",
                                action: "CreateByTemplates",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel"))))).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> Templates()
        {
            return Rds.ExecuteTable(statements: Rds.SelectTemplates(
                column: Rds.TemplatesColumn()
                    .TemplateId()
                    .Title()
                    .Standard()
                    .SiteSettingsTemplate()))
                        .AsEnumerable()
                        .ToDictionary(
                            o => o["TemplateId"].ToString(),
                            o => new ControlData(Title(o)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Title(DataRow dataRow)
        {
            return dataRow["Standard"].ToBool()
                ? "(" + Displays.Standard() + ") " + dataRow["Title"].ToString()
                : dataRow["Title"].ToString();
        }
    }
}
