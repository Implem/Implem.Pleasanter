$p.openExportMultilingualLabelsDialog = function ($control) {
    $p.data.ExportMultilingualLabelsForm = {};
    $p.openSiteSettingsDialog($control, '#ExportMultilingualLabelsDialog', '520px');
};

$p.openImportMultilingualLabelsDialog = function ($control) {
    $p.data.ImportMultilingualLabelsForm = {};
    $p.openSiteSettingsDialog($control, '#ImportMultilingualLabelsDialog', '520px');
};

$p.exportMultilingualLabels = function () {
    var encoding = $('#ExportMultilingualLabelsEncoding').val();
    var form = document.createElement('form');
    form.style.display = 'none';
    var action = $('.main-form').attr('action').replace('_action_', 'ExportMultilingualLabels');
    form.setAttribute('action', action);
    form.setAttribute('method', 'get');
    var input = document.createElement('input');
    input.setAttribute('type', 'hidden');
    input.setAttribute('name', 'encoding');
    input.setAttribute('value', encoding);
    form.appendChild(input);
    document.body.appendChild(form);
    $p.formChanged = false;
    form.submit();
    document.body.removeChild(form);
    $p.closeDialog($('#ExportMultilingualLabelsDialog'));
};

$p.importMultilingualLabels = function ($control) {
    var file = $('#ImportMultilingualLabelsFile').prop('files')[0];
    if (!file) {
        $p.setMessage(
            '#Message',
            JSON.stringify({
                Css: 'alert-error',
                Text: $p.display('SelectFile')
            })
        );
        return;
    }
    var data = new FormData();
    data.append('file', file);
    data.append('ImportMultilingualLabelsEncoding', $('#ImportMultilingualLabelsEncoding').val());

    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control
    );
};
