$p.showPassword = function (element) {
    var passwordElement = $(element).prevAll('input');
    if (passwordElement.attr('type') === 'password') {
        passwordElement.attr('type', 'text');
        $(element).text('visibility_off');
    } else {
        passwordElement.attr('type', 'password');
        $(element).text('visibility');
    }
}