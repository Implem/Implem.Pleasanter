$p.uploadSiteImage = function ($control) {
    var data = new FormData();
    data.append('SiteImage', $('#SiteImage').prop('files')[0]);
    $p.upload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        $control.attr('data-method'),
        data);
}

$p.openSiteSettingsDialog = function ($control, selector, width) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $(selector).dialog({
            modal: true,
            width: width !== undefined ? width : '90%',
            height: 'auto',
            appendTo: '#Editor'
        });
    }
}

$p.openGridColumnDialog = function ($control) {
    $p.data.GridColumnForm = {};
    $p.openSiteSettingsDialog($control, '#GridColumnDialog');
}

$p.setGridColumn = function ($control) {
    $p.setData($('#UseGridDesign'));
    $p.setData($('#GridDesign'));
    $p.send($control);
}

$p.openFilterColumnDialog = function ($control) {
    $p.data.FilterColumnForm = {};
    $p.openSiteSettingsDialog($control, '#FilterColumnDialog');
}

$p.openEditorColumnDialog = function ($control) {
    $p.data.EditorColumnForm = {};
    $p.openSiteSettingsDialog($control, '#EditorColumnDialog');
}

$p.resetEditorColumn = function ($control) {
    $p.syncSend($control);
    var data = $p.getData($control);
    $('#EditorColumnForm [class^="control-"]').each(function (index, control) {
        $p.setData($(control), data);
    });
}

$p.openSummaryDialog = function ($control) {
    $p.data.SummaryForm = {};
    $p.openSiteSettingsDialog($control, '#SummaryDialog');
}

$p.setSummary = function ($control) {
    $p.setData($('#EditSummary'), $p.getData($control));
    $p.send($control);
}

$p.openFormulaDialog = function ($control) {
    $p.data.FormulaForm = {};
    $p.openSiteSettingsDialog($control, '#FormulaDialog');
}

$p.openViewDialog = function ($control) {
    $p.data.ViewForm = {};
    $p.openSiteSettingsDialog($control, '#ViewDialog');
}

$p.openNotificationDialog = function ($control) {
    $p.data.NotificationForm = {};
    $p.openSiteSettingsDialog($control, '#NotificationDialog');
}

$p.setNotification = function ($control) {
    $p.setData($('#EditNotification'), $p.getData($control));
    $p.send($control);
}

$p.openReminderDialog = function ($control) {
    $p.data.ReminderForm = {};
    $p.openSiteSettingsDialog($control, '#ReminderDialog');
}

$p.setReminder = function ($control) {
    $p.setData($('#EditReminder'), $p.getData($control));
    $p.send($control);
}

$p.openExportDialog = function ($control) {
    $p.data.ExportForm = {};
    $p.openSiteSettingsDialog($control, '#ExportDialog');
}

$p.setExport = function ($control) {
    $p.setData($('#EditExport'), $p.getData($control));
    $p.send($control);
}

$p.openExportColumnsDialog = function ($control) {
    $p.data.ExportColumnsForm = {};
    $p.openSiteSettingsDialog($control, '#ExportColumnsDialog', '420px');
}

$p.setExportColumn = function ($control) {
    $p.setData($('#EditExport'), $p.getData($control));
    $p.send($control);
}

$p.openScriptDialog = function ($control) {
    $p.data.ScriptForm = {};
    $p.openSiteSettingsDialog($control, '#ScriptDialog');
}

$p.setScript = function ($control) {
    $p.setData($('#EditScript'), $p.getData($control));
    $p.send($control);
}

$p.openStyleDialog = function ($control) {
    $p.data.StyleForm = {};
    $p.openSiteSettingsDialog($control, '#StyleDialog');
}

$p.setStyle = function ($control) {
    $p.setData($('#EditStyle'), $p.getData($control));
    $p.send($control);
}

$p.setAggregationDetails = function ($control) {
    var data = $p.getData($control);
    data.AggregationType = $('#AggregationType').val();
    data.AggregationTarget = $('#AggregationTarget').val();
    $p.clearMessage();
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