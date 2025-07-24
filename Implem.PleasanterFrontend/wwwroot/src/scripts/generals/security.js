$p.openChangePasswordDialog = function ($control, formId) {
    if (formId) {
        error = $p.send($control, formId);
        if (error !== 0) {
            return;
        }
    } else {
        var data = $p.getData($('#ChangePasswordForm'));
        data.Users_LoginId = $('#Users_LoginId').val();
        data.Users_OldPassword = $('#Users_Password').val();
        data.ReturnUrl = $('#ReturnUrl').val();
    }
    $('#ChangePasswordDialog').dialog({
        modal: true,
        width: '420px',
        resizable: false
    });
}

$p.changePasswordAtLogin = function ($control) {
    var data = $p.getData($('#ChangePasswordForm'));
    data.Users_LoginId = $('#Users_LoginId').val();
    data.Users_Password = $('#Users_Password').val();
    $p.send($control);
}