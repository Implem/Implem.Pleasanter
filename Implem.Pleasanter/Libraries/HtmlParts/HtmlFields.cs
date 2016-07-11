using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Libraries.Validators;
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
            SiteSettings siteSettings,
            Column column,
            BaseModel.MethodTypes methodType = BaseModel.MethodTypes.NotSet,
            string value = "",
            Permissions.ColumnPermissionTypes columnPermissionType = 
                Permissions.ColumnPermissionTypes.Update,
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            bool _using = true)
        {
            if (columnPermissionType != Permissions.ColumnPermissionTypes.Deny && _using)
            {
                return hb.SwitchField(
                    column: column,
                    columnPermissionType: columnPermissionType,
                    controlId: column.Id,
                    fieldCss: Strings.CoalesceEmpty(fieldCss, column.FieldCss),
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    controlCss: Strings.CoalesceEmpty(controlCss, column.ControlCss),
                    controlType: ControlType(column),
                    value: methodType == BaseModel.MethodTypes.New
                        ? value.ToDefault(siteSettings, column)
                        : value,
                    optionCollection: column.EditChoices(siteSettings.InheritPermission),
                    attributes: ClientValidators.MessageCollection(column.Validators));
            }
            else
            {
                return hb;
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
            Dictionary<string, string> attributes)
        {
            switch (columnPermissionType)
            {
                case Permissions.ColumnPermissionTypes.Read:
                    switch (controlType)
                    {
                        case ControlTypes.CheckBox:
                            return hb.FieldCheckBox(
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                _checked: value.ToBool(),
                                disabled: true);
                        default:
                            return hb.FieldText(
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
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                optionCollection: optionCollection,
                                selectedValue: value,
                                column: column);
                        case ControlTypes.Text:
                            return hb.FieldText(
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
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                attributes: attributes);
                        case ControlTypes.MarkDown:
                            return hb.FieldMarkDown(
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                placeholder: column.LabelText,
                                readOnly: column.EditorReadOnly.ToBool(),
                                attributes: attributes);
                        case ControlTypes.TextBox:
                            return hb.FieldTextBox(
                                textType: column.Hash
                                    ? HtmlTypes.TextTypes.Password
                                    : HtmlTypes.TextTypes.Normal,
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                attributes: attributes);
                        case ControlTypes.TextBoxNumeric:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.Normal,
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value.ToDecimal() != 0
                                    ? column.Format(value.ToDecimal())
                                    : string.Empty,
                                attributes: attributes);
                        case ControlTypes.TextBoxDateTime:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.Normal,
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                text: value,
                                attributes: attributes);
                        case ControlTypes.CheckBox:
                            return hb.FieldCheckBox(
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
                                controlId: controlId,
                                fieldCss: fieldCss,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: column.LabelText,
                                value: value,
                                min: column.Min.ToDecimal(),
                                max: column.Max.ToDecimal(),
                                step: column.Step.ToDecimal(),
                                unit: column.Unit);
                        case ControlTypes.Spinner:
                            return hb.FieldSpinner(
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

        private static string ToDefault(
            this string self, SiteSettings siteSettings, Column column)
        {
            if (IsLinked(siteSettings, column))
            {
                return Forms.Data("LinkId");
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
                        return !self.ToDateTime().NotZero()
                            ? DateTime.Now
                                .ToLocal()
                                .AddDays(column.DefaultInput.ToInt())
                                .ToString(Displays.Get(column.ControlDateTime + "Format"))
                            : self;
                    default:
                        return column.DefaultInput;
                }
            }
            return self;
        }

        private static bool IsLinked(SiteSettings siteSettings, Column column)
        {
            var fromSiteId = Forms.Data("FromSiteId");
            return
                fromSiteId != string.Empty &&
                siteSettings.LinkColumnSiteIdHash.ContainsKey(
                    column.ColumnName + "_" + fromSiteId);
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
                        return column.MarkDown
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
                case Types.CsString:
                    return column.HasChoices()
                        ? ControlTypes.DropDown
                        : column.MarkDown
                            ? ControlTypes.MarkDown
                            : column.Max.ToInt() == -1 ||
                              column.Max.ToInt() >= Parameters.General.SizeToUseTextArea
                                ? ControlTypes.TextBoxMultiLine
                                : ControlTypes.TextBox;
                case Types.CsNumeric:
                    return column.HasChoices()
                        ? ControlTypes.DropDown
                        : ControlTypes.TextBoxNumeric;
                case Types.CsDateTime:
                    return ControlTypes.TextBoxDateTime;
                case Types.CsBool:
                    return ControlTypes.CheckBox;
                default:
                    return ControlTypes.Text;
            }
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string labelText = "",
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
                            labelText: labelText),
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
            string fieldId = "",
            string fieldCss = "",
            Action actionOptions = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    id: fieldId,
                    css: Css.Class("field-normal", fieldCss), action: () =>
                    {
                        if (actionOptions != null) actionOptions();
                    })
                : hb;
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            string fieldId = "",
            string fieldCss = "",
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
                        if (actionOptions != null) actionOptions();
                    })
                : hb;
        }

        private static HtmlBuilder Label(
            this HtmlBuilder hb,
            string controlId = "",
            string labelCss = "",
            string labelText = "")
        {
            return labelText != string.Empty
                ? hb.P(css: "field-label", action: () => hb
                    .Label(
                        attributes: new HtmlAttributes()
                            .For(controlId)
                            .Class(labelCss),
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
                        css: Css.Class("container-normal", controlContainerCss),
                        action: () =>
                            controlAction()));
        }

        public static HtmlBuilder FieldText(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            string text = "",
            string dataValue = "",
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    labelText: labelText,
                    controlAction: () => hb
                        .Span(
                            attributes: new HtmlAttributes()
                                .Id_Css(controlId, Css.Class("control-text", controlCss))
                                .DataValue(dataValue),
                            action: () => hb
                                .Text(text: text)),
                    controlContainerCss: controlContainerCss)
                : hb;
        }

        public static HtmlBuilder FieldTextBox(
            this HtmlBuilder hb,
            HtmlTypes.TextTypes textType = HtmlTypes.TextTypes.Normal,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            string text = "",
            string onChange = "",
            string action = "",
            string method = "", 
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
                    controlAction: () => hb
                        .TextBox(
                            textType: textType,
                            controlId: controlId,
                            controlCss: controlCss,
                            text: text,
                            placeholder: labelText,
                            onChange: onChange,
                            action: action,
                            method: method,
                            attributes: attributes))
                : hb;
        }

        public static HtmlBuilder FieldMarkDown(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            string text = "",
            string placeholder = "",
            bool readOnly = false,
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
                    controlAction: () => hb
                        .MarkDown(
                            controlId: controlId,
                            controlCss: controlCss,
                            text: text,
                            placeholder: placeholder,
                            readOnly: readOnly,
                            attributes: attributes))
                : hb;
        }

        public static HtmlBuilder FieldMarkUp(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            string text = "",
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
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            Dictionary<string, string> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            string onChange = "",
            string action = "",
            string method = "",
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
                    controlAction: () => hb
                        .DropDown(
                            controlId: controlId,
                            controlCss: controlCss,
                            optionCollection: optionCollection?.ToDictionary(
                                o => o.Key, o => new ControlData(o.Value)),
                            selectedValue: selectedValue,
                            addSelectedValue: addSelectedValue,
                            onChange: onChange,
                            action: action,
                            method: method,
                            column: column))
                : hb;
        }

        public static HtmlBuilder FieldDropDown(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = "",
            bool addSelectedValue = true,
            string onChange = "",
            string action = "",
            string method = "",
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
                    controlAction: () => hb
                        .DropDown(
                            controlId: controlId,
                            controlCss: controlCss,
                            optionCollection: optionCollection,
                            selectedValue: selectedValue,
                            addSelectedValue: addSelectedValue,
                            onChange: onChange,
                            action: action,
                            method: method,
                            column: column))
                : hb;
        }

        public static HtmlBuilder FieldCheckBox(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            bool _checked = false,
            bool disabled = false,
            string action = "",
            string method = "",
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
                    controlAction: () => hb
                        .CheckBox(
                            controlId: controlId,
                            controlCss: controlCss,
                            _checked: _checked,
                            disabled: disabled,
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
                    controlAction: () => hb
                        .CheckBox(
                            controlId: controlId,
                            controlCss: controlCss,
                            labelText: labelText,
                            _checked: _checked,
                            disabled: disabled,
                            action: action,
                            method: method));
            }
        }

        public static HtmlBuilder FieldRadio(
            this HtmlBuilder hb,
            string fieldId = "",
            string name = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValueText = "",
            string action = "",
            string method = "",
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
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
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            decimal? value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            int width = 50,
            string unit = "",
            string onChange = "",
            string action = "",
            string method = "",
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
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
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
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
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
            string fieldId = "",
            string controlId = "",
            string fieldCss = "field-auto",
            string controlContainerCss = "",
            string controlCss = "",
            string iconCss = "",
            string text = "",
            string href = "",
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
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            string labelText = "",
            Dictionary<string, string> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool commandOptionPositionIsTop = false,
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
                    controlAction: () => hb
                        .Selectable(
                            controlId: controlId,
                            controlCss: controlCss,
                            listItemCollection: 
                                listItemCollection ?? new Dictionary<string, string>(),
                            selectedValueCollection: 
                                selectedValueCollection ?? new List<string>()),
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
                    controlAction: () => 
                    {
                        commandOptionAction();
                        hb.Selectable(
                            controlId: controlId,
                            controlCss: controlCss,
                            listItemCollection: 
                                listItemCollection ?? new Dictionary<string, string>(),
                            selectedValueCollection: 
                                selectedValueCollection ?? new List<string>());
                    });
            }
        }

        public static HtmlBuilder FieldBasket(
            this HtmlBuilder hb,
            string fieldId = "",
            string controlId = "",
            string fieldCss = "",
            string labelCss = "",
            string controlContainerCss = "",
            string controlCss = "",
            Action labelAction = null,
            Dictionary<string, string> listItemCollection = null,
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
                            listItemCollection: 
                                listItemCollection ?? new Dictionary<string, string>(),
                            selectedValueCollection: 
                                selectedValueCollection ?? new List<string>()),
                    actionOptions: actionOptions)
                : hb;
        }
    }
}