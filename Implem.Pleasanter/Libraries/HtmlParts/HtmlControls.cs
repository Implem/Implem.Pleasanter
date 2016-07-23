using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlControls
    {
        public static HtmlBuilder TextBox(
            this HtmlBuilder hb,
            HtmlTypes.TextTypes textType = HtmlTypes.TextTypes.Normal,
            string controlId = "",
            string controlCss = "",
            string text = "",
            string placeholder = "",
            string onChange = "",
            string action = "",
            string method = "",
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            if (!_using) return hb;
            switch (textType)
            {
                case HtmlTypes.TextTypes.Normal:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id_Css(controlId, Css.Class("control-textbox", controlCss))
                        .Type("text")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .DataAction(action)
                        .DataMethod(method)
                        .Add(attributes));
                case HtmlTypes.TextTypes.MultiLine:
                    return hb.TextArea(
                        attributes: new HtmlAttributes()
                            .Id_Css(controlId, Css.Class("control-textarea", controlCss))
                            .Placeholder(placeholder)
                            .OnChange(onChange)
                            .DataAction(action)
                            .DataMethod(method)
                            .Add(attributes),
                        action: () => hb
                            .Text(text: text));
                case HtmlTypes.TextTypes.Password:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id_Css(controlId, Css.Class("control-textbox", controlCss))
                        .Type("password")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .Add(attributes));
                case HtmlTypes.TextTypes.File:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id_Css(controlId, Css.Class("control-textbox", controlCss))
                        .Type("file")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .DataAction(action)
                        .DataMethod(method)
                        .Add(attributes));
                default:
                    return hb;
            }
        }

        public static HtmlBuilder MarkDown(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string text = "",
            string placeholder = "",
            bool readOnly = false,
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            return _using
                ? hb
                    .Div(attributes: new HtmlAttributes()
                        .Id_Css(controlId + ".viewer", "control-markup")
                        .OnDblClick(Def.JavaScript.EditMarkDown))
                    .Div(
                        attributes: new HtmlAttributes()
                            .Id_Css(
                                controlId + ".editor",
                                "ui-icon ui-icon-pencil button-edit-markdown")
                            .OnClick(Def.JavaScript.EditMarkDown),
                        _using: !readOnly)
                    .TextArea(
                        attributes: new HtmlAttributes()
                            .Id_Css(
                                controlId,
                                Css.Class("control-markdown upload-image", controlCss))
                            .Placeholder(placeholder)
                            .Add(attributes),
                        action: () => hb
                            .Text(text: text))
                : hb;
        }

        public static HtmlBuilder MarkUp(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string text = "",
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id_Css(
                            controlId,
                            Css.Class("control-markup markup", controlCss)),
                    action: () => hb
                        .Text(text: text))
                : hb;
        }

        public static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            bool insertBlank = false,
            string onChange = "",
            string action = "",
            string method = "",
            Column column = null,
            bool _using = true)
        {
            return _using
                ? hb.Select(
                    attributes: new HtmlAttributes()
                        .Id_Css(controlId, Css.Class("control-dropdown", controlCss))
                        .OnChange(onChange)
                        .DataAction(action)
                        .DataMethod(method),
                    action: () => hb
                        .OptionCollection(
                            optionCollection: optionCollection,
                            selectedValue: selectedValue,
                            addSelectedValue: addSelectedValue,
                            insertBlank: insertBlank,
                            column: column))
                : hb;
        }

        public static HtmlBuilder OptionCollection(
            this HtmlBuilder hb,
            Dictionary<string, string> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            bool insertBlank = false,
            Column column = null,
            bool _using = true)
        {
            if (_using)
            {
                OptionCollection(
                    optionCollection: optionCollection?
                        .ToDictionary(o => o.Key, o => new ControlData(o.Value)),
                    selectedValue: selectedValue,
                    addSelectedValue: addSelectedValue,
                    insertBlank: insertBlank,
                    column: column)?
                        .ForEach(htmlData => hb.Option(
                            attributes: new HtmlAttributes()
                                .Value(htmlData.Key)
                                .DataClass(htmlData.Value.Css)
                                .DataStyle(htmlData.Value.Style)
                                .Selected(selectedValue == htmlData.Key),
                            action: () => hb
                                .Text(text: Strings.CoalesceEmpty(
                                    htmlData.Value.Text, htmlData.Key))));
            }
            return hb;
        }

        public static HtmlBuilder OptionCollection(
            this HtmlBuilder hb,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            bool insertBlank = false,
            Column column = null,
            bool _using = true)
        {
            if (_using)
            {
                OptionCollection(
                    optionCollection: optionCollection,
                    selectedValue: selectedValue,
                    addSelectedValue: addSelectedValue,
                    insertBlank: insertBlank,
                    column: column)?
                        .ForEach(htmlData => hb.Option(
                            attributes: new HtmlAttributes()
                                .Value(htmlData.Key)
                                .DataClass(htmlData.Value.Css)
                                .DataStyle(htmlData.Value.Style)
                                .Selected(selectedValue == htmlData.Key),
                            action: () => hb
                                .Text(text: Strings.CoalesceEmpty(
                                    htmlData.Value.Text, htmlData.Key))));
            }
            return hb;
        }

        private static Dictionary<string, ControlData> OptionCollection(
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            bool insertBlank = false,
            Column column = null)
        {
            if (insertBlank)
            {
                optionCollection = InsertBalnk(optionCollection);
            }
            if (selectedValue.IsNullOrEmpty() || 
                optionCollection.ContainsKey(selectedValue) ||
                selectedValue == "0" ||
                !addSelectedValue)
            {
                return optionCollection;
            }
            else
            {
                var userId = selectedValue.ToInt();
                optionCollection.Add(
                    selectedValue,
                    column != null && column.UserColumn
                        ? new ControlData(SiteInfo.UserFullName(userId))
                        : new ControlData("? " + selectedValue));
                return optionCollection;
            }
        }

        private static Dictionary<string, ControlData> InsertBalnk(
            Dictionary<string, ControlData> optionCollection)
        {
            return new Dictionary<string, ControlData>
            {
                { string.Empty, new ControlData(string.Empty) }
            }.AddRange(optionCollection);
        }

        public static HtmlBuilder RadioButtons(
            this HtmlBuilder hb,
            string name = "",
            string controlCss = "",
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool _using = true)
        {
            if (_using)
            {
                optionCollection
                    .Select((o, i) => new { Option = o, Index = i })
                    .ForEach(data => hb
                        .Input(new HtmlAttributes()
                            .Id(name, data.Index)
                            .Class(Css.Class("control-radio", controlCss))
                            .Type("radio")
                            .Value(data.Option.Key)
                            .Checked(data.Option.Key == selectedValue))
                        .Label(
                            attributes: new HtmlAttributes().For(name + data.Index),
                            action: () => hb
                                .Text(data.Option.Value.Text)));
            }
            return hb;
        }

        public static HtmlBuilder Spinner(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            decimal? value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            int width = -1,
            string onChange = "",
            string action = "",
            string method = "",
            bool _using = true)
        {
            return _using
                ? hb.Input(new HtmlAttributes()
                    .Id_Css(controlId, Css.Class("control-spinner", controlCss))
                    .Type("number")
                    .Value(value != null
                        ? value.ToString()
                        : string.Empty)
                    .Min(min)
                    .Max(max)
                    .Step(step)
                    .DataWidth(width)
                    .OnChange(onChange)
                    .DataAction(action)
                    .DataMethod(method))
                : hb;
        }

        public static HtmlBuilder CheckBox(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string labelText = "",
            bool _checked = false,
            bool disabled = false,
            string dataId = "",
            string action = "",
            string method = "",
            bool _using = true)
        {
            if (_using)
            {
                hb.Input(attributes: new HtmlAttributes()
                    .Id_Css(controlId, Css.Class("control-checkbox", controlCss))
                    .Type("checkbox")
                    .Disabled(disabled)
                    .DataId(dataId)
                    .DataAction(action)
                    .DataMethod(method)
                    .Checked(_checked));
                if (labelText != string.Empty)
                {
                    hb.Label(
                        attributes: new HtmlAttributes().For(controlId),
                        action: () => hb
                            .Text(text: labelText));
                }
            }
            return hb;
        }

        public static HtmlBuilder Slider(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string value = "",
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            string unit = "",
            string action = "",
            string method = "",
            bool _using = true)
        {
            return _using
                ? hb
                    .Div(attributes: new HtmlAttributes()
                        .Id_Css(controlId + ",ui", "control-slider-ui")
                        .Min(min)
                        .Max(max)
                        .Step(step))
                    .P(
                        attributes: new HtmlAttributes()
                            .Class(Css.Class("control-slider", controlCss)),
                        action: () => hb
                            .Span(
                                attributes: new HtmlAttributes()
                                    .Id(controlId).DataAction(action)
                                    .DataAction(action)
                                    .DataMethod(method),
                                action: () => hb
                                    .Text(value))
                            .Span(action: () => hb
                                .Text(unit)))
                : hb;
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            string controlId = "",
            string text = "",
            string controlCss = "",
            string accessKey = "",
            string onClick = "",
            string href = "",
            string dataId = "",
            string selector = "",
            string action = "",
            string method = "",
            string confirm = "",
            string type = "button",
            bool _using = true)
        {
            return _using
                ? hb.Button(
                    attributes: new HtmlAttributes()
                        .Id_Css(controlId, "button " + controlCss)
                        .Type(type)
                        .AccessKey(accessKey)
                        .OnClick(onClick + href.IsNotEmpty("location.href='" + href + "';"))
                        .DataId(dataId)
                        .DataSelector(selector)
                        .DataAction(action)
                        .DataMethod(method)
                        .DataConfirm(confirm),
                    action: () => hb
                        .Text(text: text))
                : hb;
        }

        public static HtmlBuilder Anchor(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string text = "",
            string href = "",
            bool _using = true)
        {
            return _using
                ? hb.A(
                    id: controlId,
                    css: Css.Class("control-anchor", controlCss),
                    href: href,
                    text: text)
                : hb;
        }

        public static HtmlBuilder Icon(
            this HtmlBuilder hb,
            string iconCss = "",
            string cssText = "",
            string text = "",
            bool _using = true)
        {
            return _using
                ? hb
                    .Span(css: "ui-icon " + iconCss)
                    .Span(css: cssText, _using: text != string.Empty, action: () => hb
                        .Text(text:text))
                : hb;
        }

        public static HtmlBuilder Hidden(
            this HtmlBuilder hb,
            string controlId = "",
            string css = "",
            string value = "",
            bool _using = true)
        {
            return _using
                ? hb.Input(attributes: new HtmlAttributes()
                    .Id(controlId)
                    .Class(css)
                    .Type("hidden")
                    .Value(value))
                : hb;
        }

        public static HtmlBuilder Hidden(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Input(attributes: attributes.Type("hidden"))
                : hb;
        }

        public static HtmlBuilder Selectable(
            this HtmlBuilder hb,
            string controlId,
            string controlCss = "",
            Dictionary<string, string> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    css: Css.Class("control", controlCss),
                    action: () => hb
                        .Ol(
                            attributes: new HtmlAttributes()
                                .Id_Css(controlId, "control-selectable"),
                            action: () => hb
                                .SelectableItems(
                                    listItemCollection: listItemCollection,
                                    selectedValueTextCollection: selectedValueCollection)))
                : hb;
        }

        public static HtmlBuilder Basket(
            this HtmlBuilder hb,
            string controlId,
            string controlCss = "",
            Dictionary<string, string> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool _using = true)
        {
            return _using
                ? hb.Ol(
                    attributes: new HtmlAttributes()
                        .Id_Css(controlId, Css.Class("control-basket", controlCss)),
                    action: () => hb
                        .SelectableItems(
                            listItemCollection: listItemCollection,
                            selectedValueTextCollection: selectedValueCollection))
                : hb;
        }

        public static HtmlBuilder SelectableItems(
            this HtmlBuilder hb,
            Dictionary<string, string> listItemCollection = null,
            IEnumerable<string> selectedValueTextCollection = null,
            bool _using = true)
        {
            if (_using)
            {
                selectedValueTextCollection = selectedValueTextCollection ?? new List<string>();
                listItemCollection.ForEach(listItem => hb
                    .Li(
                        attributes: new HtmlAttributes()
                            .Class(
                                selectedValueTextCollection.Contains(listItem.Key)
                                    ? "ui-widget-content ui-selected"
                                    : "ui-widget-content")
                            .Value( listItem.Key),
                        action: () => hb
                            .Text(text: listItem.Value)));
            }
            return hb;
        }
    }
}
