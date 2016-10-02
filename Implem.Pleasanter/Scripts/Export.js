$p.openExportSettingsDialog = function ($control) {
    delete $p.data.ExportSettingsForm;
    $p.ajax(
        $control.attr('data-action'),
        $control.attr('data-method'),
        $p.getData('ExportSettingsForm'),
        $control,
        false);
    $('#ExportSettingsDialog').dialog({
        modal: true,
        width: '980px'
    });
}

$p.export = function ($control) {
    $p.send($control);
    location.href = $('.main-form').attr('action').replace('_action_', 'export');
}