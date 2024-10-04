using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlFields
    {
        public enum ControlTypes
        {
            Text,
            TextBox,
            TextBoxNumeric,
            TextBoxDateTime,
            TextBoxMultiLine,
            MarkDown,
            DropDown,
            Radio,
            CheckBox,
            Slider,
            Spinner,
            Attachments
        }

        public static HtmlBuilder Field(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            ServerScriptModelColumn serverScriptModelColumn = null,
            string value = null,
            StatusControl.ControlConstraintsTypes controlConstraintsType = StatusControl.ControlConstraintsTypes.None,
            Permissions.ColumnPermissionTypes columnPermissionType = Permissions.ColumnPermissionTypes.Update,
            string fieldCss = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            bool controlOnly = false,
            bool alwaysSend = false,
            bool disableAutoPostBack = false,
            string idSuffix = null,
            bool preview = false,
            bool disableSection = false,
            bool _using = true)
        {
            if (columnPermissionType == Permissions.ColumnPermissionTypes.Deny || !_using)
            {
                return hb;
            }
            if (column.Type == Column.Types.User
                && column.MultipleSelections != true
                && SiteInfo.User(
                    context: context,
                    userId: value.ToInt()).Anonymous())
            {
                value = string.Empty;
            }
            if (column.Section != null && !controlOnly && !disableSection)
            {
                hb.Div(
                    css: "field-section",
                    action: () => hb
                        .Text(text: column.Section));
            }
            return hb
                .Raw(HtmlHtmls.ExtendedHtmls(
                    context: context,
                    id: "ColumnTop",
                    columnName: column.ColumnName))
                .Raw(Strings.CoalesceEmpty(
                    serverScriptModelColumn?.ExtendedHtmlBeforeField,
                    column.ExtendedHtmlBeforeField))
                .SwitchField(
                    context: context,
                    ss: ss,
                    column: column,
                    columnPermissionType: context.Publish
                        || column.GetEditorReadOnly()
                        || controlConstraintsType == StatusControl.ControlConstraintsTypes.ReadOnly
                            ? Permissions.ColumnPermissionTypes.Read
                            : columnPermissionType,
                    controlId: !preview
                        ? $"{column.Id}{idSuffix}"
                        : null,
                    columnName: $"{column.ColumnName}{idSuffix}",
                    fieldCss: FieldCss(
                        column: column,
                        serverScriptModelColumn: serverScriptModelColumn,
                        controlConstraintsType: controlConstraintsType,
                        fieldCss: fieldCss),
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    controlCss: ControlCss(
                        column: column,
                        serverScriptModelColumn: serverScriptModelColumn,
                        disableAutoPostBack: disableAutoPostBack,
                        controlCss: controlCss),
                    controlType: ControlType(column),
                    labelText: Strings.CoalesceEmpty(
                        serverScriptModelColumn?.LabelText,
                        column.LabelText),
                    placeholder: Strings.CoalesceEmpty(
                        column.InputGuide,
                        serverScriptModelColumn?.LabelText,
                        column.LabelText),
                    labelRaw: serverScriptModelColumn?.LabelRaw,
                    value: value,
                    optionCollection: EditChoices(
                        context: context,
                        ss: ss,
                        column: column,
                        value: value),
                    mobile: context.Mobile,
                    controlOnly: controlOnly,
                    alwaysSend: alwaysSend,
                    required: column.Required
                        || column.GetValidateRequired()
                        || controlConstraintsType == StatusControl.ControlConstraintsTypes.Required,
                    preview: preview,
                    inputGuide: column.InputGuide,
                    extendedHtmlBeforeLabel: Strings.CoalesceEmpty(
                        serverScriptModelColumn?.ExtendedHtmlBeforeLabel,
                        column.ExtendedHtmlBeforeLabel),
                    extendedHtmlBetweenLabelAndControl: Strings.CoalesceEmpty(
                        serverScriptModelColumn?.ExtendedHtmlBetweenLabelAndControl,
                        column.ExtendedHtmlBetweenLabelAndControl),
                    extendedHtmlAfterControl: Strings.CoalesceEmpty(
                        serverScriptModelColumn?.ExtendedHtmlAfterControl,
                        column.ExtendedHtmlAfterControl))
                .Raw(Strings.CoalesceEmpty(
                    serverScriptModelColumn?.ExtendedHtmlAfterField,
                    column.ExtendedHtmlAfterField))
                .Raw(HtmlHtmls.ExtendedHtmls(
                    context: context,
                    id: "ColumnBottom",
                    columnName: column.ColumnName));
        }

        private static string FieldCss(
            Column column,
            ServerScriptModelColumn serverScriptModelColumn,
            StatusControl.ControlConstraintsTypes controlConstraintsType = StatusControl.ControlConstraintsTypes.None,
            string fieldCss = null)
        {
            var extendedFieldCss = Strings.CoalesceEmpty(
                serverScriptModelColumn?.ExtendedFieldCss,
                column.ExtendedFieldCss);
            var css = Strings.CoalesceEmpty(fieldCss, column.FieldCss)
                + (column.NoWrap == true
                    ? " both"
                    : string.Empty)
                + (column.GetHide() || controlConstraintsType == StatusControl.ControlConstraintsTypes.Hidden
                    ? " hidden"
                    : string.Empty)
                + (column.TextAlign switch
                    {
                        SiteSettings.TextAlignTypes.Right => " right-align",
                        SiteSettings.TextAlignTypes.Center => " center-align",
                        _ => string.Empty
                    })
                + (!extendedFieldCss.IsNullOrEmpty()
                    ? " " + extendedFieldCss
                    : string.Empty);
            if (column.ChoicesControlType == "Radio")
            {
                // ラジオボタンのスタイルはワイドのみ使用可能
                css = (css + " field-wide").Trim();
            }
            return css;
        }

        private static string ControlCss(
            Column column,
            ServerScriptModelColumn serverScriptModelColumn,
            bool disableAutoPostBack,
            string controlCss)
        {
            var extendedControlCss = Strings.CoalesceEmpty(
                serverScriptModelColumn?.ExtendedControlCss,
                column.ExtendedControlCss);
            return Strings.CoalesceEmpty(controlCss, column.ControlCss)
                + (!disableAutoPostBack && column.AutoPostBack == true
                    ? " control-auto-postback"
                    : string.Empty)
                + (column.TextAlign switch
                    {
                        SiteSettings.TextAlignTypes.Right => " right-align",
                        SiteSettings.TextAlignTypes.Center => " center-align",
                        _ => string.Empty
                    })
            + (!extendedControlCss.IsNullOrEmpty()
                    ? " " + extendedControlCss
                    : string.Empty);
        }

        public static Dictionary<string, ControlData> EditChoices(
            Context context,
            SiteSettings ss,
            Column column,
            string value,
            bool own = false,
            bool multiple = false,
            bool addNotSet = false)
        {
            var editChoices = column.EditChoices(
                context: context,
                addNotSet: addNotSet,
                own: own);
            if (column.Linked(
                context: context,
                withoutWiki: true))
            {
                SelectedValues(
                    column: column,
                    value: value)
                        .Where(referenceId => referenceId > 0)
                        .Where(referenceId => !editChoices.ContainsKey(referenceId.ToString()))
                        .ForEach(referenceId =>
                        {
                            var title = ss.LinkedItemTitle(
                                context: context,
                                referenceId: referenceId,
                                siteIdList: ss.Links
                                    .Where(o => o.SiteId > 0)
                                    .Select(o => o.SiteId)
                                    .ToList());
                            if (title != null)
                            {
                                editChoices.Add(referenceId.ToString(), new ControlData(title));
                            }
                        });
            }
            if (column.UseSearch == true)
            {
                switch (column.Type)
                {
                    case Column.Types.Dept:
                        (column.MultipleSelections == true || multiple
                            ? value.Deserialize<List<string>>()
                                ?.Select(deptId => deptId.ToInt())
                                .ToList()
                                    ?? new List<int>()
                            : value.ToInt().ToSingleList())
                                .Select(deptId => SiteInfo.Dept(
                                    tenantId: context.TenantId,
                                    deptId: deptId))
                                .ForEach(dept =>
                                    editChoices.AddIfNotConainsKey(
                                        dept.Id.ToString(),
                                        new ControlData(dept.Name)));
                        break;
                    case Column.Types.Group:
                        (column.MultipleSelections == true || multiple
                            ? value.Deserialize<List<string>>()
                                ?.Select(groupId => groupId.ToInt())
                                .ToList()
                                    ?? new List<int>()
                            : value.ToInt().ToSingleList())
                                .Select(groupId => SiteInfo.Group(
                                    tenantId: context.TenantId,
                                    groupId: groupId))
                                .ForEach(group =>
                                    editChoices.AddIfNotConainsKey(
                                        group.Id.ToString(),
                                        new ControlData(group.Name)));
                        break;
                    case Column.Types.User:
                        (column.MultipleSelections == true || multiple
                            ? value.Deserialize<List<string>>()
                                ?.Select(userId => userId.ToInt())
                                .ToList()
                                    ?? new List<int>()
                            : value.ToInt().ToSingleList())
                                .Select(userId => SiteInfo.User(
                                    context: context,
                                    userId: userId))
                                .Where(user => !user.Anonymous())
                                .ForEach(user =>
                                    editChoices.AddIfNotConainsKey(
                                        user.Id.ToString(),
                                        new ControlData(user.Name)));
                        break;
                    default:
                        break;
                }
            }
            return editChoices;
        }

        private static List<long> SelectedValues(Column column, string value)
        {
            if (value.IsNullOrEmpty()) return new List<long>();
            return column.MultipleSelections == true
                ? value.Deserialize<List<long>>()
                    ?? new List<long>()
                : value.ToLong().ToSingleList();
        }

        public static HtmlBuilder ViewExtensionField(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            View view)
        {
            var defaultValue = column.DefaultInput;
            if (ControlType(column) == ControlTypes.TextBoxDateTime)
            {
                defaultValue = column
                    .DefaultTime(context: context)
                    .ToControl(
                        context: context,
                        ss: ss,
                        column: column);
            }
            var value = view.ViewExtension(column.ColumnName) ?? defaultValue;
            hb.SwitchField(
                context: context,
                ss: ss,
                column: column,
                columnPermissionType: Permissions.ColumnPermissionTypes.Update,
                controlId: "ViewExtensions__" + column.ColumnName,
                columnName: column.ColumnName,
                fieldCss: FieldCss(
                    column: column,
                    serverScriptModelColumn: null),
                labelCss: null,
                controlContainerCss: null,
                controlCss: ControlCss(
                    column: column,
                    serverScriptModelColumn: null,
                    disableAutoPostBack: false,
                    controlCss: null),
                controlType: ControlType(column),
                labelText: column.LabelText,
                placeholder: Strings.CoalesceEmpty(column.InputGuide, column.LabelText),
                labelRaw: null,
                value: value,
                optionCollection: EditChoices(
                    context: context,
                    ss: ss,
                    column: column,
                    value: value),
                mobile: context.Mobile,
                controlOnly: false,
                alwaysSend: false,
                required: column.Required
                    || column.GetValidateRequired(),
                preview: false,
                inputGuide: column.InputGuide,
                extendedHtmlBeforeLabel: Strings.CoalesceEmpty(
                    null,
                    column.ExtendedHtmlBeforeLabel),
                extendedHtmlBetweenLabelAndControl: Strings.CoalesceEmpty(
                    null,
                    column.ExtendedHtmlBetweenLabelAndControl),
                extendedHtmlAfterControl: Strings.CoalesceEmpty(
                    null,
                    column.ExtendedHtmlAfterControl));
            return hb;
        }

        private static HtmlBuilder SwitchField(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column column,
            Permissions.ColumnPermissionTypes columnPermissionType,
            string controlId,
            string columnName,
            string fieldCss,
            string labelCss,
            string controlContainerCss,
            string controlCss,
            ControlTypes controlType,
            string labelText,
            string placeholder,
            string labelRaw,
            string value,
            Dictionary<string, ControlData> optionCollection,
            bool mobile,
            bool controlOnly,
            bool alwaysSend,
            bool required,
            bool preview,
            string inputGuide,
            string extendedHtmlBeforeLabel,
            string extendedHtmlBetweenLabelAndControl,
            string extendedHtmlAfterControl)
        {
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
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                _checked: value.ToBool(),
                                disabled: true,
                                alwaysSend: alwaysSend,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.MarkDown:
                            return hb.FieldMarkDown(
                                context: context,
                                ss: ss,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value,
                                readOnly: true,
                                allowImage: column.AllowImage == true,
                                mobile: mobile,
                                alwaysSend: alwaysSend,
                                preview: preview,
                                validateMaxLength: column.MaxLength.ToInt(),
                                validateRegex: column.ClientRegexValidation,
                                validateRegexErrorMessage: column.RegexValidationMessage,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.Attachments:
                            return hb.FieldAttachments(
                                context: context,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                columnName: columnName,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                value: value,
                                readOnly: true,
                                preview: preview,
                                inputGuide: column.InputGuide,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        default:
                            return hb.FieldText(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss + (column.HasChoices()
                                    ? optionCollection
                                        .Where(o => o.Key == value)
                                        .Select(o => o.Value?.Css.IsNullOrEmpty() != true
                                            ? $" {o.Value.Css}"
                                            : string.Empty)
                                        .FirstOrDefault()
                                    : string.Empty),
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value.ToDisplay(
                                    context: context,
                                    ss: ss,
                                    column: column),
                                dataValue: column.HasChoices()
                                    ? value
                                    : null,
                                openAnchorNewTab: column.OpenAnchorNewTab == true,
                                anchorFormat: column.Anchor == true
                                    ? column.AnchorFormat
                                    : string.Empty,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                    }
                case Permissions.ColumnPermissionTypes.Update:
                    switch (controlType)
                    {
                        case ControlTypes.DropDown:
                            return hb.FieldDropDown(
                                context: context,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss +
                                    (column.UseSearch == true
                                        ? " search"
                                        : string.Empty),
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                optionCollection: optionCollection,
                                selectedValue: value,
                                multiple: column.MultipleSelections == true,
                                insertBlank: column.MultipleSelections != true
                                    && column.NotInsertBlankChoice != true,
                                alwaysSend: alwaysSend,
                                validateRequired: required,
                                column: column,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl,
                                controlOption: () =>
                                {
                                    if (column.MultipleSelections != true
                                        && column.ExcludeMe != true)
                                    {
                                        hb
                                            .Div(
                                                css: "ui-icon ui-icon-person current-user",
                                                _using: !Parameters.General.HideCurrentUserIcon
                                                    && column.Type == Column.Types.User)
                                            .Div(
                                                css: "ui-icon ui-icon-person current-dept",
                                                _using: !Parameters.General.HideCurrentDeptIcon
                                                    && column.Type == Column.Types.Dept);
                                    }
                                });
                        case ControlTypes.Radio:
                            return hb.FieldRadio(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: "container-normal container-radio",
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                optionCollection: optionCollection,
                                alwaysSend: alwaysSend,
                                validateRequired: required,
                                selectedValue: value,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.Text:
                            return hb.FieldText(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.TextBoxMultiLine:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.MultiLine,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                placeholder: placeholder,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value,
                                alwaysSend: alwaysSend,
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: !column.MaxLength.ToString().IsNullOrEmpty()
                                    ? column.MaxLength.ToInt()
                                    : column.ValidateMaxLength ?? 0,
                                validateRegex: column.ClientRegexValidation,
                                validateRegexErrorMessage: column.RegexValidationMessage,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.MarkDown:
                            return hb.FieldMarkDown(
                                context: context,
                                ss: ss,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                placeholder: placeholder,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value,
                                readOnly: column.GetEditorReadOnly(),
                                allowBulkUpdate: column.AllowBulkUpdate == true,
                                allowImage: column.AllowImage == true,
                                mobile: mobile,
                                alwaysSend: alwaysSend,
                                validateRequired: required,
                                viewerSwitchingTypes: (Column.ViewerSwitchingTypes)column.ViewerSwitchingType,
                                preview: preview,
                                validateMaxLength: column.MaxLength.ToInt(),
                                validateRegex: column.ClientRegexValidation,
                                validateRegexErrorMessage: column.RegexValidationMessage,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.TextBox:
                            return hb.FieldTextBox(
                                textType: column.Hash
                                    ? HtmlTypes.TextTypes.Password
                                    : HtmlTypes.TextTypes.Normal,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                placeholder: placeholder,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value,
                                alwaysSend: alwaysSend,
                                openAnchorNewTab: column.OpenAnchorNewTab == true,
                                anchorFormat: column.Anchor == true
                                    ? column.AnchorFormat
                                    : string.Empty,
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: !column.MaxLength.ToString().IsNullOrEmpty()
                                    ? column.MaxLength.ToInt()
                                    : column.ValidateMaxLength ?? 0,
                                validateRegex: column.ClientRegexValidation,
                                validateRegexErrorMessage: column.RegexValidationMessage,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.TextBoxNumeric:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.Normal,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                placeholder: placeholder,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                unit: column.Unit,
                                text: value,
                                alwaysSend: alwaysSend,
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
                                validateMaxLength: !column.MaxLength.ToString().IsNullOrEmpty()
                                    ? column.MaxLength.ToInt()
                                    : column.ValidateMaxLength ?? 0,
                                validateRegex: column.ClientRegexValidation,
                                validateRegexErrorMessage: column.RegexValidationMessage,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.TextBoxDateTime:
                            return hb.FieldTextBox(
                                textType: HtmlTypes.TextTypes.DateTime,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                placeholder: placeholder,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                text: value,
                                format: column.DateTimeFormat(context: context),
                                timepiker: column.DateTimepicker(),
                                alwaysSend: alwaysSend,
                                validateRequired: required,
                                validateNumber: column.ValidateNumber ?? false,
                                validateDate: column.ValidateDate ?? false,
                                validateEmail: column.ValidateEmail ?? false,
                                validateEqualTo: column.ValidateEqualTo,
                                validateMaxLength: !column.MaxLength.ToString().IsNullOrEmpty()
                                    ? column.MaxLength.ToInt()
                                    : column.ValidateMaxLength ?? 0,
                                validateRegex: column.ClientRegexValidation,
                                validateRegexErrorMessage: column.RegexValidationMessage,
                                attributes: column.DateTimeStep == null
                                    ? null
                                    : new Dictionary<string, string>()
                                    {
                                        { "data-step", column.DateTimeStep?.ToString() }
                                    },
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl,
                                controlOption: () => hb
                                    .Div(
                                        css: "ui-icon ui-icon-clock current-time",
                                        _using: !Parameters.General.HideCurrentTimeIcon));
                        case ControlTypes.CheckBox:
                            return hb.FieldCheckBox(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                _checked: value.ToBool(),
                                disabled: column.GetEditorReadOnly(),
                                alwaysSend: alwaysSend,
                                validateRequired: required,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.Slider:
                            return hb.FieldSlider(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                value: value.ToDecimal(),
                                min: column.Min.ToDecimal(),
                                max: column.Max.ToDecimal(),
                                step: column.Step.ToDecimal(),
                                unit: column.Unit,
                                alwaysSend: alwaysSend,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.Spinner:
                            return hb.FieldSpinner(
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                controlCss: controlCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                value: !value.IsNullOrEmpty()
                                    ? (decimal?)value.ToDecimal()
                                    : null,
                                min: column.MinNumber(),
                                max: column.MaxNumber(),
                                step: column.Step.ToDecimal(),
                                width: 50,
                                unit: column.Unit,
                                alwaysSend: alwaysSend,
                                allowBlank: true,
                                validateRequired: required,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        case ControlTypes.Attachments:
                            return hb.FieldAttachments(
                                context: context,
                                fieldId: controlId + "Field",
                                controlId: controlId,
                                columnName: columnName,
                                fieldCss: fieldCss,
                                fieldDescription: column.Description,
                                labelCss: labelCss,
                                controlContainerCss: controlContainerCss,
                                labelText: labelText,
                                labelRaw: labelRaw,
                                controlOnly: controlOnly,
                                value: value,
                                readOnly: column.GetEditorReadOnly(),
                                allowDelete: column.AllowDeleteAttachments != false,
                                preview: preview,
                                validateRequired: required,
                                inputGuide: column.InputGuide,
                                extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: extendedHtmlAfterControl);
                        default:
                            return hb;
                    }
                default:
                    return hb;
            }
        }

        private static ControlTypes ControlType(Column column)
        {
            if (column.GetEditorReadOnly())
            {
                switch (column.TypeName.CsTypeSummary())
                {
                    case Types.CsBool:
                        return ControlTypes.CheckBox;
                    default:
                        switch (column.ControlType)
                        {
                            case "MarkDown":
                                return column.FieldCss == "field-markdown"
                                    ? ControlTypes.MarkDown
                                    : ControlTypes.Text;
                            case "Attachments":
                                return ControlTypes.Attachments;
                            default:
                                return ControlTypes.Text;
                        }
                }
            }
            switch (column.ControlType)
            {
                case "TextBox": return ControlTypes.TextBox;
                case "Id": return ControlTypes.Text;
                case "Slider": return ControlTypes.Slider;
                case "Spinner": return ControlTypes.Spinner;
                case "Attachments": return ControlTypes.Attachments;
            }
            switch (column.TypeName.CsTypeSummary())
            {
                case Types.CsBool:
                    return ControlTypes.CheckBox;
                case Types.CsNumeric:
                    return column.HasChoices()
                        ? column.ChoicesControlType == "Radio"
                            ? ControlTypes.Radio
                            : ControlTypes.DropDown
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
                return column.ChoicesControlType == "Radio"
                    ? ControlTypes.Radio
                    : ControlTypes.DropDown;
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
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            string tagControlContainer = "div",
            string onFieldDblClick = null,
            bool validateRequired = false,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            Action controlAction = null,
            Action actionOptions = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    controlOnly: controlOnly,
                    onFieldDblClick: onFieldDblClick,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    actionLabel: () => hb
                        .Label(
                            controlId: controlId,
                            labelCss: labelCss,
                            labelText: labelText,
                            labelRaw: labelRaw,
                            labelTitle: labelTitle,
                            labelIcon: labelIcon,
                            required: validateRequired,
                            _using: !controlOnly),
                    actionControl: () => hb
                        .Control(
                            controlAction: controlAction,
                            fieldDescription: fieldDescription,
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
            string fieldDescription = null,
            bool controlOnly = false,
            string onFieldDblClick = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            Action actionLabel = null,
            Action actionControl = null,
            Action actionOptions = null,
            bool _using = true)
        {
            return _using
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id(fieldId)
                        .Class(!controlOnly
                            ? Css.Class("field-normal", fieldCss)
                            : fieldCss)
                        .Title(fieldDescription)
                        .OnDblClick(onFieldDblClick),
                    action: () =>
                    {
                        hb.Raw(text: extendedHtmlBeforeLabel);
                        actionLabel?.Invoke();
                        hb.Raw(text: extendedHtmlBetweenLabelAndControl);
                        actionControl?.Invoke();
                        actionOptions?.Invoke();
                        hb.Raw(text: extendedHtmlAfterControl);
                    })
                : hb;
        }

        private static HtmlBuilder Label(
            this HtmlBuilder hb,
            string controlId = null,
            string labelCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool required = false,
            bool _using = true)
        {
            return _using && labelText != string.Empty
                ? hb.P(css: "field-label", action: () =>
                {
                    if (!labelIcon.IsNullOrEmpty())
                    {
                        hb.Span(css: $"ui-icon {labelIcon}");
                    }
                    if (!labelRaw.IsNullOrEmpty())
                    {
                        hb.Raw(text: labelRaw);
                    }
                    else
                    {
                        hb.Label(
                            attributes: new HtmlAttributes()
                                .For(controlId)
                                .Class(Css.Class(labelCss, required
                                    ? " required"
                                    : string.Empty)),
                            action: () => hb
                                .Text(labelText));
                    }
                })
                : hb;
        }

        private static HtmlBuilder Control(
            this HtmlBuilder hb,
            Action controlAction,
            string fieldDescription,
            string controlContainerCss,
            string tagControlContainer)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("field-control")
                    .Title(fieldDescription),
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
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            string text = null,
            string dataValue = null,
            bool alwaysSend = false,
            bool openAnchorNewTab = false,
            string anchorFormat = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldDescription: fieldDescription,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .Span(
                            attributes: new HtmlAttributes()
                                .Id(controlId)
                                .Class(Css.Class("control-text", controlCss))
                                .DataValue(dataValue)
                                .DataReadOnly(true)
                                .DataAlwaysSend(alwaysSend),
                            action: () =>
                            {
                                if (anchorFormat.IsNullOrEmpty())
                                {
                                    hb.Text(text: text);
                                }
                                else
                                {
                                    hb.A(
                                        text: text,
                                        href: anchorFormat.Replace("{Value}", text),
                                        target: openAnchorNewTab
                                            ? "_blank"
                                            : string.Empty);
                                }
                            }),
                    controlContainerCss: controlContainerCss)
                : hb;
        }

        public static HtmlBuilder FieldTextBox(
            this HtmlBuilder hb,
            HtmlTypes.TextTypes textType = HtmlTypes.TextTypes.Normal,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string placeholder = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            string unit = null,
            string text = null,
            string format = null,
            bool timepiker = false,
            bool alwaysSend = false,
            string onChange = null,
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
            string dataLang = null,
            Dictionary<string, string> attributes = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            Action controlOption = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () =>
                    {
                        hb.TextBox(
                            textType: textType,
                            controlId: controlId,
                            controlCss: controlCss +
                                (!unit.IsNullOrEmpty()
                                    ? " with-unit"
                                    : string.Empty),
                            text: text,
                            placeholder: placeholder,
                            format: format,
                            timepicker: timepiker,
                            alwaysSend: alwaysSend,
                            onChange: onChange,
                            openAnchorNewTab: openAnchorNewTab,
                            anchorFormat: anchorFormat,
                            validateRequired: validateRequired,
                            validateNumber: validateNumber,
                            validateMinNumber: validateMinNumber,
                            validateMaxNumber: validateMaxNumber,
                            validateDate: validateDate,
                            validateEmail: validateEmail,
                            validateEqualTo: validateEqualTo,
                            validateMaxLength: validateMaxLength,
                            validateRegex: validateRegex,
                            validateRegexErrorMessage: validateRegexErrorMessage,
                            action: action,
                            method: method,
                            dataLang: dataLang,
                            attributes: attributes);
                        if (textType == HtmlTypes.TextTypes.Password)
                        {
                            hb.Div(
                                attributes: new HtmlAttributes()
                                    .Class("material-symbols-outlined show-password")
                                    .OnClick("$p.showPassword(this)"),
                                action: () => hb.Text("visibility"));
                        }
                        controlOption?.Invoke();
                        hb.Span(
                            css: "unit",
                            _using: !unit.IsNullOrEmpty(),
                            action: () => hb.Text(unit));
                    })
                : hb;
        }

        public static HtmlBuilder FieldMarkDown(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string placeholder = null,
            string labelIcon = null,
            int validateMaxLength = 0,
            string validateRegex = null,
            string validateRegexErrorMessage = null,
            bool controlOnly = false,
            string text = null,
            bool readOnly = false,
            bool allowImage = true,
            bool allowBulkUpdate = false,
            bool mobile = false,
            bool alwaysSend = false,
            bool validateRequired = false,
            Column.ViewerSwitchingTypes viewerSwitchingTypes = Column.ViewerSwitchingTypes.Auto,
            Dictionary<string, string> attributes = null,
            bool preview = false,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .MarkDown(
                            context: context,
                            ss: ss,
                            controlId: controlId,
                            controlCss: controlCss,
                            validateMaxLength: validateMaxLength,
                            validateRegex: validateRegex,
                            validateRegexErrorMessage: validateRegexErrorMessage,
                            text: text,
                            placeholder: placeholder,
                            readOnly: readOnly,
                            allowImage: allowImage,
                            allowBulkUpdate: allowBulkUpdate,
                            mobile: mobile,
                            alwaysSend: alwaysSend,
                            validateRequired: validateRequired,
                            viewerSwitchingTypes: viewerSwitchingTypes,
                            attributes: attributes,
                            preview: preview))
                : hb;
        }

        public static HtmlBuilder FieldMarkUp(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            string text = null,
            bool validateRequired = false,
            Dictionary<string, string> attributes = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .MarkUp(
                            controlId: controlId,
                            controlCss: controlCss,
                            text: text,
                            attributes: attributes))
                : hb;
        }

        public static HtmlBuilder FieldCodeEditor(
            this HtmlBuilder hb,
            Context context,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string placeholder = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            string unit = null,
            string text = null,
            bool alwaysSend = false,
            string onChange = null,
            bool validateRequired = false,
            bool validateNumber = false,
            bool validateDate = false,
            bool validateEmail = false,
            string validateEqualTo = null,
            int validateMaxLength = 0,
            string action = null,
            string method = null,
            string dataLang = null,
            Dictionary<string, string> attributes = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            var textType = context.ThemeVersionForCss() >= 2.0M && Parameters.General.EnableCodeEditor
                ? HtmlTypes.TextTypes.CodeEditor
                : HtmlTypes.TextTypes.MultiLine;
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () =>
                    {
                        hb.TextBox(
                            textType: textType,
                            controlId: controlId,
                            controlCss: controlCss +
                                (!unit.IsNullOrEmpty()
                                    ? " with-unit"
                                    : string.Empty),
                            text: text,
                            placeholder: placeholder,
                            alwaysSend: alwaysSend,
                            onChange: onChange,
                            validateRequired: validateRequired,
                            validateNumber: validateNumber,
                            validateDate: validateDate,
                            validateEmail: validateEmail,
                            validateEqualTo: validateEqualTo,
                            validateMaxLength: validateMaxLength,
                            action: action,
                            method: method,
                            dataLang: dataLang,
                            attributes: attributes);
                    })
                : hb;
        }

        public static HtmlBuilder FieldDropDown(
            this HtmlBuilder hb,
            Context context,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            Dictionary<string, string> optionCollection = null,
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
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .DropDown(
                            context: context,
                            controlId: controlId,
                            controlCss: controlCss,
                            optionCollection: optionCollection?.ToDictionary(
                                o => o.Key, o => new ControlData(o.Value)),
                            selectedValue: selectedValue,
                            multiple: multiple,
                            addSelectedValue: addSelectedValue,
                            insertBlank: insertBlank,
                            disabled: disabled,
                            alwaysSend: alwaysSend,
                            onChange: onChange,
                            validateRequired: validateRequired,
                            action: action,
                            method: method,
                            column: column))
                : hb;
        }

        public static HtmlBuilder FieldDropDown(
            this HtmlBuilder hb,
            Context context,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            bool multiple = false,
            bool addSelectedValue = true,
            bool insertBlank = false,
            bool disabled = false,
            bool alwaysSend = false,
            string onChange = null,
            string onFieldDblClick = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            Column column = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            Action controlOption = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    onFieldDblClick: onFieldDblClick,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () =>
                    {
                        hb
                            .DropDown(
                                context: context,
                                controlId: controlId,
                                controlCss: controlCss,
                                optionCollection: optionCollection,
                                selectedValue: selectedValue,
                                multiple: multiple,
                                addSelectedValue: addSelectedValue,
                                insertBlank: insertBlank,
                                disabled: disabled,
                                alwaysSend: alwaysSend,
                                onChange: onChange,
                                validateRequired: validateRequired,
                                action: action,
                                method: method,
                                column: column);
                        controlOption?.Invoke();
                    })
                : hb;
        }

        public static HtmlBuilder FieldCheckBox(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            bool _checked = false,
            bool disabled = false,
            bool alwaysSend = false,
            string dataId = null,
            string onChange = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            bool labelPositionIsRight = true,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            if (!_using) return hb;
            if (!labelPositionIsRight)
            {
                return hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .CheckBox(
                            controlId: controlId,
                            controlCss: controlCss,
                            _checked: _checked,
                            disabled: disabled,
                            alwaysSend: alwaysSend,
                            dataId: dataId,
                            onChange: onChange,
                            validateRequired: validateRequired,
                            action: action,
                            method: method));
            }
            else
            {
                return hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: string.Empty,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .CheckBox(
                            controlId: controlId,
                            controlCss: controlCss,
                            labelText: labelText,
                            labelIcon: labelIcon,
                            controlOnly: controlOnly,
                            _checked: _checked,
                            disabled: disabled,
                            alwaysSend: alwaysSend,
                            onChange: onChange,
                            validateRequired: validateRequired,
                            action: action,
                            method: method));
            }
        }

        public static HtmlBuilder FieldRadio(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            bool alwaysSend = false,
            bool validateRequired = false,
            Dictionary<string, ControlData> optionCollection = null,
            string selectedValue = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .Hidden(
                            controlId: controlId,
                            css: "radio-value",
                            value: selectedValue,
                            validateRequired: validateRequired)
                        .RadioButtons(
                            name: controlId,
                            controlCss: controlCss,
                            optionCollection: optionCollection,
                            selectedValue: selectedValue))
                : hb;
        }

        public static HtmlBuilder FieldSpinner(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            decimal? value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            int width = 50,
            string unit = null,
            bool alwaysSend = false,
            bool allowBlank = false,
            string onChange = null,
            bool validateRequired = false,
            string action = null,
            string method = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
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
                            alwaysSend: alwaysSend,
                            allowBalnk: allowBlank,
                            onChange: onChange,
                            validateRequired: validateRequired,
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
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            decimal value = 0,
            decimal min = -1,
            decimal max = -1,
            decimal step = -1,
            string unit = null,
            bool alwaysSend = false,
            bool validateRequired = false,
            string action = null,
            string method = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    controlId: controlId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .Slider(
                            controlId: controlId,
                            controlCss: controlCss,
                            value: value,
                            min: min,
                            max: max,
                            step: step,
                            unit: unit,
                            alwaysSend: alwaysSend,
                            action: action,
                            method: method))
                : hb;
        }

        public static HtmlBuilder FieldSelectable(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string controlWrapperCss = null,
            string controlCss = null,
            string labelText = null,
            string labelRaw = null,
            string labelTitle = null,
            string labelIcon = null,
            bool controlOnly = false,
            bool validateRequired = false,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            bool commandOptionPositionIsTop = false,
            bool alwaysDataValue = false,
            string action = null,
            string method = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            Action commandOptionAction = null,
            bool setSearchOptionButton = false,
            string searchOptionId = null,
            string searchOptionFunction = null,
            bool _using = true)
        {
            if (!_using) return hb;
            if (!commandOptionPositionIsTop)
            {
                return hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .SelectableWrapper(
                            controlId: controlId,
                            controlWrapperCss: controlWrapperCss,
                            controlCss: controlCss,
                            listItemCollection: listItemCollection,
                            selectedValueCollection: selectedValueCollection,
                            alwaysDataValue: alwaysDataValue,
                            action: action,
                            method: method),
                    actionOptions: commandOptionAction);
            }
            else
            {
                return hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    labelTitle: labelTitle,
                    labelIcon: labelIcon,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    controlAction: () =>
                    {
                        commandOptionAction();
                        hb.SelectableWrapper(
                            controlId: controlId,
                            controlWrapperCss: controlWrapperCss,
                            controlCss: controlCss,
                            listItemCollection: listItemCollection,
                            selectedValueCollection: selectedValueCollection,
                            alwaysDataValue: alwaysDataValue,
                            action: action,
                            method: method,
                            setSearchOptionButton: setSearchOptionButton,
                            searchOptionId: searchOptionId,
                            searchOptionFunction: searchOptionFunction);
                    });
            }
        }

        public static HtmlBuilder FieldBasket(
            this HtmlBuilder hb,
            string fieldId = null,
            string controlId = null,
            string fieldCss = null,
            string fieldDescription = null,
            string controlCss = null,
            Action labelAction = null,
            Dictionary<string, ControlData> listItemCollection = null,
            IEnumerable<string> selectedValueCollection = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            Action actionOptions = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
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

        public static HtmlBuilder FieldAttachments(
            this HtmlBuilder hb,
            Context context,
            string fieldId = null,
            string controlId = null,
            string columnName = null,
            string fieldCss = null,
            string fieldDescription = null,
            string labelCss = null,
            string controlContainerCss = null,
            string labelText = null,
            string labelRaw = null,
            bool controlOnly = false,
            string value = null,
            bool readOnly = false,
            bool allowDelete = true,
            bool preview = false,
            bool validateRequired = false,
            int validateMaxLength = 0,
            string inputGuide = null,
            string extendedHtmlBeforeLabel = null,
            string extendedHtmlBetweenLabelAndControl = null,
            string extendedHtmlAfterControl = null,
            bool _using = true)
        {
            return _using
                ? hb.Field(
                    fieldId: fieldId,
                    fieldCss: fieldCss,
                    fieldDescription: fieldDescription,
                    labelCss: labelCss,
                    controlContainerCss: controlContainerCss,
                    labelText: labelText,
                    labelRaw: labelRaw,
                    controlOnly: controlOnly,
                    validateRequired: validateRequired,
                    extendedHtmlBeforeLabel: extendedHtmlBeforeLabel,
                    extendedHtmlBetweenLabelAndControl: extendedHtmlBetweenLabelAndControl,
                    extendedHtmlAfterControl: extendedHtmlAfterControl,
                    controlAction: () => hb
                        .Attachments(
                            context: context,
                            controlId: controlId,
                            columnName: columnName,
                            value: value,
                            readOnly: readOnly,
                            allowDelete: allowDelete,
                            preview: preview,
                            validateRequired: validateRequired,
                            validateMaxLength: validateMaxLength,
                            inputGuide: inputGuide))
                : hb;
        }
    }
}