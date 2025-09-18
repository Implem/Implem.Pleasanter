using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlControls
    {
        public static HtmlBuilder TextBox(
            this HtmlBuilder hb,
            Context context = null,
            HtmlTypes.TextTypes textType = HtmlTypes.TextTypes.Normal,
            string controlId = null,
            string controlCss = null,
            string text = null,
            string placeholder = null,
            string format = null,
            bool timepicker = false,
            bool disabled = false,
            bool alwaysSend = false,
            string unit = null,
            string accept = null,
            string dataId = null,
            string dataLang = null,
            string onChange = null,
            string autoComplete = null,
            bool openAnchorNewTab = false,
            string anchorFormat = null,
            bool validateRequired = false,
            bool validateNumber = false,
            decimal validateMinNumber = 0,
            decimal validateMaxNumber = 0,
            bool validateDate = false,
            bool validateEmail = false,
            string validateEqualTo = null,
            int validateMaxLength = 0,
            string validateRegex = null,
            string validateRegexErrorMessage = null,
            string action = null,
            string method = null,
            string dataValue = null,
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            if (!_using) return hb;
            switch (textType)
            {
                case HtmlTypes.TextTypes.Normal:
                    return hb
                        .Div(
                            attributes: new HtmlAttributes()
                                .Id(controlId + ".viewer")
                                .Class("control-text not-send")
                                .DataFormat(anchorFormat),
                            action: () => hb.A(
                                text: text,
                                href: anchorFormat?.Replace("{Value}", text),
                                target: openAnchorNewTab
                                    ? "_blank"
                                    : string.Empty),
                            _using: !anchorFormat.IsNullOrEmpty())
                        .Div(
                            attributes: new HtmlAttributes()
                                .Id(controlId + ".editor")
                                .Class("ui-icon ui-icon-pencil button-edit-markdown")
                                .OnClick($"$p.toggleAnchor($('#{controlId}'));"),
                            _using: !anchorFormat.IsNullOrEmpty())
                        .Div(
                            css: "input-field",
                            action: () => hb.Input(attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Name(controlId)
                                .Class(Css.Class(
                                    anchorFormat.IsNullOrEmpty()
                                        ? "control-textbox"
                                        : "control-textbox anchor hidden", controlCss))
                                .Type("text")
                                .Value(text)
                                .DataValue(dataValue)
                                .Placeholder(placeholder)
                                .Disabled(disabled)
                                .DataAlwaysSend(alwaysSend)
                                .DataId(dataId)
                                .OnChange(onChange)
                                .AutoComplete(autoComplete)
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
                                .DataValidateRegex(validateRegex)
                                .DataValidateRegexErrorMessage(validateRegexErrorMessage)
                                .DataAction(action)
                                .DataMethod(method)
                                .Add(attributes)).Span(
                                    css: "unit",
                                    _using: !unit.IsNullOrEmpty(),
                                    action: () => hb.Text(unit)));
                case HtmlTypes.TextTypes.DateTime:
                    return !Parameters.General.UseOldDatepicker && context.ThemeVersionForCss() >= 2.0M ?
                        hb.DateField(
                            css: "date-field",
                            action: () => hb.Input(attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Name(controlId)
                                .Class(Css.Class("control-textbox", controlCss))
                                .Type("text")
                                .Value(text)
                                .Placeholder(placeholder)
                                .Disabled(disabled)
                                .DataAlwaysSend(alwaysSend)
                                .DataId(dataId)
                                .OnChange(onChange)
                                .AutoComplete(autoComplete ?? "off")
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
                                .Add(attributes)))
                        : hb.Div(
                            css: "date-field",
                            action: () => hb.Input(attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Name(controlId)
                                .Class(Css.Class("control-textbox datepicker", controlCss))
                                .Type("text")
                                .Value(text)
                                .Placeholder(placeholder)
                                .Disabled(disabled)
                                .DataAlwaysSend(alwaysSend)
                                .DataId(dataId)
                                .OnChange(onChange)
                                .AutoComplete(autoComplete ?? "off")
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
                                .Add(attributes)).Div(
                                    css: "ui-icon ui-icon-clock current-time",
                                    action: () => hb.Text(text: "schedule"),
                                    _using: !Parameters.General.HideCurrentTimeIcon));
                case HtmlTypes.TextTypes.MultiLine:
                    return hb.TextArea(
                        attributes: new HtmlAttributes()
                            .Id(controlId)
                            .Name(controlId)
                            .Class(Css.Class("control-textarea", controlCss))
                            .Placeholder(placeholder)
                            .Disabled(disabled)
                            .DataAlwaysSend(alwaysSend)
                            .DataId(dataId)
                            .OnChange(onChange)
                            .AutoComplete(autoComplete)
                            .DataValidateRequired(validateRequired)
                            .DataValidateNumber(validateNumber)
                            .DataValidateDate(validateDate)
                            .DataValidateEmail(validateEmail)
                            .DataValidateEqualTo(validateEqualTo)
                            .DataValidateMaxLength(validateMaxLength)
                            .DataAction(action)
                            .DataMethod(method)
                            .Add(attributes),
                        text: text);
                case HtmlTypes.TextTypes.CodeEditor:
                    return hb.CodeEditor(
                       action: () => hb.TextArea(
                            attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Name(controlId)
                                .Class(controlCss)
                                .Placeholder(placeholder)
                                .Disabled(disabled)
                                .DataAlwaysSend(alwaysSend)
                                .DataId(dataId)
                                .OnChange(onChange)
                                .AutoComplete(autoComplete)
                                .DataValidateRequired(validateRequired)
                                .DataValidateNumber(validateNumber)
                                .DataValidateDate(validateDate)
                                .DataValidateEmail(validateEmail)
                                .DataValidateEqualTo(validateEqualTo)
                                .DataValidateMaxLength(validateMaxLength)
                                .DataAction(action)
                                .DataMethod(method)
                                .DataLang(dataLang)
                                .Add(attributes),
                            text: text)
                       );
                case HtmlTypes.TextTypes.Password:
                    var isDummyField = controlCss
                        ?.Split(' ')
                        .Contains("dummy") == true;
                    return hb.Input(
                        attributes: new HtmlAttributes()
                            .Id(controlId)
                            .Name(controlId)
                            .Class(Css.Class("control-textbox", controlCss))
                            .Type("password")
                            .Value(text)
                            .Placeholder(placeholder)
                            .Disabled(disabled)
                            .DataAlwaysSend(alwaysSend)
                            .DataId(dataId)
                            .OnChange(onChange)
                            .AutoComplete(autoComplete)
                            .DataValidateRequired(validateRequired)
                            .DataValidateNumber(validateNumber)
                            .DataValidateDate(validateDate)
                            .DataValidateEmail(validateEmail)
                            .DataValidateEqualTo(validateEqualTo)
                            .DataValidateMaxLength(validateMaxLength)
                            .Add(attributes)
                            .Add("data-passwordgenerator", Implem.DefinitionAccessor.Parameters.Security.PasswordGenerator ? "1" : "0"))
                        .Div(
                            attributes: new HtmlAttributes()
                                .Class("material-symbols-outlined show-password")
                                .OnClick("$p.showPassword(this)"),
                            action: () => hb.Text("visibility"),
                            _using: !isDummyField);
                case HtmlTypes.TextTypes.File:
                    return hb.Input(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Name(controlId)
                        .Class(Css.Class("control-textbox", controlCss))
                        .Type("file")
                        .Value(text)
                        .Accept(accept)
                        .Disabled(disabled)
                        .DataAlwaysSend(alwaysSend)
                        .DataId(dataId)
                        .OnChange(onChange)
                        .AutoComplete(autoComplete)
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
            Context context,
            SiteSettings ss,
            string controlId = null,
            string controlCss = null,
            string text = null,
            string placeholder = null,
            bool readOnly = false,
            bool allowImage = true,
            bool allowBulkUpdate = false,
            bool mobile = false,
            bool alwaysSend = false,
            bool validateRequired = false,
            Column.ViewerSwitchingTypes viewerSwitchingTypes = Column.ViewerSwitchingTypes.Auto,
            Dictionary<string, string> attributes = null,
            bool preview = false,
            bool _using = true,
            int validateMaxLength = 0,
            string validateRegex = null,
            string validateRegexErrorMessage = null)
        {
            if (!_using) return hb;
            if (preview) controlId = Strings.NewGuid();
            if (viewerSwitchingTypes != Column.ViewerSwitchingTypes.Disabled)
            {
                hb
                    .Div(attributes: new HtmlAttributes()
                        .Id(controlId + ".viewer")
                        .Class("control-markup not-send")
                        .OnDblClick($"$p.editMarkdown($('#{controlId}'));"))
                    .Div(
                        attributes: new HtmlAttributes()
                            .Id(controlId + ".editor")
                            .Class("ui-icon ui-icon-pencil button-edit-markdown")
                            .OnClick($"$p.editMarkdown($('#{controlId}'));"),
                       _using: !readOnly);
            }
            hb
                .TextArea(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Name(controlId)
                        .Class(Css.Class((viewerSwitchingTypes == Column.ViewerSwitchingTypes.Disabled
                            ? "control-textarea"
                            : "control-markdown")
                                + (viewerSwitchingTypes == Column.ViewerSwitchingTypes.Manual
                                    ? " manual"
                                    : string.Empty)
                                + (CanUploadImage(
                                    context: context,
                                    ss: ss,
                                    readOnly: readOnly,
                                    allowImage: allowImage,
                                    preview: preview)
                                        ? " upload-image"
                                        : string.Empty),
                                controlCss))
                                    .Placeholder(placeholder)
                                    .DataAlwaysSend(alwaysSend)
                                    .DataValidateMaxLength(validateMaxLength)
                                    .DataValidateRequired(validateRequired, _using: !readOnly)
                                    .DataValidateRegex(validateRegex)
                                    .DataValidateRegexErrorMessage(validateRegexErrorMessage)
                                    .DataReadOnly(readOnly)
                                    .Add(attributes)
                        .Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                    text: text)
                .MarkDownCommands(
                    context: context,
                    ss: ss,
                    controlId: controlId,
                    readOnly: readOnly,
                    allowImage: allowImage,
                    mobile: mobile,
                    preview: preview);
            return hb;
        }

        public static HtmlBuilder RTEditor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string controlId = null,
            string controlCss = null,
            string text = null,
            string placeholder = null,
            bool readOnly = false,
            bool disabled = false,
            bool allowImage = true,
            bool allowBulkUpdate = false,
            bool mobile = false,
            bool alwaysSend = false,
            bool validateRequired = false,
            Column.ViewerSwitchingTypes viewerSwitchingTypes = Column.ViewerSwitchingTypes.Auto,
            Dictionary<string, string> attributes = null,
            bool preview = false,
            bool _using = true,
            int validateMaxLength = 0,
            string validateRegex = null,
            string validateRegexErrorMessage = null)
        {
            if (!_using) return hb;
            hb.RichTextEditor(
                action: () => hb.TextArea(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Name(controlId)
                        .Placeholder(placeholder)
                        .Disabled(disabled)
                        .DataAlwaysSend(alwaysSend)
                        .DataValidateRequired(validateRequired, _using: !readOnly)
                        .DataValidateRegex(validateRegex)
                        .DataValidateRegexErrorMessage(validateRegexErrorMessage)
                        .DataReadOnly(readOnly)
                        .Add(attributes)
                        .Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                    text: text)
            );
            return hb;
        }

        public static HtmlBuilder MarkDownCommands(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string controlId,
            bool readOnly,
            bool allowImage,
            bool mobile,
            bool preview)
        {
            return CanUploadImage(
                context: context,
                ss: ss,
                readOnly: readOnly,
                allowImage: allowImage,
                preview: preview)
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

        private static bool CanUploadImage(
            Context context,
            SiteSettings ss,
            bool readOnly,
            bool allowImage,
            bool preview)
        {
            return context.ContractSettings.Images()
                && ss?.LockedTable() != true
                && ss?.LockedRecord() != true
                && !readOnly
                && allowImage
                && !preview;
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
            Context context,
            string controlId = null,
            string controlCss = null,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            bool multiple = false,
            bool addSelectedValue = true,
            bool insertBlank = false,
            bool disabled = false,
            bool alwaysSend = false,
            string onChange = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            Column column = null,
            Action controlOption = null,
            bool _using = true)
        {
            var srcId = column?.RelatingSrcId().ToString() ?? string.Empty;
            return _using
                ? hb.Div(
                    css: "select-field",
                    action: () => {
                        hb.Select(
                            attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Name(controlId)
                                .Class(Css.Class(
                                    "control-dropdown" + (optionCollection?.Any(o =>
                                        !o.Value.Css.IsNullOrEmpty() ||
                                        !o.Value.Style.IsNullOrEmpty()) == true
                                            ? " has-css"
                                            : string.Empty),
                                    controlCss))
                                .DataId(srcId)
                                .Multiple(multiple)
                                .Disabled(disabled)
                                .DataAlwaysSend(alwaysSend)
                                .OnChange(onChange)
                                .DataValidateRequired(validateRequired)
                                .DataAction(action)
                                .DataMethod(method),
                            action: () => hb
                                .OptionCollection(
                                    context: context,
                                    optionCollection: optionCollection,
                                    selectedValue: selectedValue,
                                    multiple: multiple,
                                    addSelectedValue: addSelectedValue,
                                    insertBlank: insertBlank,
                                    column: column));
                            controlOption?.Invoke();
                        })
                : hb;
        }

        public static HtmlBuilder OptionCollection(
            this HtmlBuilder hb,
            Context context,
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
                    context: context,
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
                                    selectedValue: selectedValue,
                                    selectedValues: selectedValues,
                                    controlValue: htmlData.Key,
                                    multiple: multiple))
                                .Add(htmlData.Value.Attributes),
                            action: () =>
                            {
                                hb.Text(text: Strings.CoalesceEmpty(
                                    htmlData.Value.Text,
                                    htmlData.Key));
                            }));
            }
            return hb;
        }

        private static Dictionary<string, ControlData> OptionCollection(
            Context context,
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
                var selectedId = selectedValue.ToInt();
                optionCollection?.Add(
                    selectedValue,
                    column != null && column.Type != Column.Types.Normal
                        ? new ControlData(SiteInfo.Name(
                            context: context,
                            id: selectedId,
                            type: column.Type))
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
            bool disabled = false,
            string selectedValue = null,
            bool _using = true)
        {
            if (_using)
            {
                optionCollection
                    .Select((o, i) => new { Option = o, Index = i })
                    .ForEach(data => hb.Label(
                        attributes: new HtmlAttributes()
                            .Class("radio-option")
                            .For(name + data.Index),
                        action: () => hb
                            .Span(
                                attributes: new HtmlAttributes().Class("radio-icon"),
                                action: () => hb.Input(attributes: new HtmlAttributes()
                                    .Id(name + data.Index)
                                    .Name(name)
                                    .Class(Css.Class("control-radio", controlCss))
                                    .Type("radio")
                                    .Value(data.Option.Key)
                                    .Disabled(disabled)
                                    .Checked(data.Option.Key == selectedValue)))
                            .Span(
                                attributes: new HtmlAttributes().Class("radio-text"),
                                action: () => hb.Text(data.Option.Value.Text))));
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
            string unit = null,
            string placeholder = null,
            bool alwaysSend = false,
            bool allowBalnk = false,
            string onChange = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    css: "spinner-field",
                    action: () => hb.Input(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Name(controlId)
                        .Class(Css.Class("control-spinner", controlCss)
                            + (allowBalnk
                                ? " allow-blank"
                                : string.Empty))
                        .Type("number")
                        .Value(value != null
                            ? value.ToString()
                            : string.Empty)
                        .Add("data-raw",
                            value != null
                                ? value.ToString()
                                : string.Empty)
                        .Placeholder(placeholder)
                        .DataMin(min, _using: min != -1)
                        .DataMax(max, _using: max != -1)
                        .DataStep(step, _using: step != -1)
                        .DataWidth(width, _using: width != -1)
                        .DataAlwaysSend(alwaysSend)
                        .OnChange(onChange)
                        .DataValidateRequired(validateRequired)
                        .DataAction(action)
                        .DataMethod(method)).Span(
                            css: "unit",
                            _using: !unit.IsNullOrEmpty(),
                            action: () => hb.Text(unit)))
                : hb;
        }

        public static HtmlBuilder CheckBox(
            this HtmlBuilder hb,
            string controlId = null,
            string controlCss = null,
            string labelText = null,
            string labelIcon = null,
            string labelRaw = null,
            bool controlOnly = false,
            bool _checked = false,
            bool disabled = false,
            bool alwaysSend = false,
            string dataId = null,
            string onChange = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            bool _using = true)
        {
            if (_using)
            {
                hb.Label(
                    attributes: new HtmlAttributes()
                        .Class(validateRequired
                            ? "check-option required"
                            : "check-option")
                        .For(controlId),
                    action: () => hb
                        .Span(
                            attributes: new HtmlAttributes().Class("check-icon"),
                            action: () => hb.Input(attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Name(controlId)
                                .Class(Css.Class("control-checkbox", controlCss))
                                .Type("checkbox")
                                .Disabled(disabled)
                                .DataAlwaysSend(alwaysSend)
                                .DataId(dataId)
                                .OnChange(onChange)
                                .DataValidateRequired(validateRequired)
                                .DataAction(action)
                                .DataMethod(method)
                                .DataReadOnly(disabled)
                                .Checked(_checked)))
                        .Span(
                            attributes: new HtmlAttributes().Class("check-text"),
                            action: () =>
                            {
                                if (!controlOnly)
                                {
                                    if (labelRaw != null)
                                    {
                                        hb.Raw(text: labelRaw);
                                    }
                                    else if (labelText != string.Empty)
                                    {
                                        if (!labelIcon.IsNullOrEmpty())
                                        {
                                            hb.Span(css: $"check-add-icon ui-icon {labelIcon}");
                                        }
                                    }
                                }
                                hb.Text(text: labelText);
                            },
                            _using: !controlOnly));
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
            bool alwaysSend = false,
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
                            .DataAlwaysSend(alwaysSend)
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
            string style = null,
            string title = null,
            string accessKey = null,
            string onClick = null,
            string href = null,
            string dataId = null,
            string icon = null,
            string selector = null,
            string validations = null,
            string action = null,
            string method = null,
            string confirm = null,
            string type = "button",
            string dataType = null,
            bool disabled = false,
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Button(
                    attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Class("button " + controlCss)
                        .Style(style)
                        .Type(type)
                        .Title(title)
                        .AccessKey(accessKey)
                        .OnClick(onClick + href.IsNotEmpty("$p.transition('" + href + "');"))
                        .DataId(dataId)
                        .DataIcon(icon)
                        .DataSelector(selector)
                        .DataValidations(validations)
                        .DataAction(action)
                        .DataMethod(method)
                        .DataConfirm(confirm)
                        .DataType(dataType)
                        .Disabled(disabled)
                        .Add(attributes),
                    action: () => hb
                        .Text(text: text))
                : hb;
        }

        public static HtmlBuilder Icon(
            this HtmlBuilder hb,
            string iconCss = null,
            string cssText = null,
            string text = null,
            string onClick = null,
            bool _using = true)
        {
            return _using
                ? hb
                    .Span(
                        css: "ui-icon " + iconCss,
                        attributes: new HtmlAttributes()
                            .OnClick(onClick))
                    .Span(
                        css: cssText,
                        _using: text != string.Empty,
                        action: () => hb
                            .Text(text: text))
                : hb;
        }

        public static HtmlBuilder Hidden(
            this HtmlBuilder hb,
            string controlId = null,
            string css = null,
            string value = null,
            string rawValue = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            bool alwaysSend = false,
            bool _using = true)
        {
            return _using
                ? hb.Input(attributes: new HtmlAttributes()
                    .Id(controlId)
                    .Name(controlId)
                    .Class(css)
                    .Type("hidden")
                    .Value(value)
                    .RawValue(rawValue)
                    .DataValidateRequired(validateRequired)
                    .DataAction(action)
                    .DataMethod(method)
                    .DataAlwaysSend(alwaysSend))
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
            bool alwaysDataValue = false,
            string action = null,
            string method = null,
            bool setSearchOptionButton = false,
            string searchOptionId = null,
            string searchOptionFunction = null,
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
                            alwaysDataValue: alwaysDataValue,
                            action: action,
                            method: method))
                    .SearchOptionButton(
                        setSearchOptionButton: setSearchOptionButton,
                        searchOptionId: searchOptionId,
                        searchOptionFunction: searchOptionFunction)
                : hb;
        }

        public static HtmlBuilder SearchOptionButton(
            this HtmlBuilder hb,
            bool setSearchOptionButton = false,
            string searchOptionId = null,
            string searchOptionFunction = null)
        {
            return setSearchOptionButton
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id(searchOptionId)
                        .Class("ui-icon ui-icon-search lower-search-ui")
                        .OnClick(searchOptionFunction)
                        .DataAction("SetSiteSettings")
                        .DataMethod("post"))
                : hb;
        }

        public static HtmlBuilder Selectable(
            this HtmlBuilder hb,
            string controlId,
            string controlCss = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool alwaysDataValue = false,
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
                            selectedValueTextCollection: selectedValueCollection,
                            alwaysDataValue: alwaysDataValue))
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
            bool alwaysDataValue = false,
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
                            .DataOrder(listItem.Value?.Order)
                            .DataValue(
                                listItem.Key,
                                _using: alwaysDataValue
                                    || listItem.Value?.Text != listItem.Key),
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
            Context context,
            string controlId = null,
            string columnName = null,
            string value = null,
            bool readOnly = false,
            bool allowDelete = true,
            bool preview = false,
            bool _using = true,
            bool validateRequired = false,
            int validateMaxLength = 0,
            string inputGuide = null)
        {
            if (preview) controlId = Strings.NewGuid();
            var attachments = value.Deserialize<Attachments>();
            return _using
                ? hb
                    .Hidden(attributes: new HtmlAttributes()
                        .Id(controlId)
                        .Name(controlId)
                        .DataName(columnName)
                        .Class("control-attachments")
                        .DataReadOnly(readOnly)
                        .Value(value)
                        .DataValidateAttachmentsRequired(validateRequired))
                    .Div(
                        id: columnName + ".upload",
                        css: "control-attachments-upload",
                        attributes: new HtmlAttributes()
                            .DataName(columnName)
                            .DataAction("binaries/multiupload")
                            .DataValidateMaxLength(validateMaxLength),
                        action: () => hb
                            .Text(text: inputGuide.IsNullOrEmpty()
                                ? Displays.FileDragDrop(context: context)
                                : inputGuide)
                            .Input(attributes: new HtmlAttributes()
                                .Id(columnName + ".input")
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
                                    context: context,
                                    controlId: controlId,
                                    guid: item.Guid,
                                    css: (item.Added == true && item.Deleted == false)
                                        ? string.Empty
                                        : item.Deleted == true
                                            ? "already-attachments preparation-delete "
                                            : "already-attachments ",
                                    fileName: item.Name,
                                    displaySize: item.DisplaySize(),
                                    added: item.Added,
                                    deleted: item.Deleted,
                                    readOnly: readOnly,
                                    allowDelete: allowDelete,
                                    _using: (item?.Added == true) || item.Exists(context: context))))
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
                                    .Text(text: Displays.Cancel(context: context))),
                        _using: !readOnly)
                : hb;
        }

        private static HtmlBuilder AttachmentItem(
            this HtmlBuilder hb,
            Context context,
            string controlId = null,
            string guid = null,
            string css = null,
            string fileName = null,
            string displaySize = null,
            bool? added = null,
            bool? deleted = null,
            bool readOnly = false,
            bool allowDelete = true,
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
                                .Href(Locations.ShowFile(
                                    context: context,
                                    guid: guid,
                                    temp: added == true))
                                .Target("_blank"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-circle-zoomin show-file"))
                        .A(
                            attributes: new HtmlAttributes()
                                .Class("file-name")
                                .Href(Locations.DownloadFile(
                                    context: context,
                                    guid: guid,
                                    temp: added == true)),
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
                            _using: !readOnly && (allowDelete || added == true)))
                 : hb;
        }
    }
}
