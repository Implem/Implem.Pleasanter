using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlFields
    {
        private enum ControlTypes
        {
            Text,
            TextBox,
            TextBoxNumeric,
            TextBoxDateTime,
            TextBoxMultiLine,
            MarkDown,
            DropDown,
            CheckBox,
            Slider,
            Spinner
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column column,
            BaseModel.MethodTypes methodType = BaseModel.MethodTypes.NotSet,
            string value = null,
            Permissions.ColumnPermissionTypes columnPermissionType = 
                Permissions.ColumnPermissionTypes.Update,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            bool preview = false,
            bool _using = true)
        {
            if (column.Section != null)
            {
                hb.Div(css: "field-section", action: () => hb
                    .Text(text: column.Section));
            }
            if (column.UserColumn && value == User.UserTypes.Anonymous.ToInt().ToString())
            {
                value = string.Empty;
            }
            if (columnPermissionType != Permissions.ColumnPermissionTypes.Deny && _using)
            {
                value = methodType == BaseModel.MethodTypes.New
                    ? value.ToDefault(ss, column)
                    : value;
                return hb.SwitchField(
                    column: column,
                    columnPermissionType: columnPermissionType,
                    controlId: !preview
                        ? column.Id
                        : null,
                    fieldCss: FieldCss(column, fieldCss),
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    controlCss: Strings.CoalesceEmpty(controlCss, column.ControlCss),
                    controlType: ControlType(column),
                    value: value,
                    optionCollection: EditChoices(ss, column, value),
                    preview: preview);
            }
            else
            {
                return hb;
            }
        }

        private static string FieldCss(Column column, string fieldCss)
        {
            return Strings.CoalesceEmpty(fieldCss, column.FieldCss) +
                (column.NoWrap == true
                    ? " both"
                    : string.Empty);
        }

        private static Dictionary<string, ControlData> EditChoices(
            SiteSettings ss, Column column, string value)
        {
            var editChoices = column.EditChoices();
            if (column.UseSearch != true)
            {
                return editChoices;
            }
            else if (editChoices.ContainsKey(value))
            {
                return new Dictionary<string, ControlData>()
                {
                    { value, editChoices[value] }
                };
            }
            else
            {
                var referenceId = value.ToLong();
                if (referenceId > 0 && ss.Links?.Any() == true)
                {
                    return new Dictionary<string, ControlData>()
                    {
                        {
                            value,
                            new ControlData(ItemUtilities.Title(referenceId, ss.Links))
                        }
                    };
                }
                else
                {
                    return new Dictionary<string, ControlData>();
                }
            }
        }

        private static HtmlBuilder SwitchField(
            this HtmlBuilder hb,
            Column column,
            Permissions.ColumnPermissionTypes columnPermissionType,
            string controlId,
            string fieldCss,
            string labelCss,
            string controlContainerCss,
            string controlCss,
            ControlTypes controlType,
            string value,
            Dictionary<string, ControlData> optionCollection,
            bool preview)
        {
            var required = column.Required || (column.ValidateRequired ?? false);
            if (preview)
            {
                required = false;
                column.ValidateNumber = false;
                column.ValidateDate = false;
                column.ValidateEmail = false;
                column.ValidateEqualTo = null;
                column.ValidateMaxLength = 0;
            }
            switch (columnPermissionType)
            {
                case Permissions.ColumnPermissionTypes.Read:
                    switch (controlType)
                    {
                        case ControlTypes.CheckBox:
                            return hb.FieldCheckBox(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                _checked: value.ToBool(),
                                disabled: true);
                        case ControlTypes.MarkDown:
                            return hb.FieldMarkDown(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                placeholder: column.LabelText,
                                readOnly: column.EditorReadOnly == true,
                                validateRequired: required,
                                preview: preview);
                        default:
                            return hb.FieldText(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: column.HasChoices() && optionCollection.ContainsKey(value)
                                    ? optionCollection[value].Text
                                    : value);
                    }
                case Permissions.ColumnPermissionTypes.Update:
                    switch (controlType)
                    {
                        case ControlTypes.DropDown:
                            return hb.FieldDropDown(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss +
                                    (required
                                        ? " always-send"
                                        : string.Empty) +
                                    (column.UseSearch == true
                                        ? " search"
                                        : string.Empty),
                                labelText: column.LabelText,
                                optionCollection: optionCollection,
                                selectedValue: value,
                                insertBlank: !required && column.UseSearch != true,
                                validateRequired: required,
                                column: column);
                        case ControlTypes.Text:
                            return hb.FieldText(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value);
                        case ControlTypes.TextBoxMultiLine:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: column.ValidateMaxLength ?? 0);
                        case ControlTypes.MarkDown:
                            return hb.FieldMarkDown(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                placeholder: column.LabelText,
                                readOnly: column.EditorReadOnly.ToBool(),
                                validateRequired: required,
                                preview: preview);
                        case ControlTypes.TextBox:
                            return hb.FieldTextBox(
                                textType: column.Hash
                                    ? HtmlTypes.TextTypes.Password
                                    : HtmlTypes.TextTypes.Normal,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: column.ValidateMaxLength ?? 0);
                        case ControlTypes.TextBoxNumeric:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.Normal,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                unit: column.Unit,
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateMinNumber: !preview
                                    ? column.MinNumber()
                                    : 0,
                                validateMaxNumber: !preview
                                    ? column.MaxNumber()
                                    : 0,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: column.ValidateMaxLength ?? 0);
                        case ControlTypes.TextBoxDateTime:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.DateTime,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                format: column.DateTimeFormat(),
                                timepiker: column.DateTimepicker(),
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: column.ValidateMaxLength ?? 0);
                        case ControlTypes.CheckBox:
                            return hb.FieldCheckBox(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                _checked: value.ToBool(),
                                disabled: column.EditorReadOnly.ToBool());
                        case ControlTypes.Slider:
                            return hb.FieldSlider(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                value: value.ToDecimal(),
                                min: column.Min.ToDecimal(),
                                max: column.Max.ToDecimal(),
                                step: column.Step.ToDecimal(),
                                unit: column.Unit);
                        case ControlTypes.Spinner:
                            return hb.FieldSpinner(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                value: value.ToDecimal(),
                                min: column.Min.ToDecimal(),
                                max: column.Max.ToDecimal(),
                                step: column.Step.ToDecimal(),
                                width: 50,
                                unit: column.Unit);
                        default:
                            return hb;
                    }
                default:
                    return hb;
            }
        }

        private static string ToDefault(this string self, SiteSettings ss, Column column)
        {
            if (column.Linked(ss, Forms.Long("FromSiteId")))
            {
                var id = Forms.Data("LinkId");
                if (column.UseSearch == true)
                {
                    ss.SetChoiceHash(
                        columnName: column?.ColumnName,
                        selectedValues: id.ToSingleList());
                }
                return id;
            }
            if (column.DefaultInput != string.Empty)
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        return column.DefaultInput.ToBool().ToOneOrZeroString();
                    case Types.CsNumeric:
                        return column.DefaultInput.ToLong().ToString();
                    case Types.CsDateTime:
                        return !self.ToDateTime().InRange()
                            ? DateTime.Now
                                .ToLocal()
                                .AddDays(column.DefaultInput.ToInt())
                                .ToString(Displays.Get(column.EditorFormat + "Format"))
                            : self;
                    default:
                        return column.DefaultInput;
                }
            }
            return self;
        }

        private static ControlTypes ControlType(Column column)
        {
            if (column.EditorReadOnly.ToBool())
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        return ControlTypes.CheckBox;
                    default:
                        return column.FieldCss == "field-markdown"
                            ? ControlTypes.MarkDown
                            : ControlTypes.Text;
                }
            }
            switch (column.ControlType)
            {
                case "TextBox": return ControlTypes.TextBox;
                case "Id": return ControlTypes.Text;
                case "Slider": return ControlTypes.Slider;
                case "Spinner": return ControlTypes.Spinner;
            }
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    return ControlTypes.CheckBox;
                case Types.CsNumeric:
                    return column.HasChoices()
                        ? ControlTypes.DropDown
                        : ControlTypes.TextBoxNumeric;
                case Types.CsDateTime:
                    return ControlTypes.TextBoxDateTime;
                case Types.CsString:
                    return StringControlType(column);
                default:
                    return ControlTypes.Text;
            }
        }

        private static ControlTypes StringControlType(Column column)
        {
            if (column.HasChoices())
            {
                return ControlTypes.DropDown;
            }
            switch (column.FieldCss)
            {
                case "field-normal":
                case "field-wide":
                    return ControlTypes.TextBox;
                case "field-markdown":
                    return ControlTypes.MarkDown;
                default:
                    return 
                        column.Max.ToInt() == -1 ||
                        column.Max.ToInt() >= Parameters.General.SizeToUseTextArea
                            ? ControlTypes.TextBoxMultiLine
                            : ControlTypes.TextBox;
            }
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string labelText = null,
            string labelTitle = null,
            Action controlAction = null,
            Action actionOptions = null,
            string tagControlContainer = "div",
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    actionLabel: () => hb
                        .Label(
                            controlId: controlId,
                            labelCss: labelCss,
                            labelText: labelText,
                            labelTitle: labelTitle),
                    actionControl: () => hb
                        .Control(
                            controlAction: controlAction,
                            controlContainerCss: controlContainerCss,
                            tagControlContainer: tagControlContainer),
                    actionOptions: actionOptions)
                : hb;
        }

        public static HtmlBuilder FieldContainer(
            this HtmlBuilder hb,
            string fieldId = null,
            string fieldCss = null,
            Action actionOptions = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    id: fieldId,
                    css: Css.Class("field-normal", fieldCss), action: () =>
                    {
                        actionOptions?.Invoke();
                    })
                : hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            string fieldId = null,
            string fieldCss = null,
            Action actionLabel = null,
            Action actionControl = null,
            Action actionOptions = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    id: fieldId,
                    css: Css.Class("field-normal", fieldCss), action: () =>
                    {
                        actionLabel();
                        actionControl();
                        actionOptions?.Invoke();
                    })
                : hb;
        }

        private static HtmlBuilder Label(
            this HtmlBuilder hb,
            string controlId = null,
            string labelCss = null,
            string labelText = null,
            string labelTitle = null)
        {
            return labelText != string.Empty
                ? hb.P(css: "field-label", action: () => hb
                    .Label(
                        attributes: new HtmlAttributes()
                            .For(controlId)
                            .Class(labelCss)
                            .Title(labelTitle),
                        action: () => hb
                            .Text(labelText)))
                : hb;
        }

        private static HtmlBuilder Control(
            this HtmlBuilder hb,
            Action controlAction,
            string controlContainerCss,
            string tagControlContainer)
        {
            return hb.Div(
                css: "field-control",
                action: () => hb
                    .Append(
                        tag: tagControlContainer,
                        id: null,
                        css: Css.Class("container-normal", controlContainerCss),
                        attributes: null,
                        action: () =>
                            controlAction()));
        }

        public static HtmlBuilder FieldText(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            string text = null,
            string dataValue = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .Span(
                            attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Class(Css.Class("control-text", controlCss))
                                .Title(text)
                                .DataValue(dataValue),
                            action: () => hb
                                .Text(text: text)),
                    controlContainerCss: controlContainerCss)
                : hb;
        }

        public static HtmlBuilder FieldTextBox(
            this HtmlBuilder hb,
            HtmlTypes.TextTypes textType = HtmlTypes.TextTypes.Normal,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            string text = null,
            string unit = null,
            string format = null,
            bool timepiker = false,
            string onChange = null,
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
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .TextBox(
                            textType: textType,
                            controlId: controlId,
                            controlCss: controlCss +
                                (!unit.IsNullOrEmpty()
                                    ? " with-unit"
                                    : string.Empty),
                            text: text,
                            placeholder: labelText,
                            onChange: onChange,
                            format: format,
                            timepicker: timepiker,
                            validateRequired: validateRequired,
                            validateNumber: validateNumber,
                            validateMinNumber: validateMinNumber,
                            validateMaxNumber: validateMaxNumber,
                            validateDate: validateDate,
                            validateEmail: validateEmail,
                            validateEqualTo: validateEqualTo,
                            validateMaxLength: validateMaxLength,
                            action: action,
                            method: method,
                            attributes: attributes)
                        .Span(css: "unit", _using: !unit.IsNullOrEmpty(), action: () => hb
                            .Text(unit)))
                : hb;
        }

        public static HtmlBuilder FieldMarkDown(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            string text = null,
            string placeholder = null,
            bool readOnly = false,
            bool validateRequired = false,
            Dictionary<string, string> attributes = null,
            bool preview = false,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .MarkDown(
                            controlId: controlId,
                            controlCss: controlCss,
                            text: text,
                            placeholder: placeholder,
                            readOnly: readOnly,
                            validateRequired: validateRequired,
                            attributes: attributes,
                            preview: preview))
                : hb;
        }

        public static HtmlBuilder FieldMarkUp(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            string text = null,
            Dictionary<string, string> attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .MarkUp(
                            controlId: controlId,
                            controlCss: controlCss,
                            text: text,
                            attributes: attributes))
                : hb;
        }

        public static HtmlBuilder FieldDropDown(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            Dictionary<string, string> optionCollection = null,
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
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .DropDown(
                            controlId: controlId,
                            controlCss: controlCss,
                            optionCollection: optionCollection?.ToDictionary(
                                o => o.Key, o => new ControlData(o.Value)),
                            selectedValue: selectedValue,
                            multiple: multiple,
                            addSelectedValue: addSelectedValue,
                            insertBlank: insertBlank,
                            disabled: disabled,
                            onChange: onChange,
                            validateRequired: validateRequired,
                            action: action,
                            method: method,
                            column: column))
                : hb;
        }

        public static HtmlBuilder FieldDropDown(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
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
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .DropDown(
                            controlId: controlId,
                            controlCss: controlCss,
                            optionCollection: optionCollection,
                            selectedValue: selectedValue,
                            multiple: multiple,
                            addSelectedValue: addSelectedValue,
                            insertBlank: insertBlank,
                            disabled: disabled,
                            onChange: onChange,
                            validateRequired: validateRequired,
                            action: action,
                            method: method,
                            column: column))
                : hb;
        }

        public static HtmlBuilder FieldCheckBox(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            bool _checked = false,
            bool disabled = false,
            string dataId = null,
            string onChange = null,
            string action = null,
            string method = null,
            bool labelPositionIsRight = true,
            bool _using = true)
        {
            if (!_using) return hb;
            if (!labelPositionIsRight)
            {
                return hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .CheckBox(
                            controlId: controlId,
                            controlCss: controlCss,
                            _checked: _checked,
                            disabled: disabled,
                            dataId: dataId,
                            onChange: onChange,
                            action: action,
                            method: method));
            }
            else
            {
                return hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: string.Empty,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .CheckBox(
                            controlId: controlId,
                            controlCss: controlCss,
                            labelText: labelText,
                            _checked: _checked,
                            disabled: disabled,
                            onChange: onChange,
                            action: action,
                            method: method));
            }
        }

        public static HtmlBuilder FieldRadio(
            this HtmlBuilder hb,
            string fieldId = null,
            string name = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValueText = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .RadioButtons(
                            name: name,
                            controlCss: controlCss,
                            optionCollection: optionCollection,
                            selectedValue: selectedValueText))
                : hb;
        }

        public static HtmlBuilder FieldSpinner(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            decimal? value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            int width = 50,
            string unit = null,
            string onChange = null,
            string action = null,
            string method = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () =>
                    {
                        hb.Spinner(
                            controlId: controlId,
                            controlCss: controlCss,
                            value: value,
                            min: min,
                            max: max,
                            step: step,
                            width: width,
                            onChange: onChange,
                            action: action,
                            method: method);
                        if (unit != string.Empty)
                        {
                            hb.Span(css: "unit", action: () => hb
                                .Text(unit));
                        }
                    })
                : hb;
        }

        public static HtmlBuilder FieldSlider(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
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
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .Slider(
                            controlId: controlId,
                            controlCss: controlCss,
                            value: value,
                            min: min,
                            max: max,
                            step: step,
                            unit: unit,
                            action: action,
                            method: method))
                : hb;
        }

        public static HtmlBuilder FieldAnchor(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = "field-auto",
            string controlContainerCss = null,
            string controlCss = null,
            string iconCss = null,
            string text = null,
            string href = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    controlContainerCss: controlContainerCss,
                    labelText: string.Empty,
                    controlAction: () =>
                    {
                        if (iconCss != string.Empty) hb.Icon(iconCss: iconCss);
                        hb.Anchor(
                            controlId: controlId,
                            controlCss: controlCss,
                            text: text,
                            href: href);
                    })
                : hb;
        }

        public static HtmlBuilder FieldSelectable(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlWrapperCss = null,
            string controlCss = null,
            string labelText = null,
            string labelTitle = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool commandOptionPositionIsTop = false,
            string action = null,
            string method = null,
            bool _using = true,
            Action commandOptionAction = null)
        {
            if (!_using) return hb;
            if (!commandOptionPositionIsTop)
            {
                return hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => hb
                        .SelectableWrapper(
                            controlId: controlId,
                            controlWrapperCss: controlWrapperCss,
                            controlCss: controlCss,
                            listItemCollection: listItemCollection,
                            selectedValueCollection: selectedValueCollection,
                            action: action,
                            method: method),
                    actionOptions: commandOptionAction);
            }
            else
            {
                return hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelTitle: labelTitle,
                    controlAction: () => 
                    {
                        commandOptionAction();
                        hb.SelectableWrapper(
                            controlId: controlId,
                            controlWrapperCss: controlWrapperCss,
                            controlCss: controlCss,
                            listItemCollection: listItemCollection,
                            selectedValueCollection: selectedValueCollection,
                            action: action,
                            method: method);
                    });
            }
        }

        public static HtmlBuilder FieldBasket(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string title = null,
            Action labelAction = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool _using = true,
            Action actionOptions = null)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    actionLabel: () => hb
                        .Div(css: "field-label", action: labelAction),
                    actionControl: () => hb
                        .Basket(
                            controlId: controlId,
                            controlCss: controlCss,
                            listItemCollection: listItemCollection,
                            selectedValueCollection: selectedValueCollection),
                    actionOptions: actionOptions)
                : hb;
        }
    }
}