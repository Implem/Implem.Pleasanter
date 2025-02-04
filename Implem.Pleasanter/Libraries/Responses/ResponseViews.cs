using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViews
    {
        public static ResponseCollection View(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            View view)
        {
            return res
                .ViewFilters(
                    context: context,
                    ss: ss,
                    view: view)
                .ClearFormData(
                    target: "View",
                    type: "startsWith",
                    _using: ss.SaveViewType != SiteSettings.SaveViewTypes.None);
        }

        private static ResponseCollection ViewFilters(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            View view)
        {
            switch (context.Forms.ControlId())
            {
                case "ViewSelector":
                case "ViewFilters_Reset":
                case "ReduceViewFilters":
                case "ExpandViewFilters":
                    return res
                        .ReplaceAll("#ViewFilters",
                            new HtmlBuilder().ViewFilters(
                                context: context,
                                ss: ss,
                                view: view))
                        .ReplaceAll("#ShowHistoryField",
                            new HtmlBuilder().FieldCheckBox(
                                fieldId: "ShowHistoryField",
                                fieldCss: "field-auto-thin",
                                controlId: "ViewFilters_ShowHistory",
                                controlCss: " auto-postback",
                                method: "post",
                                _checked: view.ShowHistory == true,
                                labelText: Displays.ShowHistory(context: context),
                                _using: ss.HistoryOnGrid == true));
                // 一括処理のプルダウンから有効なプロセスを選択したときフィルタを無効にするため非表示にする。一覧に戻るアクションもプルダウンのリセット操作であるためここで統一的に判定できる。
                case "BulkProcessingItems":
                    var processId = context.Forms.Int("BulkProcessingItems");
                    var process = ss.GetProcess(
                        context: context,
                        id: processId);
                    return process == null
                        ? res.ReplaceAll("#ViewFilters",
                            new HtmlBuilder().ViewFilters(
                                context: context,
                                ss: ss,
                                view: view))
                        : res.ReplaceAll("#ViewFilters",
                            new HtmlBuilder().Div(
                                id: "ViewFilters",
                                css: "always-hidden"));
                default:
                    return res;
            }
        }

        public static ResponseCollection FilterField(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            View view,
            string controlId,
            string prefix)
        {
            if (context.Forms.ContainsKey(controlId))
            {
                var filterName = context.Forms.Data(controlId);
                if (!filterName.Contains(prefix))
                {
                    if (filterName != null)
                    {
                        var hb = new HtmlBuilder();
                        switch (filterName)
                        {
                            case "ViewFilters_Incomplete":
                                HtmlViewFilters.Incomplete(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    view: view);
                                res.ReplaceAll($"[id=\"ViewFilters_IncompleteField\"]", hb);
                                break;
                            case "ViewFilters_Own":
                                HtmlViewFilters.Own(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    view: view);
                                res.ReplaceAll($"[id=\"ViewFilters_OwnField\"]", hb);
                                break;
                            case "ViewFilters_NearCompletionTime":
                                HtmlViewFilters.NearCompletionTime(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    view: view);
                                res.ReplaceAll($"[id=\"ViewFilters_NearCompletionTimeField\"]", hb);
                                break;
                            case "ViewFilters_Delay":
                                HtmlViewFilters.Delay(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    view: view);
                                res.ReplaceAll($"[id=\"ViewFilters_DelayField\"]", hb);
                                break;
                            case "ViewFilters_Overdue":
                                HtmlViewFilters.Limit(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    view: view);
                                res.ReplaceAll($"[id=\"ViewFilters_OverdueField\"]", hb);
                                break;
                            case "ViewFilters_Search":
                                HtmlViewFilters.Search(
                                    hb: hb,
                                    context: context,
                                    ss: ss,
                                    view: view);
                                res.ReplaceAll($"[id=\"ViewFilters_SearchField\"]", hb);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    filterName = filterName.Substring(prefix.Length);
                    filterName = filterName.Replace("_NumericRange", string.Empty);
                    filterName = filterName.Replace("_DateRange", string.Empty);
                    var column = ss.GetColumn(
                        context: context,
                        columnName: filterName);
                    if (column != null)
                    {
                        var hb = new HtmlBuilder();
                        string ReplaceColumnName = column.ColumnName;
                        switch (column.DateFilterSetMode.ToString())
                        {
                            case "Range":
                                switch (column.TypeName)
                                {
                                    case "decimal":
                                        ReplaceColumnName += "_NumericRange";
                                        break;
                                    case "datetime":
                                        ReplaceColumnName += "_DateRange";
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        HtmlViewFilters.Column(
                            hb: hb,
                            context: context,
                            ss: ss,
                            view: view,
                            column: column);
                        res.ReplaceAll($"[id=\"ViewFilters__{ReplaceColumnName}Field\"]", hb);
                    }
                }
            }
            return res;
        }
    }
}