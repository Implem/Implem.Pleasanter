$p.openExportMultilingualLabelsDialog = function () {
    if ($p.formChanged) {
        $p.setErrorMessage('SiteSettingsUnsavedChangesBlocker');
        return;
    }
    $p.data.ExportMultilingualLabelsForm = {};
    $('#ExportMultilingualLabelsDialog').dialog({
        autoOpen: true,
        modal: true,
        width: '520px',
        height: 'auto',
        appendTo: '#Editor',
        resizable: false
    });
};

$p.openImportMultilingualLabelsDialog = function () {
    if ($p.formChanged) {
        $p.setErrorMessage('SiteSettingsUnsavedChangesBlocker');
        return;
    }
    $p.data.ImportMultilingualLabelsForm = {};
    $('#ImportMultilingualLabelsDialog').dialog({
        autoOpen: true,
        modal: true,
        width: '520px',
        height: 'auto',
        appendTo: '#Editor',
        resizable: false
    });
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
    form.submit();
    document.body.removeChild(form);
    $p.closeDialog($('#ExportMultilingualLabelsDialog'));
};

$p.importMultilingualLabels = function ($control) {
    var file = $('#ImportMultilingualLabelsFile').prop('files')[0];
    if (!file) {
        $p.setErrorMessage('SelectFile');
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
