$p.authenticatebymail = function () {
    $('#AuthenticationByMail').on('click', function () {
        $('#SecondaryAuthenticationCode').remove();
        $('#AuthenticationByMail').data('isauthenticationbymail', '1');
        $p.send($('#Login'));
        if ($('#TotpRegister').length) {
            $('#TotpRegister').remove();
        }
    });
}

$p.addAuthenticationByMailParameter = function (url) {
    if ($('#AuthenticationByMail').length) {
        return $p.addUrlParameter(url, 'isAuthenticationByMail', $('#AuthenticationByMail').data('isauthenticationbymail'));
    } else {
        return url;
    }
}