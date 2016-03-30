using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlControls
    {

        public enum TextStyles
        {
            Normal,
            MultiLine,
            Password,
            File
        }

        public static HtmlBuilder TextBox(
            this HtmlBuilder hb,
            TextStyles textStyle = TextStyles.Normal,
            string controlId = "",
            string controlCss = "",
            string text = "",
            string placeholder = "",
            string onChange = "",
            string action = "",
            string method = "",
            Dictionary<string, string> attributes = null)
        {
            switch (textStyle)
            {
                case TextStyles.Normal:
                    return hb.Input(attributes: Html.Attributes()
                        .Id_Css(controlId, CssClasses.Get("control-textbox", controlCss))
                        .Type("text")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .DataAction(action)
                        .DataMethod(method)
                        .Add(attributes));
                case TextStyles.MultiLine:
                    return hb.TextArea(
                        attributes: Html.Attributes()
                            .Id_Css(controlId, CssClasses.Get("control-textarea", controlCss))
                            .Placeholder(placeholder)
                            .OnChange(onChange)
                            .DataAction(action)
                            .DataMethod(method)
                            .Add(attributes),
                        action: () => hb
                            .Text(text: text));
                case TextStyles.Password:
                    return hb.Input(attributes: Html.Attributes()
                        .Id_Css(controlId, CssClasses.Get("control-textbox", controlCss))
                        .Type("password")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .Add(attributes));
                case TextStyles.File:
                    return hb.Input(attributes: Html.Attributes()
                        .Id_Css(controlId, CssClasses.Get("control-textbox", controlCss))
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
            Dictionary<string, string> attributes = null)
        {
            return hb
                .Div(attributes: Html.Attributes()
                    .Id_Css(controlId + ".viewer", "control-markup")
                    .OnDblClick(Def.JavaScript.EditMarkDown))
                .Div(attributes: Html.Attributes()
                    .Id_Css(
                        controlId + ".edit",
                        "ui-icon ui-icon-pencil button-edit-markdown")
                    .OnClick(Def.JavaScript.EditMarkDown))
                .TextArea(
                    attributes: Html.Attributes()
                        .Id_Css(
                            controlId,
                            CssClasses.Get("control-markdown upload-image", controlCss))
                        .Placeholder(placeholder)
                        .Add(attributes),
                    action: () => hb
                        .Text(text: text));
        }

        public static HtmlBuilder MarkUp(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string text = "",
            Dictionary<string, string> attributes = null)
        {
            return hb.Div(
                attributes: Html.Attributes()
                    .Id_Css(
                        controlId,
                        CssClasses.Get("control-markup markup", controlCss)),
                action: () => hb
                    .Text(text: text));
        }

        public static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            string onChange = "",
            string action = "",
            string method = "",
            Column column = null)
        {
            return hb.Select(
                attributes: Html.Attributes()
                    .Id_Css(controlId, CssClasses.Get("control-dropdown", controlCss))
                    .OnChange(onChange)
                    .DataAction(action)
                    .DataMethod(method),
                action: () => hb
                    .OptionCollection(
                        optionCollection: optionCollection, 
                        selectedValue: selectedValue,
                        addSelectedValue: addSelectedValue,
                        column: column));
        }

        public static HtmlBuilder OptionCollection(
            this HtmlBuilder hb,
            Dictionary<string, string> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            Column column = null)
        {
            OptionCollection(
                optionCollection: optionCollection?
                    .ToDictionary(o => o.Key, o => new ControlData(o.Value)),
                selectedValue: selectedValue,
                addSelectedValue: addSelectedValue,
                column: column)?
                    .ForEach(htmlData => hb.Option(
                        attributes: Html.Attributes()
                            .Value(htmlData.Key)
                            .DataClass(htmlData.Value.Css)
                            .DataStyle(htmlData.Value.Style)
                            .Selected(selectedValue == htmlData.Key),
                        action: () => hb
                            .Text(text: Strings.CoalesceEmpty(
                                htmlData.Value.Text, htmlData.Key))));
            return hb;
        }

        public static HtmlBuilder OptionCollection(
            this HtmlBuilder hb,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            Column column = null)
        {
            OptionCollection(
                optionCollection: optionCollection,
                selectedValue: selectedValue,
                addSelectedValue: addSelectedValue,
                column: column)?
                    .ForEach(htmlData => hb.Option(
                        attributes: Html.Attributes()
                            .Value(htmlData.Key)
                            .DataClass(htmlData.Value.Css)
                            .DataStyle(htmlData.Value.Style)
                            .Selected(selectedValue == htmlData.Key),
                        action: () => hb
                            .Text(text: Strings.CoalesceEmpty(
                                htmlData.Value.Text, htmlData.Key))));
            return hb;
        }

        private static Dictionary<string, ControlData> OptionCollection(
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            Column column = null)
        {
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

        public static HtmlBuilder RadioButtons(
            this HtmlBuilder hb,
            string name = "",
            string controlCss = "",
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "")
        {
            optionCollection
                .Select((o, i) => new { Option = o, Index = i })
                .ForEach(data => hb
                    .Input(Html.Attributes()
                        .Id(name, data.Index)
                        .Class(CssClasses.Get("control-radio", controlCss))
                        .Type("radio")
                        .Value(data.Option.Key)
                        .Checked(data.Option.Key == selectedValue))
                    .Label(
                        attributes: Html.Attributes().For(name + data.Index),
                        action: () => hb
                            .Text(data.Option.Value.Text)));
            return hb;
        }

        public static HtmlBuilder Spinner(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            decimal value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            int width = -1,
            string onChange = "",
            string action = "",
            string method = "")
        {
            return hb.Input(Html.Attributes()
                .Id_Css( controlId, CssClasses.Get("control-spinner", controlCss))
                .Type("number")
                .Value(value.ToString())
                .Min(min)
                .Max(max)
                .Step(step)
                .DataWidth(width)
                .OnChange(onChange)
                .DataAction(action)
                .DataMethod(method));
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
            string method = "")
        {
            hb.Input(attributes: Html.Attributes()
                .Id_Css(controlId, CssClasses.Get("control-checkbox", controlCss))
                .Type("checkbox")
                .Disabled(disabled)
                .DataId(dataId)
                .DataAction(action)
                .DataMethod(method)
                .Checked(_checked));
            if (labelText != string.Empty)
            {
                hb.Label(
                    attributes: Html.Attributes().For(controlId),
                    action: () => hb
                        .Text(text: labelText));
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
            string method = "")
        {
            return hb
                .Div(attributes: Html.Attributes()
                    .Id_Css(controlId + ",ui", "control-slider-ui")
                    .Min(min)
                    .Max(max)
                    .Step(step))
                .P(
                    attributes: Html.Attributes()
                        .Class(CssClasses.Get("control-slider", controlCss)),
                    action: () => hb
                        .Span(
                            attributes: Html.Attributes()
                                .Id(controlId).DataAction(action)
                                .DataAction(action)
                                .DataMethod(method),
                            action: () => hb
                                .Text(value))
                        .Span(action: () => hb
                            .Text(unit)));
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
            string type = "button")
        {
            return hb.Button(
                attributes: Html.Attributes()
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
                    .Text(text: text));
        }

        public static HtmlBuilder Anchor(
            this HtmlBuilder hb,
            string controlId = "",
            string controlCss = "",
            string text = "",
            string href = "")
        {
            return hb.A(
                id: controlId,
                css: CssClasses.Get("control-anchor", controlCss),
                href: href,
                text: text);
        }

        public static HtmlBuilder Icon(
            this HtmlBuilder hb,
            string iconCss = "",
            string cssText = "",
            string text = "")
        {
            hb.Span(css: "ui-icon " + iconCss);
            return text != string.Empty
                ? hb.Span(css: cssText, action: () => hb
                    .Text(text:text))
                : hb;
        }

        public static HtmlBuilder Hidden(
            this HtmlBuilder hb,
            string controlId = "",
            string css = "",
            string value = "")
        {
            return hb.Input(attributes: Html.Attributes()
                .Id(controlId)
                .Class(css)
                .Type("hidden")
                .Value(value));
        }

        public static HtmlBuilder Hidden(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null)
        {
            return hb.Input(attributes: attributes.Type("hidden"));
        }

        public static HtmlBuilder Selectable(
            this HtmlBuilder hb,
            string controlId,
            string controlCss = "",
            Dictionary<string, string> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null)
        {
            return hb.Div(
                css: CssClasses.Get("control", controlCss),
                action: () => hb
                    .Ol(
                        attributes: Html.Attributes()
                            .Id_Css(controlId, "control-selectable"),
                        action: () => hb
                            .SelectableItems(
                                listItemCollection: listItemCollection,
                                selectedValueTextCollection: selectedValueCollection)));
        }

        public static HtmlBuilder SelectableItems(
            this HtmlBuilder hb,
            Dictionary<string, string> listItemCollection = null,
            IEnumerable<string> selectedValueTextCollection = null)
        {
            selectedValueTextCollection = selectedValueTextCollection ?? new List<string>();
            listItemCollection.ForEach(listItem => hb
                .Li(
                    attributes: Html.Attributes()
                        .Class(
                            selectedValueTextCollection.Contains(listItem.Key)
                                ? "ui-widget-content ui-selected"
                                : "ui-widget-content")
                        .Value( listItem.Key),
                    action: () => hb
                        .Text(text: listItem.Value)));
            return hb;
        }
    }
}
