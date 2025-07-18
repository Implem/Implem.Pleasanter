$p.openColumnAccessControlDialog = function ($control) {
    $p.data.ColumnAccessControlForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#ColumnAccessControlDialog').dialog({
            modal: true,
            width: '900px',
            appendTo: '#Editor',
            resizable: false
        });
    }
}

$p.addColumnAccessControl = function () {
    $('#SourceColumnAccessControl li.ui-selected').appendTo('#CurrentColumnAccessControl');
    $p.setData($('#CurrentColumnAccessControl'));
}

$p.deleteColumnAccessControl = function () {
    $('#CurrentColumnAccessControl li.ui-selected').appendTo('#SourceColumnAccessControl');
}

$p.changeColumnAccessControl = function ($control, type) {
    $p.setData($('#' + type + 'ColumnAccessControl'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.ColumnAccessControl = mainFormData[type + 'ColumnAccessControl'];
    data.ColumnAccessControlAll = mainFormData[type + 'ColumnAccessControlAll'];
    data.ColumnAccessControlType = type;
    $p.send($control);
}