$p.setPermissionEvents = function () {
    $p.setPaging('SourcePermissions');
};

$p.setPermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    $p.setData($('#SourcePermissions'));
    $p.setData($('#SearchPermissionElements'));
    $p.send($control);
};

$p.openPermissionsDialog = function ($control) {
    $p.data.PermissionsForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionsDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor',
            resizable: false,
            close: $p.destroyPermissionsDialog
        });
    }
};

$p.changePermissions = function ($control) {
    $p.setData($('#CurrentPermissions'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissions = mainFormData.CurrentPermissions;
    data.CurrentPermissionsAll = mainFormData.CurrentPermissionsAll;
    $p.send($control);
};

$p.setPermissionForCreating = function ($control) {
    $p.setData($('#CurrentPermissionForCreating'));
    $p.setData($('#SourcePermissionForCreating'));
    $p.send($control);
};

$p.openPermissionForCreatingDialog = function ($control) {
    $p.data.PermissionForCreatingForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionForCreatingDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor',
            resizable: false,
            close: $p.destroyPermissionsDialog
        });
    }
};

$p.setPermissionForUpdating = function ($control) {
    $p.setData($('#CurrentPermissionForUpdating'));
    $p.setData($('#SourcePermissionForUpdating'));
    $p.send($control);
};

$p.openPermissionForUpdatingDialog = function ($control) {
    $p.data.PermissionForUpdatingForm = {};
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#PermissionForUpdatingDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '#Editor',
            resizable: false,
            close: $p.destroyPermissionsDialog
        });
    }
};

$p.changePermissionForCreating = function ($control) {
    $p.setData($('#CurrentPermissionForCreating'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissionForCreating = mainFormData.CurrentPermissionForCreating;
    data.CurrentPermissionForCreatingAll = mainFormData.CurrentPermissionForCreatingAll;
    $p.send($control);
};

$p.changePermissionForUpdating = function ($control) {
    $p.setData($('#CurrentPermissionForUpdating'));
    var data = $p.getData($control);
    var mainFormData = $p.getData($('.main-form'));
    data.CurrentPermissionForUpdating = mainFormData.CurrentPermissionForUpdating;
    data.CurrentPermissionForUpdatingAll = mainFormData.CurrentPermissionForUpdatingAll;
    $p.send($control);
};

$p.destroyPermissionsDialog = function (e) {
    const dialog = $(e.target);
    dialog.dialog('destroy');
    dialog.empty();
};
