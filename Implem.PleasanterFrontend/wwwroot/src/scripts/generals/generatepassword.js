$(function () {
    $(document).on('click', '.generate-password', function () {
        const $control = $(this);
        $p.generatePassword(
            $control,
            $control.attr('data-password-object'),
            $control.attr('data-password-validate-object')
        );
    });
    $p.setGeneratePasswordButton();
});

$p.setGeneratePasswordButton = function () {
    $p.generatePasswordButton('#Users_Password', '#Users_PasswordValidate');
    $p.generatePasswordButton('#Users_ChangedPassword', '#Users_ChangedPasswordValidator');
    $p.generatePasswordButton('#Users_AfterResetPassword', '#Users_AfterResetPasswordValidator');
};

$p.generatePasswordButton = function (passwordObject, passwordValidateObject) {
    const $password = $(passwordObject);
    if (!$password.length || !$(passwordValidateObject).length) {
        return;
    }
    if (!parseInt($password.data('passwordgenerator'))) {
        return;
    }
    const $container = $password.closest('div');
    if ($container.children('.generate-password').length) {
        return;
    }
    $('<span>', {
        class: 'material-symbols-outlined generate-password',
        title: $p.display('PasswordAutoGenerate'),
        text: 'key',
        'data-action': 'GeneratePassword',
        'data-password-object': passwordObject,
        'data-password-validate-object': passwordValidateObject
    }).appendTo($container);
};

$p.generatePassword = function ($control, passwordObject, passwordValidateObject) {
    var action = $control.attr('data-action');
    var url = action
        ? $control.closest('form').attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    var data = {
        passwordObject: passwordObject,
        passwordValidateObject: passwordValidateObject
    };
    $p.ajax(url, 'post', data, $control);
};
