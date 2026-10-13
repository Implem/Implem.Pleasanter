using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public class HistoryDiffField
    {
        public string ColumnName;
        public string LabelText;
        public string OldValue;
        public string NewValue;
        public string OldSourceValue;
        public string NewSourceValue;
        public bool Switchable;

        public bool Changed
        {
            get
            {
                return OldValue != NewValue
                    || OldSourceValue != NewSourceValue;
            }
        }
    }

    public class HistoryDiffTab
    {
        public string Id;
        public string LabelText;
        public List<HistoryDiffField> Fields = new List<HistoryDiffField>();

        public bool Changed
        {
            get { return Fields.Any(diffField => diffField.Changed); }
        }
    }

    public static class HtmlHistoryDiff
    {
        private static readonly Regex TagRegex = new Regex(
            @"<(?:[^>""']|""[^""]*""|'[^']*')*>",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex HtmlLineBreakRegex = new Regex(
            @"<br\s*/?>|</(?:p|div|li|h[1-6]|tr|blockquote|pre)\s*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex HtmlCellBreakRegex = new Regex(
            @"</t[dh]\s*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex HtmlImageRegex = new Regex(
            @"<img\b(?<attributes>(?:[^>""']|""[^""]*""|'[^']*')*)>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex HtmlSrcRegex = new Regex(
            @"\bsrc\s*=\s*(?:""(?<value>[^""]*)""|'(?<value>[^']*)')",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex HtmlAltRegex = new Regex(
            @"\balt\s*=\s*(?:""(?<value>[^""]*)""|'(?<value>[^']*)')",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex TrailingCellBreakRegex = new Regex(
            @"\t+(?=\n|$)",
            RegexOptions.Compiled);

        private static readonly Regex ExcessiveLineBreakRegex = new Regex(
            @"\n{3,}",
            RegexOptions.Compiled);

        private static readonly Regex MarkDownFencedCodeRegex = new Regex(
            @"^[ \t]*(?<fence>```+|~~~+)[^\n]*\n(?<code>[\s\S]*?)\n[ \t]*\k<fence>[ \t]*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex MarkDownInlineCodeRegex = new Regex(
            @"(?<fence>`+)(?<code>.+?)\k<fence>",
            RegexOptions.Compiled);

        private const string CodePlaceholderStart = "\u0001";
        private const string CodePlaceholderEnd = "\u0002";

        private static readonly Regex CodePlaceholderRegex = new Regex(
            CodePlaceholderStart + @"(?<index>\d+)" + CodePlaceholderEnd,
            RegexOptions.Compiled);

        private static readonly Regex[] MarkDownLineRegexes = new[]
        {
            new Regex(@"^[ \t]{0,3}(?:([-*_])[ \t]*){3,}$", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^[ \t]{0,3}#{1,6}[ \t]+", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^(?:[ \t]{0,3}>[ \t]?)+", RegexOptions.Compiled | RegexOptions.Multiline),
            new Regex(@"^[ \t]*(?:[-*+]|\d+[.)])[ \t]+", RegexOptions.Compiled | RegexOptions.Multiline)
        };

        private const string Inline = @"(?:(?!\n\n)[\s\S])+?";

        private static readonly (Regex Regex, string Replacement)[] MarkDownInlineRegexes = new[]
        {
            (new Regex(
                @"(?<!!)\[(?<text>(?:!\[[^\]]*\]\([^)]*\)|[^\[\]])*)\]\((?:[^()]|\([^()]*\))*\)",
                RegexOptions.Compiled), "${text}"),
            (new Regex(@"\*\*\*(?=[^\s*])(" + Inline + @")(?<=[^\s*])\*\*\*", RegexOptions.Compiled), "$1"),
            (new Regex(@"(?<!\*)\*\*(?=[^\s*])(" + Inline + @")(?<=[^\s*])\*\*(?!\*)", RegexOptions.Compiled), "$1"),
            (new Regex(@"(?<!\w)__(?=[^\s_])(" + Inline + @")(?<=[^\s_])__(?!\w)", RegexOptions.Compiled), "$1"),
            (new Regex(@"(?<!\*)\*(?=[^\s*])(" + Inline + @")(?<=[^\s*])\*(?!\*)", RegexOptions.Compiled), "$1"),
            (new Regex(@"(?<!\w)_(?=[^\s_])(" + Inline + @")(?<=[^\s_])_(?!\w)", RegexOptions.Compiled), "$1"),
            (new Regex(@"~~(?=[^\s~])(" + Inline + @")(?<=[^\s~])~~", RegexOptions.Compiled), "$1")
        };

        public enum MarkupTypes
        {
            None,
            MarkDown,
            Html
        }

        public static MarkupTypes MarkupType(Column column)
        {
            if (column == null)
            {
                return MarkupTypes.None;
            }
            if (column.TypeCs == "Comments")
            {
                return MarkupTypes.MarkDown;
            }
            switch (column.FieldCss)
            {
                case "field-rte":
                    return MarkupTypes.Html;
                case "field-markdown":
                    return MarkupTypes.MarkDown;
                default:
                    return MarkupTypes.None;
            }
        }

        public static bool Switchable(Column column)
        {
            return MarkupType(column: column) != MarkupTypes.None;
        }

        public static readonly string[] ExcludedColumnNames = new[]
        {
            "Ver",
            "Updator",
            "UpdatedTime",
            "Password",
            "PasswordValidate",
            "PasswordDummy"
        };

        public static HtmlBuilder HistoryDiffNoPreviousVersion(
            this HtmlBuilder hb,
            Context context)
        {
            return hb.Div(css: "tabs-panel-inner", action: () => hb
                .Div(css: "history-diff", action: () => hb
                    .P(css: "history-diff-empty", action: () => hb
                        .Text(text: Displays.NoPreviousVersion(context: context)))));
        }

        public static HtmlBuilder HistoryDiff(
            this HtmlBuilder hb,
            Context context,
            Dictionary<string, ControlData> versionOptions,
            int oldVer,
            int newVer,
            string oldVerLabelText,
            string newVerLabelText,
            List<HistoryDiffTab> tabs,
            bool invalidVerOrder)
        {
            return hb.Div(css: "tabs-panel-inner", action: () => hb
                .Div(
                    attributes: new HtmlAttributes()
                        .Class("history-diff")
                        .Add(
                            "data-enablelightbox",
                            Implem.DefinitionAccessor.Parameters.General.EnableLightBox
                                ? "1"
                                : "0"),
                    action: () => hb
                    .HistoryDiffToolbar(
                        context: context,
                        tabs: tabs,
                        _using: !invalidVerOrder)
                    .Div(css: "history-diff-panes", action: () => hb
                        .HistoryDiffPane(
                            context: context,
                            side: "old",
                            controlId: "HistoryDiffOldVer",
                            selectorLabelText: Displays.OldVersion(context: context),
                            versionOptions: versionOptions,
                            selectedVer: oldVer,
                            verLabelText: oldVerLabelText,
                            tabs: tabs,
                            showSections: !invalidVerOrder)
                        .HistoryDiffPane(
                            context: context,
                            side: "new",
                            controlId: "HistoryDiffNewVer",
                            selectorLabelText: Displays.NewVersion(context: context),
                            versionOptions: versionOptions,
                            selectedVer: newVer,
                            verLabelText: newVerLabelText,
                            tabs: tabs,
                            showSections: !invalidVerOrder))));
        }

        private static HtmlBuilder HistoryDiffToolbar(
            this HtmlBuilder hb,
            Context context,
            List<HistoryDiffTab> tabs,
            bool _using)
        {
            if (!_using)
            {
                return hb;
            }
            var changedCount = tabs.Sum(tab => tab.Fields.Count(diffField => diffField.Changed));
            return hb.Div(id: "HistoryDiffToolbar", action: () => hb
                .FieldCheckBox(
                    fieldId: "HistoryDiffChangedOnlyField",
                    controlId: "HistoryDiffChangedOnly",
                    fieldCss: "field-auto-thin",
                    controlCss: " not-set-form-changed",
                    labelText: Displays.ShowChangedColumnsOnly(
                        context: context,
                        data: changedCount.ToString()),
                    _checked: context.Forms.Exists("HistoryDiffChangedOnly")
                        ? context.Forms.Bool("HistoryDiffChangedOnly")
                        : true,
                    alwaysSend: true));
        }

        private static HtmlBuilder HistoryDiffPane(
            this HtmlBuilder hb,
            Context context,
            string side,
            string controlId,
            string selectorLabelText,
            Dictionary<string, ControlData> versionOptions,
            int selectedVer,
            string verLabelText,
            List<HistoryDiffTab> tabs,
            bool showSections)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("history-diff-pane")
                    .Add("data-side", side),
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        fieldId: controlId + "Field",
                        controlId: controlId,
                        fieldCss: "history-diff-pane-ver",
                        controlCss: " auto-postback always-send not-set-form-changed",
                        labelText: selectorLabelText,
                        optionCollection: versionOptions,
                        selectedValue: selectedVer.ToString(),
                        alwaysSend: true,
                        action: "HistoryDiff",
                        method: "post")
                    .Div(css: "history-diff-pane-ver-detail", action: () => hb
                        .Text(text: verLabelText))
                    .Div(
                        css: "history-diff-sections",
                        _using: showSections,
                        action: () => tabs.ForEach(tab => hb
                            .HistoryDiffSection(
                                context: context,
                                side: side,
                                tab: tab,
                                showHeader: tabs.Count > 1))));
        }

        private static HtmlBuilder HistoryDiffSectionHeader(
            this HtmlBuilder hb,
            string labelText)
        {
            return hb.Ul(
                css: "history-diff-section-header ui-tabs-nav "
                    + "ui-helper-reset ui-helper-clearfix",
                action: () => hb
                    .Li(
                        css: "ui-tabs-tab ui-corner-top ui-tab ui-state-default",
                        action: () => hb
                            .Span(css: "ui-tabs-anchor", action: () => hb
                                .Text(text: labelText))));
        }

        private static HtmlBuilder HistoryDiffSection(
            this HtmlBuilder hb,
            Context context,
            string side,
            HistoryDiffTab tab,
            bool showHeader)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("history-diff-section")
                    .Add("data-tab", tab.Id)
                    .Add("data-changed", tab.Changed
                        ? "1"
                        : "0"),
                action: () =>
                {
                    if (showHeader)
                    {
                        hb.HistoryDiffSectionHeader(labelText: tab.LabelText);
                    }
                    if (tab.Fields.Count == 0)
                    {
                        hb.P(css: "history-diff-empty", action: () => hb
                            .Text(text: Displays.NoChangedColumns(context: context)));
                        return;
                    }
                    hb.Table(css: "grid not-link history-diff-fields", action: () => hb
                        .THead(action: () => hb
                            .Tr(css: "ui-widget-header", action: () => hb
                                .Th(css: "history-diff-label", action: () => hb
                                    .Text(text: Displays.Column(context: context)))
                                .Th(css: "history-diff-value", action: () => hb
                                    .Text(text: Displays.Value(context: context)))))
                        .TBody(action: () => tab.Fields.ForEach(diffField => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history-diff-row"
                                        + (diffField.Changed
                                            ? " changed"
                                            : string.Empty))
                                    .Add("data-column", diffField.ColumnName),
                                action: () => hb
                                    .Th(css: "history-diff-label", action: () => hb
                                        .Div(css: "history-diff-label-inner", action: () => hb
                                            .Span(css: "history-diff-label-text", action: () => hb
                                                .Text(text: diffField.LabelText))
                                            .HistoryDiffTools(
                                                context: context,
                                                switchable: diffField.Switchable)))
                                    .Td(css: "history-diff-value", action: () => hb
                                        .HistoryDiffValue(
                                            mode: "rendered",
                                            value: side == "old"
                                                ? diffField.OldValue
                                                : diffField.NewValue)
                                        .HistoryDiffValue(
                                            mode: "source",
                                            value: side == "old"
                                                ? diffField.OldSourceValue
                                                : diffField.NewSourceValue,
                                            _using: diffField.Switchable))))));
                });
        }

        private static HtmlBuilder HistoryDiffTools(
            this HtmlBuilder hb,
            Context context,
            bool switchable)
        {
            return hb.Span(css: "history-diff-label-tools", action: () => hb
                .Button(
                    attributes: new HtmlAttributes()
                        .Class("history-diff-tool history-diff-pin")
                        .Type("button")
                        .Title(Displays.PinExpanded(context: context)),
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-pin-w"))
                .Button(
                    attributes: new HtmlAttributes()
                        .Class("history-diff-tool history-diff-source-toggle")
                        .Type("button")
                        .Title(Displays.CompareSource(context: context)),
                    _using: switchable,
                    action: () => hb
                        .Span(css: "ui-icon ui-icon-script")));
        }

        private static HtmlBuilder HistoryDiffValue(
            this HtmlBuilder hb,
            string mode,
            string value,
            bool _using = true)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("history-diff-value-inner")
                    .Add("data-mode", mode),
                _using: _using,
                action: () => hb.Text(text: value));
        }

        public static string ToDiffText(string value, Column column, bool rendered)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            if (!rendered)
            {
                return value;
            }
            switch (MarkupType(column: column))
            {
                case MarkupTypes.Html:
                    return ToHtmlText(value: value);
                case MarkupTypes.MarkDown:
                    return ToMarkDownText(value: value);
                default:
                    return value;
            }
        }

        private static string ToHtmlText(string value)
        {
            value = value.Replace("\r\n", "\n");
            value = HtmlImageRegex.Replace(value, match =>
            {
                var attributes = match.Groups["attributes"].Value;
                var src = HtmlSrcRegex.Match(attributes).Groups["value"].Value;
                var alt = HtmlAltRegex.Match(attributes).Groups["value"].Value;
                return "![" + (alt.IsNullOrEmpty() ? "image" : alt) + "](" + src + ")";
            });
            value = HtmlLineBreakRegex.Replace(value, "\n");
            value = HtmlCellBreakRegex.Replace(value, "\t");
            value = TagRegex.Replace(value, string.Empty);
            value = TrailingCellBreakRegex.Replace(value, string.Empty);
            value = ExcessiveLineBreakRegex.Replace(value, "\n\n");
            value = value.Replace("&nbsp;", " ");
            return System.Net.WebUtility.HtmlDecode(value).Trim();
        }

        private static string ToMarkDownText(string value)
        {
            value = value.Replace("\r\n", "\n");
            var codes = new List<string>();
            value = MarkDownFencedCodeRegex.Replace(value, match =>
                StashCode(codes: codes, code: match.Groups["code"].Value));
            value = MarkDownInlineCodeRegex.Replace(value, match =>
                StashCode(codes: codes, code: match.Groups["code"].Value));
            foreach (var regex in MarkDownLineRegexes)
            {
                value = regex.Replace(value, string.Empty);
            }
            foreach (var inline in MarkDownInlineRegexes)
            {
                value = inline.Regex.Replace(value, inline.Replacement);
            }
            value = CodePlaceholderRegex.Replace(value, match =>
                codes[match.Groups["index"].Value.ToInt()]);
            return value.Trim();
        }

        private static string StashCode(List<string> codes, string code)
        {
            codes.Add(code);
            return CodePlaceholderStart + (codes.Count - 1) + CodePlaceholderEnd;
        }
    }
}
