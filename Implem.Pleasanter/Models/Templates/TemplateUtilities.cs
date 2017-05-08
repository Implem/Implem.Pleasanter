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
        public static string OpenTemplateDialog()
        {
            var hb = new HtmlBuilder();
            return new ResponseCollection().Html("#TemplateDialog", hb.Div(action: () => hb
                .FieldSet(
                    css: "fieldset",
                    action: () => hb
                        .FieldSelectable(
                            controlId: "TemplateSelector",
                            fieldCss: "field-vertical w500",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h300",
                            listItemCollection: Templates())
                        .P(css: "message-dialog")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                text: Displays.Create(),
                                controlCss: "button-icon",
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
                    .Body()
                    .SiteSettingsTemplate()))
                        .AsEnumerable()
                        .ToDictionary(
                            o => o["TemplateId"].ToString(),
                            o => new ControlData(o["Title"].ToString()));
        }
    }
}
