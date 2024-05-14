$(function () {
    $(document).on('change', '#UseGridDesign', function () {
        $('#GridDesignField').toggle($(this).prop('checked'));
    });
    $(document).on('change', '#NoDuplication', function () {
        var visibility = $(this).prop('checked');
        $('#MessageWhenDuplicatedField').toggle(visibility);
    });
    $(document).on('change', '#Anchor', function () {
        var visibility = $(this).prop('checked');
        $('#OpenAnchorNewTabField').toggle(visibility);
        $('#AnchorFormatField').toggle(visibility);
    });
    $(document).on('change', '#AutoPostBack', function () {
        var visibility = $(this).prop('checked');
        $('#ColumnsReturnedWhenAutomaticPostbackField').toggle(visibility);
    });
    $(document).on('change', '#ControlType', function () {
        var visibility = $(this).val() === 'Spinner';
        $('#StepField').toggle(visibility);
    });
    $(document).on('change', '#BinaryStorageProvider', function () {
        var visibilityL = $(this).val() !== 'LocalFolder';
        $('#LimitSizeField').toggle(visibilityL);
        $('#LimitTotalSizeField').toggle(visibilityL);
        var visibilityD = $(this).val() !== 'DataBase';
        $('#LocalFolderLimitSizeField').toggle(visibilityD);
        var visibilityA = $(this).val() === 'LocalFolder';
        $('#LocalFolderLimitTotalSizeField').toggle(visibilityA);
    });
    $(document).on('change', '#EditorFormat', function () {
        var visibility = $(this).val() === 'Ymdhm';
        $('#DateTimeStepField').toggle(visibility);
    });
    $(document).on('change', '#FormatSelector', function () {
        var $control = $(this);
        switch ($control.val()) {
            case '\t':
                $('#FormatField').toggle(true);
                $('#Format').val('');
                break;
            default:
                $('#FormatField').toggle(false);
                $('#Format').val($control.val());
                break;
        }
        $p.setData($('#Format'));
    });
    $(document).on('change', '#NotificationType', function () {
        $('#NotificationTokenField').toggle(
            $('#NotificationTokenEnableList').val()
                .split(',')
                .indexOf($('#NotificationType').val()) !== -1);
        switch ($(this).val()) {
            case '9':
                $('#NotificationMethodTypeField').toggle(true);
                $('#NotificationEncodingField').toggle(true);
                $('#NotificationMediaTypeField').toggle(true);
                $('#NotificationRequestHeadersField').toggle(true);
                break;
            default:
                $('#NotificationMethodTypeField').toggle(false);
                $('#NotificationEncodingField').toggle(false);
                $('#NotificationMediaTypeField').toggle(false);
                $('#NotificationRequestHeadersField').toggle(false);
                break;
        }
    });
    $(document).on('change', '#ProcessDataChangeType', function () {
        $('#ProcessDataChangeValueField').toggle(false);
        $('#ProcessDataChangeBaseDateTimeField').toggle(false);
        $('#ProcessDataChangeValueDateTimeField').toggle(false);
        $('#ProcessDataChangeValueColumnNamePeriodField').toggle(false);
        $('#ProcessDataChangeValueColumnNameField').toggle(false);
        switch ($(this).val()) {
            case 'CopyValue':
            case 'CopyDisplayValue':
                $('#ProcessDataChangeValueColumnNameField').toggle(true);
                break;
            case 'InputValue':
                $('#ProcessDataChangeValueField').toggle(true);
                break;
            case 'InputDate':
                $('#ProcessDataChangeBaseDateTimeField').toggle(true);
                $('#ProcessDataChangeValueDateTimeField').toggle(true);
                $('#ProcessDataChangeValueColumnNamePeriodField').toggle(true);
                $('#ProcessDataChangeBaseDateTime').val('CurrentDate');
                break;
            case 'InputDateTime':
                $('#ProcessDataChangeBaseDateTimeField').toggle(true);
                $('#ProcessDataChangeValueDateTimeField').toggle(true);
                $('#ProcessDataChangeValueColumnNamePeriodField').toggle(true);
                $('#ProcessDataChangeBaseDateTime').val('CurrentTime');
                break;
            default:
                break;
        }
    });
    $(document).on('change', '#ProcessNotificationType', function () {
        $('#ProcessNotificationTokenField').toggle(
            $('#ProcessNotificationTokenEnableList').val()
                .split(',')
                .indexOf($('#ProcessNotificationType').val()) !== -1);
    });
    $(document).on('change', '#ReminderType', function () {
        var reminderFromVisible = $('#ReminderFromEnableList').val()
            .split(',')
            .indexOf($('#ReminderType').val()) !== -1;
        $('#ReminderFromField').toggle(reminderFromVisible)
        $('#ReminderFrom').attr('data-validate-required', reminderFromVisible
            ? '1'
            : '0');
        $('#ReminderTokenField').toggle(
            $('#ReminderTokenEnableList').val()
                .split(',')
                .indexOf($('#ReminderType').val()) !== -1);
    });
    $(document).on('change', '#NotificationUseCustomFormat', function () {
        $('#NotificationFormatField').toggle($('#NotificationUseCustomFormat').prop('checked'));
    });
    $(document).on('click', '#SummarySettings .grid-row button', function () {
        var $control = $($(this).attr('data-selector'))
        $p.getData($control)[$control.attr('id') + "Id"] = $(this).attr('data-id');
        $p.send($control);
    });
    $(document).on('change', '#SummaryDestinationCondition', function () {
        $('#SummarySetZeroWhenOutOfConditionField').toggle($(this).val() !== '');
    });
    $(document).on('change', '#FormulaCondition', function () {
        $('#FormulaOutOfConditionField').toggle($(this).val() !== '');
    });
    $(document).on('click', '.add-view-sorter', function () {
        var $control = $(this);
        var prefix = $control.data('prefix');
        var $dataViewSorter = $('#' + prefix + 'ViewSorterSelector option:selected');
        var orderType = $('#' + prefix + 'ViewSorterOrderTypes option:selected').val();
        var orderText = '';
        switch (orderType) {
            case 'asc':
                orderText = '(' + $p.display('OrderAsc') + ')';
                break;
            case 'desc':
                orderText = '(' + $p.display('OrderDesc') + ')';
                break;
        }
        $p.addBasket(
            $('#' + prefix + 'ViewSorters'),
            $dataViewSorter.text() + orderText,
            $dataViewSorter.val() + '&' + orderType);
    });
    $(document).on('change', '#StyleAll', function () {
        if ($('#StyleAll').prop('checked')) {
            $('.output-destination-style')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-style')
                .removeClass('hidden')
                .find('input');
        }
    });
    $(document).on('change', '#ScriptAll', function () {
        if ($('#ScriptAll').prop('checked')) {
            $('.output-destination-script')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-script')
                .removeClass('hidden')
                .find('input');
        }
    });
    $(document).on('change', '#HtmlAll', function () {
        if ($('#HtmlAll').prop('checked')) {
            $('.output-destination-html')
                .addClass('hidden')
                .find('input')
                .prop('checked', false);
        } else {
            $('.output-destination-html')
                .removeClass('hidden')
                .find('input');
        }
    });
    $(document).on('change', '#DateFilterSetMode', function () {
        if ($('#DateFilterSetMode').val() === '1') {
            $('#FilterColumnSettingField').removeClass('hidden');
        } else {
            $('#FilterColumnSettingField').addClass('hidden');
        }
    });
    $(document).on('change', '#UseRelatingColumnsOnFilter', function () {
        if ($('#UseRelatingColumnsOnFilter').prop('checked')) {
            $p.set($('#UseGridHeaderFilters'), false);
            $('#UseGridHeaderFilters').prop('disabled', true);
        } else {
            $('#UseGridHeaderFilters').prop('disabled', false);
        }
    });
    $(document).on('change', '#ViewFilters_KeepFilterState', function () {
        $('#ViewFiltersFilterConditionSettingsEditor').toggle();
    });
    $(document).on('change', '#KeepSorterState', function () {
        $('#ViewFiltersSorterConditionSettingsEditor').toggle();
    });
    $(document).on('change', '#DashboardPartType', function () {
        let $control = $(this);
        let selected = $control.val();        
        $('#DashboardPartQuickAccessSitesField').toggle(selected === '0');
        $('#DashboardPartQuickAccessLayoutField').toggle(selected === '0');
        $('#DashboardPartTimeLineSitesField').toggle(selected === '1');
        $('#DashboardPartTimeLineTitleField').toggle(selected === '1');
        $('#DashboardPartTimeLineBodyField').toggle(selected === '1');
        $('#DashboardPartTimeLineDisplayTypeField').toggle(selected === '1');
        $('#DashboardPartTimeLineItemCountField').toggle(selected === '1');
        $('#DashboardPartContentField').toggle(selected === '2');
        $('#DashboardPartHtmlContentField').toggle(selected === '3');
        $('#DashboardPartCalendarSitesField').toggle(selected === '4');
        $('#DashboardPartCalendarTypeField').toggle(selected === '4');
        $('#DashboardPartCalendarGroupByField').toggle(selected === '4'
            && $("#DashboardPartCalendarType").val() === '1');
        $('#DashboardPartCalendarTimePeriodField').toggle(selected === '4'
            && $("#DashboardPartCalendarType").val() === '1');
        $('#DashboardPartCalendarFromToField').toggle(selected === '4');
        $('#DashboardPartCalendarShowStatusField').toggle(selected === '4');
        $('#DashboardPartKambanSitesField').toggle(selected === '5');
        $('#DashboardPartKambanGroupByXField').toggle(selected === '5');
        $('#DashboardPartKambanGroupByYField').toggle(selected === '5');
        $('#DashboardPartKambanAggregateTypeField').toggle(selected === '5');
        $('#DashboardPartKambanValueField').toggle(selected === '5');
        $('#DashboardPartKambanColumnsField').toggle(selected === '5');
        $('#DashboardPartKambanAggregationViewField').toggle(selected === '5');
        $('#DashboardPartKambanShowStatusField').toggle(selected === '5');
        $('#DashboardPartIndexSitesField').toggle(selected === '6');
        $('#DashboardPartViewIndexTabControl').toggle(selected === '6');
        $('#DashboardPartViewFiltersTabControl').toggle(selected === '1'
            || selected === '4'
            || selected === '5'
            || selected === '6');
        $('#DashboardPartViewSortersTabControl').toggle(selected === '1' || selected === '6');
    });
    $(document).on('change','#DashboardPartCalendarType',function () {
        
        $('#DashboardPartCalendarGroupByField').toggle($("#DashboardPartCalendarType").val() === '1');
        $('#DashboardPartCalendarTimePeriodField').toggle($("#DashboardPartCalendarType").val() === '1');

    });
    $(document).on('change', '#DashboardPartKambanAggregateType', function () {

        $('#DashboardPartKambanValueField').toggle($('#DashboardPartKambanAggregateType').val() !== 'Count');

    });
    $(document).on('change', '#GuideAllowExpand', function () {
        var visibility = $(this).prop('checked');
        $('#GuideExpandField').toggle(visibility);
    });
});