$p.authenticatebymail = function () {
    $('#AuthenticationByMail').on('click', function () {
        $('#AuthenticationByMail').data('isauthenticationbymail', '1');
        $('#SecondaryAuthenticationCode').remove();
        $p.send($('#Login'));
        if ($('#TotpRegister').length) {
            $('#TotpRegister').remove();
        }
    });
}

$p.isAuthenticationByMail = function () {
    if ($('#AuthenticationByMail').length) {
        return '&isAuthenticationByMail=' + $('#AuthenticationByMail').data('isauthenticationbymail');
    } else {
        return '';
    }
}