﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViewModes
    {
        public static ResponseCollection ViewMode(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            View view,
            string invoke = null,
            Message message = null,
            bool editOnGrid = false,
            bool loadScroll = false,
            bool bodyOnly = false,
            string bodySelector = null,
            ServerScriptModelRow serverScriptModelRow = null,
            HtmlBuilder body = null,
            bool replaceAllBody = false)
        {
            return res
                .Html(
                    target: !bodyOnly
                        ? "#ViewModeContainer"
                        : bodySelector,
                    value: body,
                    _using: !replaceAllBody || !bodyOnly)
                .ReplaceAll(
                    target: !bodyOnly
                        ? "#ViewModeContainer"
                        : bodySelector,
                    value: body,
                    _using: replaceAllBody && bodyOnly)
                .View(
                    context: context,
                    ss: ss,
                    view: view)
                .Invoke("initRelatingColumnWhenViewChanged")
                .ReplaceAll(
                    "title",
                    HtmlTitle.Title(
                        context: context,
                        ss: ss))
                .ReplaceAll(
                    "#Breadcrumb",
                    new HtmlBuilder().Breadcrumb(
                        context: context,
                        ss: ss),
                    _using: context.Controller == "items")
                .ReplaceAll("#Guide", new HtmlBuilder()
                    .Guide(
                        context: context,
                        ss: ss,
                        view: view))
                .ReplaceAll("#CopyToClipboards", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(
                        context: context,
                        view: view))
                .ReplaceAll("#Aggregations", new HtmlBuilder()
                    .Aggregations(
                        context: context,
                        ss: ss,
                        view: view))
                .ReplaceAll("#MainCommandsContainer", new HtmlBuilder()
                    .MainCommands(
                        context: context,
                        ss: ss,
                        view: view,
                        verType: Versions.VerTypes.Latest,
                        backButton: !context.Publish && !editOnGrid,
                        serverScriptModelRow: serverScriptModelRow))
                .Val("#EditOnGrid", editOnGrid.ToOneOrZeroString())
                .SetMemory("formChanged", false, _using: !editOnGrid)
                .ClearFormData(
                    context: context,
                    ss: ss,
                    editOnGrid: editOnGrid)
                .Invoke(invoke)
                .Message(message)
                .LoadScroll(loadScroll)
                .FilterField(
                    context: context,
                    ss: ss,
                    view: view,
                    controlId: "ViewFilters_Negative",
                    prefix: "ViewFilters__")
                .FilterField(
                    context: context,
                    ss: ss,
                    view: view,
                    controlId: "ViewFilters_Positive",
                    prefix: "ViewFilters__")
                .ServerScriptResponses(
                    context: context,
                    ss: ss,
                    view: view,
                    responses: serverScriptModelRow?.Responses);
        }

        private static ResponseCollection ClearFormData(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            bool editOnGrid)
        {
            if (editOnGrid)
            {
                new FormDataSet(
                    context: context,
                    ss: ss)
                        .Where(o => o.Suffix.IsNullOrEmpty())
                        .ForEach(formData =>
                            formData.Data.Keys.ForEach(controlId =>
                                res.ClearFormData(
                                    controlId,
                                    type: ss.SaveViewType == SiteSettings.SaveViewTypes.None
                                        ? "ignoreView"
                                        : null)));
            }
            else
            {
                res.ClearFormData(
                    type: ss.SaveViewType == SiteSettings.SaveViewTypes.None
                        ? "ignoreView"
                        : null);
            }
            if (ss.SaveViewType == SiteSettings.SaveViewTypes.None)
            {
                var controlId = context.Forms.Get("ControlId") ?? string.Empty;
                if (controlId == "ViewFilters_Reset")
                {
                    foreach (var key in context.Forms.Keys)
                    {
                        if (key.StartsWith("ViewFilters_") || key.StartsWith("ViewSorters_"))
                        {
                            res.ClearFormData(target: key);
                        }
                    }
                }
                else if (controlId.StartsWith("ViewFilters_") || controlId.StartsWith("ViewSorters_"))
                {
                    res.SetFormData(
                        target: controlId,
                        value: context.Forms.Get(controlId) ?? string.Empty);
                }
            }
            return res;
        }
    }
}