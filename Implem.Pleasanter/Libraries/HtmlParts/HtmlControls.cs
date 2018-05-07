using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlControls
    {
        public static HtmlBuilder TextBox(
            this HtmlBuilder hb,
            HtmlTypes.TextTypes textType = HtmlTypes.TextTypes.Normal,
            string controlId = null,
            string controlCss = null,
            string text = null,
            string placeholder = null,
            string onChange = null,
            string format = null,
            bool timepicker = false,
            string accept = null,
            string dataId = null,
            bool validateRequired = false,
            bool validateNumber = false,
            decimal validateMinNumber = 0,
            decimal validateMaxNumber = 0,
            bool validateDate = false,
            bool validateEmail = false,
            string validateEqualTo = null,
            int validateMaxLength = 0,
            string action = null,
            string method = null,
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            if (!_using) return hb;
            switch (textType)
            {
                case HtmlTypes.TextTypes.Normal:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class("control-textbox", controlCss))
                        .Type("text")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .DataId(dataId)
                        .DataValidateRequired(validateRequired)
                        .DataValidateNumber(validateNumber)
                        .DataValidateMinNumber(
                            validateMinNumber, _using: validateMinNumber != validateMaxNumber)
                        .DataValidateMaxNumber(
                            validateMaxNumber, _using: validateMinNumber != validateMaxNumber)
                        .DataValidateDate(validateDate)
                        .DataValidateEmail(validateEmail)
                        .DataValidateEqualTo(validateEqualTo)
                        .DataValidateMaxLength(validateMaxLength)
                        .DataAction(action)
                        .DataMethod(method)
                        .Add(attributes));
                case HtmlTypes.TextTypes.DateTime:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class("control-textbox datepicker", controlCss))
                        .Type("text")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .DataId(dataId)
                        .DataFormat(format)
                        .DataTimepicker(timepicker)
                        .DataValidateRequired(validateRequired)
                        .DataValidateNumber(validateNumber)
                        .DataValidateDate(validateDate)
                        .DataValidateEmail(validateEmail)
                        .DataValidateEqualTo(validateEqualTo)
                        .DataValidateMaxLength(validateMaxLength)
                        .DataAction(action)
                        .DataMethod(method)
                        .Add(attributes));
                case HtmlTypes.TextTypes.MultiLine:
                    return hb.TextArea(
                        attributes: new HtmlAttributes()
                            .Id(controlId)
                            .Class(Css.Class("control-textarea", controlCss))
                            .Placeholder(placeholder)
                            .OnChange(onChange)
                            .DataId(dataId)
                            .DataValidateRequired(validateRequired)
                            .DataValidateNumber(validateNumber)
                            .DataValidateDate(validateDate)
                            .DataValidateEmail(validateEmail)
                            .DataValidateEqualTo(validateEqualTo)
                            .DataValidateMaxLength(validateMaxLength)
                            .DataAction(action)
                            .DataMethod(method)
                            .Add(attributes),
                        action: () => hb
                            .Text(text: text));
                case HtmlTypes.TextTypes.Password:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class("control-textbox", controlCss))
                        .Type("password")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .DataId(dataId)
                        .DataValidateRequired(validateRequired)
                        .DataValidateNumber(validateNumber)
                        .DataValidateDate(validateDate)
                        .DataValidateEmail(validateEmail)
                        .DataValidateEqualTo(validateEqualTo)
                        .DataValidateMaxLength(validateMaxLength)
                        .Add(attributes));
                case HtmlTypes.TextTypes.File:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class("control-textbox", controlCss))
                        .Type("file")
                        .Value(text)
                        .Placeholder(placeholder)
                        .OnChange(onChange)
                        .Accept(accept)
                        .DataId(dataId)
                        .DataValidateRequired(validateRequired)
                        .DataValidateNumber(validateNumber)
                        .DataValidateDate(validateDate)
                        .DataValidateEmail(validateEmail)
                        .DataValidateEqualTo(validateEqualTo)
                        .DataValidateMaxLength(validateMaxLength)
                        .DataAction(action)
                        .DataMethod(method)
                        .Add(attributes));
                default:
                    return hb;
            }
        }

        public static HtmlBuilder MarkDown(
            this HtmlBuilder hb,
            string controlId = null,
            string controlCss = null,
            string text = null,
            string placeholder = null,
            bool readOnly = false,
            bool allowImage = true,
            bool mobile = false,
            bool validateRequired = false,
            Dictionary<string, string> attributes = null,
            bool preview = false,
            bool _using = true)
        {
            if (preview) controlId = Strings.NewGuid();
            return _using
                ? hb
                    .Div(attributes: new HtmlAttributes()
                        .Id(controlId + ".viewer")
                        .Class("control-markup not-send")
                        .OnDblClick("$p.editMarkdown($(this));"))
                    .Div(
                        attributes: new HtmlAttributes()
                            .Id(controlId + ".editor")
                            .Class("ui-icon ui-icon-pencil button-edit-markdown")
                            .OnClick("$p.editMarkdown($(this));"),
                        _using: !readOnly)
                    .TextArea(
                        attributes: new HtmlAttributes()
                            .Id(controlId)
                            .Class(Css.Class(
                                "control-markdown" + (CanUploadImage(readOnly, allowImage, preview)
                                    ? " upload-image"
                                    : string.Empty),
                                controlCss))
                            .Placeholder(placeholder)
                            .DataValidateRequired(validateRequired, _using: !readOnly)
                            .Add(attributes),
                        action: () => hb
                            .Text(text: text))
                    .MarkDownCommands(
                        controlId: controlId,
                        readOnly: readOnly,
                        allowImage: allowImage,
                        mobile: mobile,
                        preview: preview)
                : hb;
        }

        public static HtmlBuilder MarkDownCommands(
            this HtmlBuilder hb,
            string controlId,
            bool readOnly,
            bool allowImage,
            bool mobile,
            bool preview)
        {
            return CanUploadImage(readOnly, allowImage, preview)
                ? hb
                    .Div(
                        attributes: new HtmlAttributes()
                            .Class("ui-icon ui-icon-image button-upload-image")
                            .OnClick($"$p.selectImage('{controlId}');"))
                    .Div(
                        attributes: new HtmlAttributes()
                            .Class("ui-icon ui-icon-video")
                            .OnClick($"$p.openVideo('{controlId}');"),
                        _using: !mobile)
                    .TextBox(
                        controlId: controlId + ".upload-image-file",
                        controlCss: "hidden upload-image-file",
                        textType: HtmlTypes.TextTypes.File,
                        accept: "image/*",
                        dataId: controlId)
                : hb;
        }

        private static bool CanUploadImage(bool readOnly, bool allowImage, bool preview)
        {
            return Contract.Images() && !readOnly && allowImage && !preview;
        }

        public static HtmlBuilder MarkUp(
            this HtmlBuilder hb,
            string controlId = null,
            string controlCss = null,
            string text = null,
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class("control-markup markup", controlCss)),
                    action: () => hb
                        .Text(text: text))
                : hb;
        }

        public static HtmlBuilder DropDown(
            this HtmlBuilder hb,
            string controlId = null,
            string controlCss = null,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            bool multiple = false,
            bool addSelectedValue = true,
            bool insertBlank = false,
            bool disabled = false,
            string onChange = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            Column column = null,
            bool _using = true)
        {
            return _using
                ? hb.Select(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class(
                            "control-dropdown" + (optionCollection.Any(o =>
                                !o.Value.Css.IsNullOrEmpty() ||
                                !o.Value.Style.IsNullOrEmpty())
                                    ? " has-css"
                                    : string.Empty),
                            controlCss))
                        .Multiple(multiple)
                        .Disabled(disabled)
                        .OnChange(onChange)
                        .DataValidateRequired(validateRequired)
                        .DataAction(action)
                        .DataMethod(method),
                    action: () => hb
                        .OptionCollection(
                            optionCollection: optionCollection,
                            selectedValue: selectedValue,
                            multiple: multiple,
                            addSelectedValue: addSelectedValue,
                            insertBlank: insertBlank,
                            column: column))
                : hb;
        }

        public static HtmlBuilder OptionCollection(
            this HtmlBuilder hb,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            bool multiple = false,
            bool addSelectedValue = true,
            bool insertBlank = false,
            Column column = null,
            bool _using = true)
        {
            if (_using)
            {
                var selectedValues = multiple
                    ? selectedValue.Deserialize<List<string>>()
                    : null;
                OptionCollection(
                    optionCollection: optionCollection,
                    selectedValue: selectedValue,
                    multiple: multiple,
                    addSelectedValue: addSelectedValue,
                    insertBlank: insertBlank,
                    column: column)?
                        .ForEach(htmlData => hb.Option(
                            attributes: new HtmlAttributes()
                                .Value(htmlData.Key)
                                .DataClass(htmlData.Value.Css)
                                .DataStyle(htmlData.Value.Style)
                                .Selected(Selected(
                                    selectedValue, selectedValues, htmlData.Key, multiple)),
                            action: () => hb
                                .Text(text: Strings.CoalesceEmpty(
                                    htmlData.Value.Text, htmlData.Key))));
            }
            return hb;
        }

        private static Dictionary<string, ControlData> OptionCollection(
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            bool multiple = false,
            bool addSelectedValue = true,
            bool insertBlank = false,
            Column column = null)
        {
            if (insertBlank)
            {
                optionCollection = InsertBlank(optionCollection);
            }
            if (selectedValue == null ||
                selectedValue == string.Empty ||
                optionCollection?.ContainsKey(selectedValue) == true ||
                selectedValue == "0" ||
                multiple ||
                !addSelectedValue)
            {
                return optionCollection;
            }
            else
            {
                var userId = selectedValue.ToInt();
                optionCollection?.Add(
                    selectedValue,
                    column != null && column.UserColumn
                        ? new ControlData(SiteInfo.UserName(userId))
                        : new ControlData("? " + selectedValue));
                return optionCollection;
            }
        }

        private static Dictionary<string, ControlData> InsertBlank(
            Dictionary<string, ControlData> optionCollection)
        {
            return optionCollection?.ContainsKey(string.Empty) == false
                ? new Dictionary<string, ControlData>
                {
                    { string.Empty, new ControlData(string.Empty) }
                }.AddRange(optionCollection)
                : optionCollection;
        }

        private static bool Selected(
            string selectedValue, List<string> selectedValues, string controlValue, bool multiple)
        {
            return multiple
                ? selectedValues != null
                    ? selectedValues.Contains(controlValue)
                    : false
                : selectedValue == controlValue;
        }

        public static HtmlBuilder RadioButtons(
            this HtmlBuilder hb,
            string name = null,
            string controlCss = null,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            bool _using = true)
        {
            if (_using)
            {
                optionCollection
                    .Select((o, i) => new { Option = o, Index = i })
                    .ForEach(data => hb
                        .Input(attributes: new HtmlAttributes()
                            .Id(name + data.Index)
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
            string controlId = null,
            string controlCss = null,
            decimal? value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            int width = -1,
            string onChange = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb.Input(attributes: new HtmlAttributes()
                    .Id(controlId)
                    .Class(Css.Class("control-spinner", controlCss))
                    .Type("number")
                    .Value(value != null
                        ? value.ToString()
                        : string.Empty)
                    .DataMin(min, _using: min != -1)
                    .DataMax(max, _using: max != -1)
                    .DataStep(step, _using: step != -1)
                    .DataWidth(width, _using: width != -1)
                    .OnChange(onChange)
                    .DataAction(action)
                    .DataMethod(method))
                : hb;
        }

        public static HtmlBuilder CheckBox(
            this HtmlBuilder hb,
            string controlId = null,
            string controlCss = null,
            string labelText = null,
            bool _checked = false,
            bool disabled = false,
            string dataId = null,
            string onChange = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            if (_using)
            {
                hb.Input(attributes: new HtmlAttributes()
                    .Id(controlId)
                    .Class(Css.Class("control-checkbox", controlCss))
                    .Type("checkbox")
                    .Disabled(disabled)
                    .DataId(dataId)
                    .OnChange(onChange)
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
            string controlId = null,
            string controlCss = null,
            decimal value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            string unit = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb
                    .Div(attributes: new HtmlAttributes()
                        .Id(controlId + ",ui")
                        .Class("control-slider-ui")
                        .DataMin(min, _using: min != -1)
                        .DataMax(max, _using: max != -1)
                        .DataStep(step, _using: step != -1))
                    .P(
                        attributes: new HtmlAttributes()
                            .Id(controlId).DataAction(action)
                            .Class(Css.Class("control-slider", controlCss))
                            .DataAction(action)
                            .DataMethod(method),
                        action: () => hb
                            .Span(action: () => hb
                                .Text(value.ToString()))
                            .Span(action: () => hb
                                .Text(unit)))
                : hb;
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            string controlId = null,
            string text = null,
            string controlCss = null,
            string accessKey = null,
            string onClick = null,
            string href = null,
            string dataId = null,
            string icon = null,
            string selector = null,
            string action = null,
            string method = null,
            string confirm = null,
            string type = "button",
            bool _using = true)
        {
            return _using
                ? hb.Button(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class("button " + controlCss)
                        .Type(type)
                        .AccessKey(accessKey)
                        .OnClick(onClick + href.IsNotEmpty("location.href='" + href + "';"))
                        .DataId(dataId)
                        .DataIcon(icon)
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
            string controlId = null,
            string controlCss = null,
            string text = null,
            string href = null,
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
            string iconCss = null,
            string cssText = null,
            string text = null,
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
            string controlId = null,
            string css = null,
            string value = null,
            string rawValue = null,
            bool _using = true)
        {
            return _using
                ? hb.Input(attributes: new HtmlAttributes()
                    .Id(controlId)
                    .Class(css)
                    .Type("hidden")
                    .Value(value)
                    .RawValue(rawValue))
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

        public static HtmlBuilder SelectableWrapper(
            this HtmlBuilder hb,
            string controlId,
            string controlWrapperCss = null,
            string controlCss = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    id: controlId + "Wrapper",
                    css: Css.Class("wrapper", controlWrapperCss),
                    action: () => hb
                        .Selectable(
                            controlId: controlId,
                            controlCss: Css.Class("control-selectable", controlCss),
                            listItemCollection: listItemCollection,
                            selectedValueCollection: selectedValueCollection,
                            action: action,
                            method: method))
                : hb;
        }

        public static HtmlBuilder Selectable(
            this HtmlBuilder hb,
            string controlId,
            string controlCss = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb.Ol(
                    id: controlId,
                    css: Css.Class("control-selectable", controlCss),
                    attributes: new HtmlAttributes()
                        .DataAction(action)
                        .DataMethod(method),
                    action: () => hb
                        .SelectableItems(
                            listItemCollection: listItemCollection,
                            selectedValueTextCollection: selectedValueCollection))
                : hb;
        }

        public static HtmlBuilder Basket(
            this HtmlBuilder hb,
            string controlId,
            string controlCss = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool _using = true)
        {
            return _using
                ? hb.Ol(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class(Css.Class("control-basket", controlCss)),
                    action: () => hb
                        .SelectableItems(
                            listItemCollection: listItemCollection,
                            selectedValueTextCollection: selectedValueCollection,
                            basket: true))
                : hb;
        }

        public static HtmlBuilder SelectableItems(
            this HtmlBuilder hb,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueTextCollection = null,
            bool basket = false,
            bool _using = true)
        {
            if (_using)
            {
                listItemCollection?.ForEach(listItem => hb
                    .Li(
                        attributes: new HtmlAttributes()
                            .Class(
                                selectedValueTextCollection?.Contains(listItem.Key) == true
                                    ? "ui-widget-content ui-selected"
                                    : "ui-widget-content")
                            .Title(listItem.Value?.Title)
                            .DataValue(listItem.Key, _using: listItem.Value?.Text != listItem.Key),
                        action: () =>
                        {
                            if (basket)
                            {
                                hb
                                    .Span(action: () => hb
                                        .Text(listItem.Value?.Text))
                                    .Span(css: "ui-icon ui-icon-close delete");
                            }
                            else
                            {
                                hb.Text(text: listItem.Value?.Text);
                            }
                        }));
            }
            return hb;
        }

        public static HtmlBuilder Attachments(
           this HtmlBuilder hb,
            string controlId = null,
            string columnName = null,
            string controlCss = null,
            string value = null,
            string placeholder = null,
            bool readOnly = false,
            bool validateRequired = false,
            Dictionary<string, string> attributes = null,
            bool preview = false,
            bool _using = true)
        {
            if (preview) controlId = Strings.NewGuid();
            var attachments = value.Deserialize<Attachments>();
            return _using
                ? hb
                    .Hidden(
                        controlId: controlId,
                        css: "control-attachments",
                        value: value)
                    .Div(
                        id: columnName + ".upload",
                        css: "control-attachments-upload",
                        attributes: new HtmlAttributes()
                            .DataName(columnName)
                            .DataAction("binaries/multiupload"),
                        action: () => hb
                            .Text(text: Displays.FileDragDrop())
                            .Input(
                                id: columnName + ".input",
                                attributes: new HtmlAttributes()
                                    .Class("hidden")
                                    .Type("file")
                                    .Multiple(true)),
                        _using: !readOnly && !preview)
                    .Div(
                        id: columnName + ".items",
                        css: "control-attachments-items",
                        action: () => attachments?
                            .ForEach(item => hb
                                .AttachmentItem(
                                    controlId: controlId,
                                    guid: item.Guid,
                                    css: item.Added == true
                                        ? string.Empty
                                        : item.Deleted == true
                                            ? "already-attachments preparation-delete "
                                            : "already-attachments ",
                                    fileName: item.Name,
                                    displaySize: item.DisplaySize(),
                                    added: item.Added,
                                    deleted: item.Deleted,
                                    readOnly: readOnly)))
                    .Div(
                        id: columnName + ".status",
                        attributes: new HtmlAttributes()
                            .Style("display: none; "),
                        action: () => hb
                            .Div(
                                id: columnName + ".progress",
                                css: "progress-bar",
                                action: () => hb
                                    .Div())
                            .Div(
                                id: columnName + ".abort",
                                css: "abort",
                                action: () => hb
                                    .Text(text: Displays.Cancel())),
                        _using: !readOnly)
                : hb;
        }

        private static HtmlBuilder AttachmentItem(
            this HtmlBuilder hb,
            string controlId = null,
            string guid = null,
            string css = null,
            string fileName = null,
            string displaySize = null,
            bool? added = null,
            bool? deleted = null,
            bool readOnly = false,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    id: guid,
                    css: Css.Class("control-attachments-item ", css),
                    action: () => hb
                        .A(
                            attributes: new HtmlAttributes()
                                .Class("file-name")
                                .Href(Locations.ShowFile(guid, temp: added == true)),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-circle-zoomin show-file"))
                        .A(
                            attributes: new HtmlAttributes()
                                .Class("file-name")
                                .Href(Locations.DownloadFile(guid, temp: added == true)),
                            action: () => hb
                                .Text(text: fileName + "　(" + displaySize + ")"))
                        .Div(
                            attributes: new HtmlAttributes()
                                .Class(deleted == true
                                    ? "ui-icon ui-icon-trash file-delete"
                                    : "ui-icon ui-icon-circle-close delete-file")
                                .DataAction("binaries/deletetemp")
                                .DataId(guid)
                                .OnClick($"$p.deleteAttachment($('#{controlId}'), $(this));"),
                            _using: !readOnly))
                 : hb;
        }
    }
}
