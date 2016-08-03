$p.openImportSettingsDialog = function ($control) {
    $p.send($control, undefined, false);
    $('#ImportSettingsDialog').dialog({
        modal: true,
        width: '420px'
    });
}

$p.import = function ($control) {
    var data = new FormData();
    data.append('Import', $('#Import').prop('files')[0]);
    data.append('Encoding', $('#Encoding').val());
    $p.upload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        $control.attr('data-method'),
        data);
}