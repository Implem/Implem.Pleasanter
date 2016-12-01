$p.uploadSiteImage = function ($control) {
    var data = new FormData();
    data.append('SiteImage', $('[id=\'SiteSettings,SiteImage\']').prop('files')[0]);
    $p.upload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        $control.attr('data-method'),
        data);
}

$p.openSiteSettingsDialog = function ($control, selector, appendTo) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $(selector).dialog({
            modal: true,
            width: '90%',
            height: 'auto',
            appendTo: appendTo !== undefined
                ? appendTo
                : '.main-form'
        });
    }
}

$p.openGridColumnPropertiesDialog = function ($control) {
    $p.openSiteSettingsDialog($control, '#GridColumnPropertiesDialog');
}

$p.setGridColumnProperties = function ($control) {
    $p.setData($('[id="GridColumnProperty,UseGridDesign"]'));
    $p.setData($('[id="GridColumnProperty,GridDesign"]'));
    $p.send($control);
    $p.closeDialog($control);
}

$p.openEditorColumnPropertiesDialog = function ($control) {
    $p.openSiteSettingsDialog($control, '#EditorColumnPropertiesDialog');
}

$p.openViewDialog = function ($control) {
    $p.data['ViewForm'] = {};
    $p.openSiteSettingsDialog($control, '#ViewDialog', 'body');
}

$p.openNotificationDialog = function ($control) {
    $p.openSiteSettingsDialog($control, '#NotificationDialog', 'body');
}

$p.setAggregationDetails = function ($control) {
    var data = $p.getData($control);
    data.AggregationType = $('#AggregationType').val();
    data.AggregationTarget = $('#AggregationTarget').val();
    $('.ui-dialog-content').dialog('close');
    $p.send($control);
}

$p.addSummary = function ($control) {
    $p.setData($('#SummarySiteId'));
    $p.setData($('#SummaryDestinationColumn'));
    $p.setData($('#SummaryLinkColumn'));
    $p.setData($('#SummaryType'));
    $p.setData($('#SummarySourceColumn'));
    $p.send($control);
}