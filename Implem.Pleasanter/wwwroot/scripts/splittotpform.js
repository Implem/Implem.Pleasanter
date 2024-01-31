$(function () {
    $('body').on('keydown', '.totp-authentication-code', function (e) {
        var totpForm = $('.totp-authentication-code');
        var columnNum = totpForm.index(this);
        var selectedForm = totpForm.eq(columnNum).get(0);
        if (
            e.code === 'ArrowLeft' &&
            selectedForm.selectionStart === 0 &&
            columnNum !== 0
        ) {
            totpForm.eq(columnNum - 1).focus();
        } else if (
            e.code === 'ArrowRight' &&
            (
                selectedForm.selectionStart === 1 ||
                totpForm.eq(columnNum).val() === ''
            ) &&
            columnNum !== 5
        ) {
            totpForm.eq(columnNum + 1).focus();
        }
    });

    $('body').on('keyup', '.totp-authentication-code', function (e) {
        var totpForm = $('.totp-authentication-code');
        var columnNum = totpForm.index(this);
        var selectedForm = totpForm.eq(columnNum).get(0);
        var totpCode = $('#SecondaryAuthenticationCode').val();
        var isDeleted = false;
        if (
            e.code === 'Backspace' &&
            selectedForm.selectionStart === 0 &&
            columnNum !== 0
        ) {
            columnNum = columnNum - 1;
            totpForm.eq(columnNum).focus();
            isDeleted = true;
        } else if (
            e.code === 'Delete' &&
            (
                selectedForm.selectionStart === 1 ||
                totpForm.eq(columnNum).val() === ''
            ) &&
            columnNum !== 5) {
            columnNum = columnNum + 1;
            isDeleted = true;
        }
        if (isDeleted) {
            totpCode = totpCode.slice(0, columnNum) + totpCode.slice(columnNum + 1);
            $('#SecondaryAuthenticationCode').val(totpCode);
            setCodeSepalateForm(totpForm, totpCode);
        }
    });
    $('body').on('input', '.totp-authentication-code', function () {
        var totpForm = $('.totp-authentication-code');
        var columnNum = totpForm.index(this);
        var inputCode = totpForm.eq(columnNum).val();
        var inputedForm = totpForm.eq(columnNum).get(0);
        var totpCode = '';
        var secondaryAuthenticationCodeLength = 0;
        var totpCode = $('#SecondaryAuthenticationCode').val();
        var newTotpCode = '';
        if (inputCode.match(/[^0-9]+/)) {
            totpForm.eq(columnNum).val(inputCode.replace(/[^0-9]+/g, ''));
            return;
        }
        for (var i = 0; i < 6; i++) {
            secondaryAuthenticationCodeLength += totpForm.eq(i).val().length;
            newTotpCode += totpForm.eq(i).val();
        }
        if (secondaryAuthenticationCodeLength > 6) {
            setCodeSepalateForm(totpForm, totpCode);
            return;
        }
        $('#SecondaryAuthenticationCode').val(newTotpCode);
        setCodeSepalateForm(totpForm, newTotpCode);
        if (inputCode === '') {
            inputedForm.setSelectionRange(0, 0);
        } else {
            if (newTotpCode.length === 6) {
                $('#SecondaryAuthenticate').focus();
            } else if (newTotpCode.length <= columnNum + 1) {
                totpForm.eq(newTotpCode.length).focus();
            } else if (newTotpCode.length === columnNum + 2) {
                totpForm.eq(columnNum + 2).get(0).focus();
            }
        }
    });
    
    function setCodeSepalateForm(totpForm, totpCode) {
        for (var i = 0; i < 6; i++) {
            totpForm.eq(i).val(totpCode.charAt(i));
        }
    }
});
