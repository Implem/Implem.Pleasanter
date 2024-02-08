$(function () {
    var passwordObject;
    var passwordValidateObject;
    if ($('input#Users_PasswordValidate').length) {
        $p.generatePasswordButton('#Users_Password', '#Users_PasswordValidate');
    }
    if ($('input#Users_ChangedPasswordValidator').length) {
        $p.generatePasswordButton('#Users_ChangedPassword', '#Users_ChangedPasswordValidator');
    }
});

$p.generatePasswordButton = function (passwordObject, passwordValidateObject) {
    $('<button>', {
        id: 'passwordGeneratebutton',
        class: 'button button-icon validate ui-button ui-corner-all ui-widget',
        text: '自動生成',
        'data-action': 'GeneratePassword'
    }).appendTo($(passwordObject).closest('div'));

    $(document).on('click', '#passwordGeneratebutton', function () {
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