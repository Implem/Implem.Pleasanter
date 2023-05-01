$p.showPassword = function () {
    $('#Users_Password').attr('type') === 'password'
        ? $('#Users_Password').attr('type', 'text')
        : $('#Users_Password').attr('type', 'password');
}