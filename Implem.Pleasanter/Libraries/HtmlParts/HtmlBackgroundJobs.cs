using System;
using System.Collections.Generic;
using System.Linq;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;

namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlBackgroundJobs
    {
        private static readonly Dictionary<int, string> StatusCssClasses =
            new Dictionary<int, string>
        {
            { BackgroundJobStatus.Failed, "background-job-warning" },
            { BackgroundJobStatus.RunningOverdue, "background-job-warning" }
        };

        private static readonly Dictionary<int, string> StatusDisplayIds =
            new Dictionary<int, string>
        {
            { BackgroundJobStatus.Pending, "BackgroundJobStatusPending" },
            { BackgroundJobStatus.Running, "BackgroundJobStatusRunning" },
            { BackgroundJobStatus.Completed, "BackgroundJobStatusCompleted" },
            { BackgroundJobStatus.Failed, "BackgroundJobStatusFailed" },
            { BackgroundJobStatus.Cancelled, "BackgroundJobStatusCancelled" },
            { BackgroundJobStatus.RunningOverdue, "BackgroundJobStatusRunningOverdue" }
        };

        private static string GetJobTypeLabel(Context context, string jobType)
        {
            return BackgroundJobQueue.GetJobTypeLabel(
                context: context,
                jobType: jobType);
        }

        private static string GetStatusLabel(Context context, int status)
        {
            return StatusDisplayIds.ContainsKey(status)
                ? Displays.Get(
                    context: context,
                    id: StatusDisplayIds[status])
                : status.ToString();
        }

        private static bool CanCancel(int status)
        {
            return status == BackgroundJobStatus.Pending;
        }

        private static bool CanDelete(int status)
        {
            return status == BackgroundJobStatus.Pending
                || status == BackgroundJobStatus.Completed
                || status == BackgroundJobStatus.Failed
                || status == BackgroundJobStatus.Cancelled;
        }

        private static bool CanRunNextJob(BackgroundJobModel model)
        {
            return model.Status == BackgroundJobStatus.RunningOverdue;
        }

        public static string BackgroundJobsIndex(this HtmlBuilder hb, Context context)
        {
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return hb.Template(
                    context: context,
                    ss: new SiteSettings(),
                    view: null,
                    title: Displays.BackgroundJobs(context: context),
                    useNavigationMenu: true,
                    action: () => hb.Div(
                        css: "not-found",
                        action: () => hb.Text(
                            text: Displays.HasNotPermission(context: context))))
                    .ToString();
            }
            var ss = new SiteSettings();
            var sortState = BackgroundJobSortState.FromContext(context: context);
            var filter = BackgroundJobFilterParams.FromContext(context: context);
            var jobs = BackgroundJobQueue.GetList(
                context: context,
                filter: filter,
                sortState: sortState);
            return hb.Template(
                context: context,
                ss: ss,
                view: null,
                title: Displays.BackgroundJobs(context: context)
                    + " - "
                    + Displays.Grid(context: context),
                useNavigationMenu: true,
                action: () => hb.Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form")
                        .Action(Locations.Action(
                            context: context,
                            controller: context.Controller,
                            id: 0)),
                    action: () =>
                    {
                        Filters(hb: hb, context: context, filter: filter);
                        Aggregations(hb: hb, context: context, jobs: jobs);
                        Grid(
                            hb: hb,
                            context: context,
                            jobs: jobs,
                            sortState: sortState);
                    }))
                    .Hidden(
                        controlId: "BaseUrl",
                        value: Locations.BaseUrl(context: context))
                    .MainCommands(
                        context: context,
                        ss: ss,
                        verType: Versions.VerTypes.Latest,
                        backButton: false,
                        extensions: () =>
                        {
                            hb.Button(
                                controlId: "GoBack",
                                text: Displays.GoBack(context: context),
                                controlCss: "button-icon button-neutral",
                                accessKey: "q",
                                onClick: "$p.back();",
                                icon: "ui-icon-circle-arrow-w");
                            hb.Button(
                                controlId: "BulkDeleteCommand",
                                text: Displays.BulkDelete(context: context),
                                controlCss: "button-icon button-negative",
                                onClick: "$p.backgroundJobBulkCommand($(this));",
                                icon: "ui-icon-trash",
                                action: "BulkDelete",
                                method: "post",
                                confirm: Displays.ConfirmDelete(context: context),
                                attributes: new Dictionary<string, string>
                                {
                                    ["data-url"] = Locations.Action(
                                        context: context,
                                        controller: "BackgroundJobs")
                                            .Replace("_action_", "bulkdelete")
                                });
                            hb.Button(
                                controlId: "BulkNextJobCommand",
                                text: Displays.Get(
                                    context: context,
                                    id: "BackgroundJobRunNextJob"),
                                controlCss: "button-icon button-positive",
                                onClick: "$p.backgroundJobBulkCommand($(this));",
                                icon: "ui-icon-play",
                                action: "BulkNextJob",
                                method: "post",
                                attributes: new Dictionary<string, string>
                                {
                                    ["data-url"] = Locations.Action(
                                        context: context,
                                        controller: "BackgroundJobs")
                                            .Replace("_action_", "bulknextjob")
                                });
                            hb.Button(
                                controlId: "BulkCancelCommand",
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-positive",
                                onClick: "$p.backgroundJobBulkCommand($(this));",
                                icon: "ui-icon-cancel",
                                action: "BulkCancel",
                                method: "post",
                                confirm: Displays.Get(
                                    context: context,
                                    id: "BackgroundJobsBulkCancelConfirm"),
                                attributes: new Dictionary<string, string>
                                {
                                    ["data-url"] = Locations.Action(
                                        context: context,
                                        controller: "BackgroundJobs")
                                            .Replace("_action_", "bulkcancel")
                                });
                        })
                    .Div(attributes: new HtmlAttributes()
                        .Id("SetDateRangeDialog")
                        .Class("dialog")
                        .Title(Displays.DateRange(context: context)))
                    .ToString();
        }

        private static void Filters(
            HtmlBuilder hb,
            Context context,
            BackgroundJobFilterParams filter)
        {
            hb.Div(id: "ViewFilters", action: () =>
            {
                hb.Div(
                    id: "ReduceViewFilters",
                    css: "display-control",
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-close")
                        .Text(Displays.Filters(context: context) + ":"));
                hb.Button(
                    controlId: "ViewFilters_Reset",
                    text: Displays.Reset(context: context),
                    controlCss: "button-icon",
                    icon: "ui-icon-close",
                    action: "IndexJson",
                    method: "post");
                hb.FieldTextBox(
                    context: context,
                    fieldId: "ViewFilters__BackgroundJobIdField",
                    controlId: "ViewFilters__BackgroundJobId",
                    fieldCss: "field-auto-thin",
                    controlCss: "control-textbox auto-postback",
                    labelText: Displays.Id(context: context),
                    text: filter.BackgroundJobId,
                    action: "IndexJson",
                    method: "post");
                hb.FieldDropDown(
                    context: context,
                    fieldId: "ViewFilters__JobTypeField",
                    controlId: "ViewFilters__JobType",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.Get(
                        context: context,
                        id: "BackgroundJobsJobType"),
                    optionCollection: JobTypeOptionCollection(context: context),
                    selectedValue: filter.JobTypes?.Any() == true
                        ? filter.JobTypes.ToJson() : null,
                    multiple: true,
                    addSelectedValue: false,
                    action: "IndexJson",
                    method: "post");
                hb.FieldDropDown(
                    context: context,
                    fieldId: "ViewFilters__StatusField",
                    controlId: "ViewFilters__Status",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.Status(context: context),
                    optionCollection: StatusOptionCollection(context: context),
                    selectedValue: filter.Statuses?.Any() == true
                        ? filter.Statuses.Select(s => s.ToString()).ToList().ToJson() : null,
                    multiple: true,
                    addSelectedValue: false,
                    action: "IndexJson",
                    method: "post");
                hb.FieldTextBox(
                    context: context,
                    fieldId: "ViewFilters__SiteIdField",
                    controlId: "ViewFilters__SiteId",
                    fieldCss: "field-auto-thin",
                    controlCss: "control-textbox auto-postback",
                    labelText: Displays.SiteId(context: context),
                    text: filter.SiteId,
                    action: "IndexJson",
                    method: "post");
                hb.FieldTextBox(
                    context: context,
                    fieldId: "ViewFilters__SiteNameField",
                    controlId: "ViewFilters__SiteName",
                    fieldCss: "field-auto-thin",
                    controlCss: "control-textbox auto-postback",
                    labelText: Displays.Get(
                        context: context,
                        id: "BackgroundJobsSiteName"),
                    text: filter.SiteName,
                    action: "IndexJson",
                    method: "post");
                hb.FieldDropDown(
                    context: context,
                    fieldId: "ViewFilters__UserIdField",
                    controlId: "ViewFilters__UserId",
                    fieldCss: "field-auto-thin",
                    labelText: Displays.Get(
                        context: context,
                        id: "BackgroundJobsRegisteredUser"),
                    optionCollection: UserOptionCollection(context: context),
                    selectedValue: filter.UserIds?.Any() == true
                        ? filter.UserIds.Select(u => u.ToString()).ToList().ToJson() : null,
                    multiple: true,
                    addSelectedValue: false,
                    action: "IndexJson",
                    method: "post");
                DateRangeFilter(
                    hb: hb,
                    context: context,
                    columnName: "JobEnqueuedTime",
                    labelId: "BackgroundJobsJobEnqueuedTime",
                    value: filter.JobEnqueuedTime);
                hb.FieldTextBox(
                    context: context,
                    fieldId: "ViewFilters__ResultMessageField",
                    controlId: "ViewFilters__ResultMessage",
                    fieldCss: "field-auto-thin",
                    controlCss: "control-textbox auto-postback",
                    labelText: Displays.Get(
                        context: context,
                        id: "BackgroundJobsResultMessage"),
                    text: filter.ResultMessage,
                    action: "IndexJson",
                    method: "post");
                hb.FieldTextBox(
                    context: context,
                    fieldId: "ViewFilters__FileField",
                    controlId: "ViewFilters__File",
                    fieldCss: "field-auto-thin",
                    controlCss: "control-textbox auto-postback",
                    labelText: Displays.Get(
                        context: context,
                        id: "File"),
                    text: filter.File,
                    action: "IndexJson",
                    method: "post");
                hb.Button(
                    controlId: "FilterButton",
                    text: Displays.Filters(context: context),
                    controlCss: "button-icon",
                    icon: "ui-icon-search",
                    onClick: "$p.send($(this));",
                    action: "IndexJson",
                    method: "post");
            });
        }

        private static Dictionary<string, ControlData> JobTypeOptionCollection(
            Context context)
        {
            var result = new Dictionary<string, ControlData>();
            foreach (var jobType in BackgroundJobQueue.JobTypes())
            {
                result.Add(
                    jobType,
                    new ControlData(GetJobTypeLabel(
                        context: context,
                        jobType: jobType)));
            }
            return result;
        }

        private static Dictionary<string, ControlData> StatusOptionCollection(
            Context context)
        {
            var result = new Dictionary<string, ControlData>();
            foreach (var kv in StatusDisplayIds)
            {
                result.Add(
                    kv.Key.ToString(),
                    new ControlData(GetStatusLabel(context: context, kv.Key)));
            }
            return result;
        }

        private static void DateRangeFilter(
            HtmlBuilder hb,
            Context context,
            string columnName,
            string labelId,
            string value)
        {
            var controlId = $"ViewFilters__{columnName}_DateRange";
            var hiddenId = $"ViewFilters__{columnName}";
            hb.FieldTextBox(
                context: context,
                fieldId: $"ViewFilters__{columnName}_DateRangeField",
                controlId: controlId,
                fieldCss: "field-auto-thin",
                controlCss: string.Empty,
                labelText: Displays.Get(
                    context: context,
                    id: labelId),
                text: HtmlViewFilters.GetDisplayDateFilterRange(
                    context: context,
                    value: value,
                    timepicker: true),
                action: "OpenSetDateRangeDialog",
                method: "post",
                attributes: new Dictionary<string, string>
                {
                    ["onclick"] = $"$p.openSetDateRangeDialog($(this))"
                });
            hb.Hidden(attributes: new HtmlAttributes()
                .Id(hiddenId)
                .Class("always-send")
                .DataMethod("post")
                .Value(value));
        }

        private static Dictionary<string, ControlData> UserOptionCollection(
            Context context)
        {
            var result = new Dictionary<string, ControlData>();
            if (BackgroundJobAccessValidator.GetAccessScope(context: context)
                == BackgroundJobAccessScope.All)
            {
                var dataTable = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn()
                            .UserId()
                            .Name()
                            .Disabled()));
                var users = new List<(int UserId, string Name)>();
                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];
                    if (row.Int("UserId") > 0 && row.Bool("Disabled") == false)
                    {
                        users.Add((row.Int("UserId"), row.String("Name")));
                    }
                }
                foreach (var user in users.OrderBy(u => u.Name))
                {
                    result.Add(
                        user.UserId.ToString(),
                        new ControlData(user.Name));
                }
            }
            else
            {
                var userHash = SiteInfo.TenantCaches.Get(context.TenantId)?.UserHash;
                if (userHash != null)
                {
                    foreach (var user in userHash.Values
                        .Where(u => u.Id > 0 && !u.Disabled)
                        .OrderBy(u => u.Name))
                    {
                        result.Add(
                            user.Id.ToString(),
                            new ControlData(user.Name));
                    }
                }
            }
            return result;
        }

        internal static void Aggregations(
            HtmlBuilder hb, Context context, BackgroundJobCollection jobs)
        {
            hb.Div(id: "Aggregations", css: "background-jobs", action: () =>
            {
                hb.Div(
                    id: "ReduceAggregations",
                    css: "display-control",
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-close")
                        .Text(Displays.Aggregations(context: context) + ":"));
                hb.Span(css: "label", action: () =>
                    hb.Text(Displays.Get(
                        context: context,
                        id: "BackgroundJobsTotalCount")));
                hb.Span(css: "data", id: "aggTotal", action: () =>
                    hb.Text(jobs.TotalCount.ToString()));
                foreach (var statusKv in StatusDisplayIds)
                {
                    var count = jobs.Count(j => j.Status == statusKv.Key);
                    var cssClass = StatusCssClasses.ContainsKey(statusKv.Key)
                        ? " " + StatusCssClasses[statusKv.Key]
                        : string.Empty;
                    var label = GetStatusLabel(context, statusKv.Key);
                    hb.Span(css: "label" + cssClass, action: () =>
                        hb.Text(label));
                    hb.Span(
                        css: "data" + cssClass,
                        id: $"agg{statusKv.Key}",
                        action: () => hb.Text(count.ToString()));
                }
            });
        }

        private static void Grid(
            HtmlBuilder hb,
            Context context,
            BackgroundJobCollection jobs,
            BackgroundJobSortState sortState = null)
        {
            hb.Div(id: "ViewModeContainer", action: () =>
                hb.GridTable(
                    context: context,
                    id: "Grid",
                    css: null,
                    attributes: new HtmlAttributes()
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    scrollable: true,
                    action: () =>
                    {
                        GridHeader(
                            hb: hb,
                            context: context,
                            sortState: sortState);
                        hb.TBody(id: "GridBody", action: () =>
                        {
                            foreach (var job in jobs)
                            {
                                GridRow(hb: hb, context: context, job: job);
                            }
                        });
                    }));
            GridHeaderMenus(hb: hb, context: context);
            hb.Button(
                controlId: "ViewSorters_Reset",
                controlCss: "hidden",
                action: "GridRows",
                method: "post");
        }

        private static void GridHeader(
            HtmlBuilder hb,
            Context context,
            BackgroundJobSortState sortState = null)
        {
            hb.THead(action: () =>
                hb.Tr(css: "ui-widget-header", action: () =>
                {
                    hb.Th(action: () =>
                        hb.Label(
                            attributes: new HtmlAttributes()
                                .Class("check-option")
                                .For("GridCheckAll"),
                            action: () =>
                                hb.Span(css: "check-icon", action: () =>
                                    hb.Input(attributes: new HtmlAttributes()
                                        .Id("GridCheckAll")
                                        .Name("GridCheckAll")
                                        .Class("control-checkbox")
                                        .Type("checkbox")))
                                .Span(css: "check-text")));
                    GridTh(
                        hb: hb,
                        dataName: "BackgroundJobId",
                        text: Displays.Id(context: context),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "JobType",
                        text: Displays.Get(
                            context: context,
                            id: "BackgroundJobsJobType"),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "Status",
                        text: Displays.Status(context: context),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "SiteId",
                        text: Displays.SiteId(context: context),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "SiteName",
                        text: Displays.Get(
                            context: context,
                            id: "BackgroundJobsSiteName"),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "UserId",
                        text: Displays.Get(
                            context: context,
                            id: "BackgroundJobsRegisteredUser"),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "JobEnqueuedTime",
                        text: Displays.Get(
                            context: context,
                            id: "BackgroundJobsJobEnqueuedTime"),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "ResultMessage",
                        text: Displays.Get(
                            context: context,
                            id: "BackgroundJobsResultMessage"),
                        sortState: sortState);
                    GridTh(
                        hb: hb,
                        dataName: "File",
                        text: Displays.Get(
                            context: context,
                            id: "File"),
                        sortState: sortState);
                }));
        }

        private static void GridTh(
            HtmlBuilder hb,
            string dataName,
            string text,
            bool sortable = true,
            BackgroundJobSortState sortState = null)
        {
            var css = sortable ? "sortable" : "";
            hb.Th(
                css: css,
                attributes: new HtmlAttributes().DataName(dataName),
                action: () =>
                {
                    if (sortable)
                    {
                        hb.Div(
                            attributes: new HtmlAttributes()
                                .Add("data-id", "ViewSorters__" + dataName)
                                .Add("data-order-type", sortState?.NextOrderType(dataName) ?? "asc")
                                .DataAction("GridRows")
                                .DataMethod("post"),
                            action: () => hb
                                .Span(action: () => hb.Text(text))
                                .SortIcon(sortState: sortState, columnName: dataName));
                    }
                    else
                    {
                        hb.Div(action: () =>
                            hb.Span(action: () => hb.Text(text)));
                    }
                });
        }

        private static void GridHeaderMenus(HtmlBuilder hb, Context context)
        {
            var sortableColumns = new[]
            {
                "BackgroundJobId", "JobType", "Status", "SiteId",
                "SiteName", "UserId", "JobEnqueuedTime",
                "ResultMessage", "File"
            };
            hb.Div(id: "GridHeaderMenus", action: () =>
            {
                foreach (var colName in sortableColumns)
                {
                    hb.Ul(
                        id: "GridHeaderMenu__" + colName,
                        attributes: new HtmlAttributes()
                            .Class("menu menu-sort")
                            .Add("data-target", $"[data-id='ViewSorters__{colName}']")
                            .Add("style", "display: none; position: absolute;"),
                        action: () => hb
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "asc"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes()
                                            .Class("ui-icon ui-icon-triangle-1-n"))
                                        .Text(Displays.OrderAsc(context: context))))
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "desc"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes()
                                            .Class("ui-icon ui-icon-triangle-1-s"))
                                        .Text(Displays.OrderDesc(context: context))))
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("sort")
                                    .Add("data-order-type", "release"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes()
                                            .Class("ui-icon ui-icon-close"))
                                        .Text(Displays.OrderRelease(context: context))))
                            .Li(
                                attributes: new HtmlAttributes()
                                    .Class("reset"),
                                action: () => hb
                                    .Div(action: () => hb
                                        .Span(attributes: new HtmlAttributes()
                                            .Class("ui-icon ui-icon-power"))
                                        .Text(Displays.ResetOrder(context: context)))));
                }
            });
        }

        public static HtmlBuilder SortIcon(
            this HtmlBuilder hb,
            BackgroundJobSortState sortState,
            string columnName)
        {
            if (sortState == null) return hb;
            switch (sortState.CurrentOrderType(columnName))
            {
                case "asc":
                    return hb.Icon(iconCss: "ui-icon-triangle-1-n");
                case "desc":
                    return hb.Icon(iconCss: "ui-icon-triangle-1-s");
                default:
                    return hb;
            }
        }

        private static string NotPermittedJson(Context context)
        {
            return new ResponseCollection(context: context)
                .Message(new Message(
                    id: "HasNotPermission",
                    text: Displays.HasNotPermission(context: context),
                    css: "error"))
                .ToJson();
        }

        private static HtmlBuilder AggregationsHtml(
            Context context, BackgroundJobCollection jobs)
        {
            var hb = new HtmlBuilder();
            Aggregations(hb: hb, context: context, jobs: jobs);
            return hb;
        }

        private static HtmlBuilder GridBodyHtml(
            Context context, BackgroundJobCollection jobs)
        {
            var hb = new HtmlBuilder();
            foreach (var job in jobs)
            {
                GridRow(hb: hb, context: context, job: job);
            }
            return hb;
        }

        public static string BackgroundJobsIndexJson(Context context)
        {
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return NotPermittedJson(context: context);
            }
            var sortState = BackgroundJobSortState.FromContext(context: context);
            var filter = BackgroundJobFilterParams.FromContext(context: context);
            var jobs = BackgroundJobQueue.GetList(
                context: context,
                filter: filter,
                sortState: sortState);
            var filtersHb = new HtmlBuilder();
            Filters(hb: filtersHb, context: context, filter: filter);
            return new ResponseCollection(context: context)
                .ReplaceAll("#ViewFilters", filtersHb)
                .ReplaceAll("#Aggregations", AggregationsHtml(context: context, jobs: jobs))
                .Html("#GridBody", GridBodyHtml(context: context, jobs: jobs))
                .ClearFormData("GridCheckAll")
                .ClearFormData("GridUnCheckedItems")
                .ClearFormData("GridCheckedItems")
                .Invoke("clearGridCheckState")
                .ToJson();
        }

        public static string BackgroundJobsGridRows(Context context)
        {
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return NotPermittedJson(context: context);
            }
            var sortState = BackgroundJobSortState.FromContext(context: context);
            var filter = BackgroundJobFilterParams.FromContext(context: context);
            var jobs = BackgroundJobQueue.GetList(
                context: context,
                filter: filter,
                sortState: sortState);
            var gridHeaderHb = new HtmlBuilder();
            GridHeader(hb: gridHeaderHb, context: context, sortState: sortState);
            return new ResponseCollection(context: context)
                .ReplaceAll("#Grid thead", gridHeaderHb)
                .ReplaceAll("#Aggregations", AggregationsHtml(context: context, jobs: jobs))
                .Html("#GridBody", GridBodyHtml(context: context, jobs: jobs))
                .ClearFormData("GridCheckAll")
                .ClearFormData("GridUnCheckedItems")
                .ClearFormData("GridCheckedItems")
                .Invoke("clearGridCheckState")
                .ToJson();
        }

        private static void GridRow(
            HtmlBuilder hb, Context context, BackgroundJobModel job)
        {
            var cssClass = StatusCssClasses.ContainsKey(job.Status)
                ? StatusCssClasses[job.Status]
                : "";
            var jobTypeLabel = GetJobTypeLabel(
                context: context,
                jobType: job.JobType);
            var statusLabel = GetStatusLabel(context, job.Status);
            var userName = GetUserName(
                context: context,
                tenantId: job.TenantId,
                userId: job.UserId);
            var canCancel = CanCancel(status: job.Status);
            var canDelete = CanDelete(status: job.Status);
            var canRunNextJob = CanRunNextJob(model: job);
            hb.Tr(
                css: "grid-row",
                attributes: new HtmlAttributes()
                    .DataId(job.BackgroundJobId.ToString())
                    .DataVer(1)
                    .DataLatest(1)
                    .Add("data-status", job.Status.ToString()),
                action: () =>
                {
                    hb.Td(action: () =>
                    {
                        hb.Label(
                            attributes: new HtmlAttributes()
                                .Class("check-option"),
                            action: () =>
                                hb.Span(css: "check-icon", action: () =>
                                    hb.Input(attributes: new HtmlAttributes()
                                        .Class("grid-check row-check")
                                        .Type("checkbox")
                                        .DataId(job.BackgroundJobId
                                            .ToString())
                                        .Add("data-status", job.Status
                                            .ToString())
                                        .Add("data-can-delete", canDelete.ToOneOrZeroString())
                                        .Add("data-can-cancel", canCancel.ToOneOrZeroString())
                                        .Add("data-can-next-job", canRunNextJob.ToOneOrZeroString())))
                                .Span(css: "check-text"));
                    });
                    hb.Td(action: () =>
                    {
                        hb.Text(job.BackgroundJobId.ToString());
                    });
                    hb.Td(action: () =>
                        hb.Text(jobTypeLabel));
                    hb.Td(action: () =>
                        hb.Span(
                            css: cssClass != string.Empty
                                ? $"background-job-status {cssClass}"
                                : "background-job-status",
                            action: () => hb.Text(statusLabel)));
                    hb.Td(action: () =>
                        hb.Text(job.SiteId.ToString()));
                    hb.Td(action: () =>
                        hb.Text(GetSiteName(
                            context: context,
                            tenantId: job.TenantId,
                            siteId: job.SiteId)));
                    hb.Td(action: () =>
                        hb.P(css: "user", action: () =>
                            hb.Span(
                                css: "ui-icon ui-icon-person")
                                .Span(action: () =>
                                    hb.Text(userName))));
                    hb.Td(action: () =>
                        hb.P(css: "time", action: () =>
                            hb.Text(job.JobEnqueuedTime
                                .ToBackgroundJobLocalTime(context: context).InRange()
                                    ? job.JobEnqueuedTime
                                        .ToBackgroundJobLocalTime(context: context)
                                        .ToString("yyyy/MM/dd HH:mm")
                                    : string.Empty)));
                    hb.Td(
                        css: "result-msg",
                        attributes: new HtmlAttributes()
                            .Style("white-space: normal;"),
                        action: () => hb.Text(job.ResultMessage ?? string.Empty));
                    hb.Td(action: () =>
                    {
                        if (CanDownload(job: job))
                        {
                            hb.A(
                                attributes: new HtmlAttributes()
                                    .Href(Locations.Action(
                                        context: context,
                                        controller: "BackgroundJobs",
                                        id: job.BackgroundJobId)
                                    .Replace("_action_", "download"))
                                    .OnClick("event.stopPropagation();"),
                                action: () => hb.Text(
                                    new System.IO.FileInfo(
                                        job.File).Name));
                        }
                        else if (job.Status == BackgroundJobStatus.Completed)
                        {
                            hb.Text(Displays.Get(
                                context: context,
                                id: "BackgroundJobsNoDownloadableFile"));
                        }
                    });
                });
        }

        private static bool CanDownload(BackgroundJobModel job)
        {
            return job.Status == BackgroundJobStatus.Completed
                && job.File.IsNullOrEmpty() == false
                && System.IO.File.Exists(job.File);
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }
            if (bytes < 1024 * 1024)
            {
                return $"{bytes / 1024.0:F2} KB";
            }
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }

        private static string GetSiteName(
            Context context,
            int tenantId,
            long siteId)
        {
            if (tenantId == context.TenantId)
            {
                var sites = SiteInfo.Sites(context: context);
                if (sites?.ContainsKey(siteId) == true)
                {
                    return sites[siteId].String("Title");
                }
                return siteId.ToString();
            }
            var dataTable = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().Title(),
                    where: Rds.SitesWhere()
                        .TenantId(tenantId)
                        .SiteId(siteId)));
            return dataTable.Rows.Count == 1
                ? dataTable.Rows[0].String("Title")
                : siteId.ToString();
        }

        private static string GetUserName(
            Context context,
            int tenantId,
            int userId)
        {
            if (tenantId == context.TenantId)
            {
                return SiteInfo.UserName(
                    context: context,
                    userId: userId);
            }
            var dataTable = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().Name(),
                    where: Rds.UsersWhere().UserId(userId)));
            return dataTable.Rows.Count == 1
                ? dataTable.Rows[0].String("Name")
                : Displays.NotSet(context: context);
        }

        public static string BackgroundJobsEditor(
            Context context,
            long backgroundJobId)
        {
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(
                        type: Error.Types.HasNotPermission));
            }
            var model = BackgroundJobQueue.Get(
                context: context,
                backgroundJobId: backgroundJobId);
            var errorData = BackgroundJobAccessValidator.OnEditing(
                context: context,
                model: model);
            if (errorData.Type != Error.Types.None)
            {
                return HtmlTemplates.Error(
                    context: context,
                    errorData: errorData);
            }
            var hb = new HtmlBuilder();
            return hb.Template(
                context: context,
                ss: new SiteSettings(),
                view: null,
                referenceType: "BackgroundJobs",
                title: Displays.BackgroundJobs(context: context)
                    + " - "
                    + Displays.Get(
                        context: context,
                        id: "BackgroundJobsDetail"),
                useNavigationMenu: true,
                action: () => hb.Editor(
                    context: context,
                    model: model)).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            BackgroundJobModel model)
        {
            return hb.Div(id: "Editor", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id("MainForm")
                        .Class("main-form")
                        .Action(Locations.Action(
                            context: context,
                            controller: "BackgroundJobs",
                            id: model.BackgroundJobId)),
                    action: () => hb
                        .Div(id: "RecordHeader", action: () => hb
                            .Div(id: "RecordInfo", action: () => hb
                                .RecordInfo(
                                    context: context,
                                    baseModel: model,
                                    tableName: "BackgroundJobs"))
                            .Div(id: "RecordSwitchers", action: () => hb
                                .Button(
                                    controlId: "Reload",
                                    text: Displays.Reload(
                                        context: context),
                                    controlCss: "button-icon confirm-unload",
                                    onClick: "$p.ajax('{0}','post');".Params(
                                        Locations.Action(
                                            context: context,
                                            controller: "BackgroundJobs",
                                            id: model.BackgroundJobId)
                                            .Replace("_action_", "Edit")),
                                    icon: "ui-icon-refresh")))
                        .Div(
                            id: "EditorTabsContainer",
                            css: "tab-container max",
                            action: () => hb
                                .EditorTabs(context: context)
                                .FieldSetGeneral(
                                    context: context,
                                    model: model)
                                .Div(id: "MainCommandsContainer", action: () => hb
                                    .Div(id: "MainCommands", action: () =>
                                    {
                                        hb.Button(
                                            controlId: "GoBack",
                                            text: Displays.GoBack(
                                                context: context),
                                            controlCss: "button-icon button-neutral",
                                            accessKey: "q",
                                            onClick: "$p.back();",
                                            icon: "ui-icon-circle-arrow-w");
                                        hb.Button(
                                            controlId: "DeleteCommand",
                                            text: Displays.Delete(
                                                context: context),
                                            controlCss: "button-icon button-negative",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-trash",
                                            action: "Delete",
                                            method: "post",
                                            confirm: Displays.ConfirmDelete(
                                                context: context));
                                        hb.Button(
                                            controlId: "NextJobCommand",
                                            text: Displays.Get(
                                                context: context,
                                                id: "BackgroundJobRunNextJob"),
                                            controlCss: "button-icon button-positive",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-play",
                                            action: "NextJob",
                                            method: "post");
                                        hb.Button(
                                            controlId: "CancelCommand",
                                            text: Displays.Cancel(
                                                context: context),
                                            controlCss: "button-icon button-positive",
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-cancel",
                                            action: "Cancel",
                                            method: "post",
                                            confirm: Displays.Get(
                                                context: context,
                                                id: "BackgroundJobsBulkCancelConfirm"));
                                    })))
                        .Hidden(
                            controlId: "BaseUrl",
                            value: Locations.BaseUrl(context: context))
                        .Hidden(
                            controlId: "BackgroundJobId",
                            value: model.BackgroundJobId.ToString())));
        }

        private static HtmlBuilder EditorTabs(
            this HtmlBuilder hb,
            Context context)
        {
            return hb.Ul(id: "EditorTabs", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral",
                        text: Displays.General(context: context))));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            Context context,
            BackgroundJobModel model)
        {
            var statusLabel = GetStatusLabel(
                context: context,
                status: model.Status);
            var statusCss = StatusCssClasses.ContainsKey(model.Status)
                ? StatusCssClasses[model.Status]
                : string.Empty;
            var jobTypeLabel = GetJobTypeLabel(
                context: context,
                jobType: model.JobType);
            var userName = GetUserName(
                context: context,
                tenantId: model.TenantId,
                userId: model.UserId);
            var siteName = GetSiteName(
                context: context,
                tenantId: model.TenantId,
                siteId: model.SiteId);
            var canDownload = CanDownload(job: model);
            return hb.TabsPanelField(
                id: "FieldSetGeneral",
                action: () =>
                {
                    hb.FieldText(
                        controlId: "BackgroundJobs_BackgroundJobId",
                        fieldCss: "field-normal",
                        labelText: Displays.Id(context: context),
                        text: model.BackgroundJobId.ToString());
                    hb.FieldText(
                        controlId: "BackgroundJobs_JobType",
                        fieldCss: "field-normal",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsJobType"),
                        text: jobTypeLabel);
                    hb.FieldText(
                        controlId: "BackgroundJobs_Status",
                        fieldCss: "field-normal",
                        labelText: Displays.Status(context: context),
                        text: statusLabel);
                    hb.FieldText(
                        controlId: "BackgroundJobs_SiteId",
                        fieldCss: "field-normal both",
                        labelText: Displays.SiteId(context: context),
                        text: model.SiteId.ToString());
                    hb.FieldText(
                        controlId: "BackgroundJobs_SiteName",
                        fieldCss: "field-normal",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsSiteName"),
                        text: siteName);
                    hb.FieldText(
                        controlId: "BackgroundJobs_User",
                        fieldCss: "field-normal",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsRegisteredUser"),
                        text: userName);
                    hb.FieldText(
                        controlId: "BackgroundJobs_JobEnqueuedTime",
                        fieldCss: "field-normal both",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsJobEnqueuedTime"),
                        text: model.JobEnqueuedTime
                            .ToBackgroundJobLocalTime(context: context).InRange()
                                ? model.JobEnqueuedTime
                                    .ToBackgroundJobLocalTime(context: context)
                                    .ToString("yyyy/MM/dd HH:mm")
                                : string.Empty);
                    hb.FieldText(
                        controlId: "BackgroundJobs_JobStartedTime",
                        fieldCss: "field-normal",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsJobStartedTime"),
                        text: model.JobStartedTime
                            .ToBackgroundJobLocalTime(context: context).InRange()
                                ? model.JobStartedTime
                                    .ToBackgroundJobLocalTime(context: context)
                                    .ToString("yyyy/MM/dd HH:mm")
                                : string.Empty);
                    hb.FieldText(
                        controlId: "BackgroundJobs_JobFinishedTime",
                        fieldCss: "field-normal",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsJobFinishedTime"),
                        text: model.JobFinishedTime
                            .ToBackgroundJobLocalTime(context: context).InRange()
                                ? model.JobFinishedTime
                                    .ToBackgroundJobLocalTime(context: context)
                                    .ToString("yyyy/MM/dd HH:mm")
                                : string.Empty);
                    hb.FieldText(
                        controlId: "BackgroundJobs_ResultMessage",
                        fieldCss: "field-wide",
                        labelText: Displays.Get(
                            context: context,
                            id: "BackgroundJobsResultMessage"),
                        text: model.ResultMessage ?? string.Empty);
                    var fileText = string.Empty;
                    if (canDownload)
                    {
                        try
                        {
                            var fileInfo = new System.IO.FileInfo(model.File);
                            var fileSizeText = FormatFileSize(bytes: fileInfo.Length);
                            fileText = $"{fileInfo.Name}　({fileSizeText})";
                        }
                        catch (Exception e)
                        {
                            new SysLogModel(context: context, e: e);
                        }
                    }
                    if (fileText.IsNullOrEmpty() == false)
                    {
                        var downloadUrl = Locations.Action(
                            context: context,
                            controller: "BackgroundJobs",
                            id: model.BackgroundJobId)
                            .Replace("_action_", "download");
                        hb.Field(
                            fieldId: "BackgroundJobs_FileField",
                            fieldCss: "field-wide",
                            labelText: Displays.Get(
                                context: context,
                                id: "File"),
                            controlAction: () => hb.Div(
                                css: "control-attachments-items",
                                action: () => hb.Div(
                                    css: "control-attachments-item already-attachments",
                                    action: () => hb.A(
                                        attributes: new HtmlAttributes()
                                            .Class("file-name")
                                            .Href(downloadUrl),
                                        action: () => hb.Text(
                                            text: fileText)))));
                    }
                    else
                    {
                        hb.FieldText(
                            controlId: "BackgroundJobs_File",
                            fieldCss: "field-wide",
                            labelText: Displays.Get(
                                context: context,
                                id: "File"),
                            text: model.Status
                                == BackgroundJobStatus.Completed
                                    ? Displays.Get(
                                        context: context,
                                        id: "BackgroundJobsNoDownloadableFile")
                                    : string.Empty);
                    }
                });
        }

        public static string BackgroundJobsEditorJson(
            Context context,
            long backgroundJobId)
        {
            var model = BackgroundJobQueue.Get(
                context: context,
                backgroundJobId: backgroundJobId);
            var errorData = BackgroundJobAccessValidator.OnEditing(
                context: context,
                model: model);
            if (errorData.Type != Error.Types.None)
            {
                return errorData.Type.MessageJson(context: context);
            }
            var hb = new HtmlBuilder();
            return new ResponseCollection(context: context)
                .ReplaceAll(
                    "#MainContainer",
                    hb.Template(
                        context: context,
                        ss: new SiteSettings(),
                        view: null,
                        referenceType: "BackgroundJobs",
                        title: Displays.BackgroundJobs(context: context)
                            + " - "
                            + model.BackgroundJobId.ToString(),
                        useNavigationMenu: true,
                        action: () => hb.Editor(
                            context: context,
                            model: model)))
                .ToJson();
        }

    }
}
