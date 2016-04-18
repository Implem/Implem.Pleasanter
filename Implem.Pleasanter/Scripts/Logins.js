func.openDialog_ChangePassword = function () {
    var data = getFormData($('#ChangePasswordForm'));
    data['Users_LoginId'] = $('#Users_LoginId').val();
    data['Users_OldPassword'] = $('#Users_Password').val();
    data['ReturnUrl'] = $('#ReturnUrl').val();
    $('#Dialog_ChangePassword').dialog({
        modal: true,
        width: '420px'
    });
}