$(function () {
    var passwordObject;
    var passwordValidateObject;
    if ($('input#Users_PasswordValidate').length) {
        $p.generatePasswordButton('#Users_Password', '#Users_PasswordValidate');
    }
    if ($('input#Users_ChangedPasswordValidator').length) {
        $p.generatePasswordButton('#Users_ChangedPassword', '#Users_ChangedPasswordValidator');
    }
    if ($('input#Users_AfterResetPasswordValidator').length) {
        $p.generatePasswordButton('#Users_AfterResetPassword', '#Users_AfterResetPasswordValidator');
    }
});

$p.generatePasswordButton = function (passwordObject, passwordValidateObject) {
    if (!parseInt($(passwordObject).data('passwordgenerator'))) {
        return;
    }
    $('<span>', {
        id: 'passwordGenerateicon',
        class: 'material-symbols-outlined generate-password',
        title: $p.display('PasswordAutoGenerate'),
        text: 'key',
        'data-action': 'GeneratePassword'
    }).appendTo($(passwordObject).closest('div'));

    $(document).on('click', '#passwordGenerateicon', function () {
        $p.generatePassword($(this), passwordObject, passwordValidateObject);
    });
}

$p.generatePassword = function ($control, passwordObject, passwordValidateObject) {
    var action = $control.attr('data-action');
    var url = action
        ? $('.main-form').attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    var data = {
        passwordObject: passwordObject,
        passwordValidateObject: passwordValidateObject
    };
    $p.ajax(url, 'post', data, $control);
}