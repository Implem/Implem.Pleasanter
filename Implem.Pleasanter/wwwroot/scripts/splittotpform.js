$(function () {
    $('body').on('keydown', '.totp-authentication-code', function (e) {
        var totpForm = $(".totp-authentication-code");
        var columnNum = totpForm.index(this);
        var selectedForm = totpForm.eq(columnNum).get(0);
        if (
            e.code === "ArrowLeft" &&
            selectedForm.selectionStart === 0 &&
            columnNum !== 0
        ) {
            totpForm.eq(columnNum - 1).focus();
        } else if (
            e.code === "ArrowRight" &&
            (
                selectedForm.selectionStart === 1 ||
                totpForm.eq(columnNum).val() === ""
            ) &&
            columnNum !== 5
        ) {
            totpForm.eq(columnNum + 1).focus();
        }

        if (
            e.code === "Backspace" &&
            selectedForm.selectionStart === 0 &&
            columnNum !== 0
        ) {
            totpForm.eq(columnNum - 1).val("");
            totpForm.eq(columnNum - 1).focus();
        } else if (
            e.code === "Delete" &&
            (
                selectedForm.selectionStart === 1 ||
                totpForm.eq(columnNum).val() === ""
            ) &&
            columnNum !== 5) {
            totpForm.eq(columnNum + 1).val("");
        }
    });

    $('body').on('input', '.totp-authentication-code', function () {
        var totpForm = $(".totp-authentication-code");
        var columnNum = totpForm.index(this);
        var inputCode = totpForm.eq(columnNum).val();
        var totpCode = "";
        var secondaryAuthenticationCodeLength = 0;
        var totpCode = $("#SecondaryAuthenticationCode").val();
        var newTotpCode = "";


        var strLength = 6 - columnNum < inputCode.length ? 6 - columnNum : inputCode.length;
        var inputColumnNum = strLength + columnNum;

        if (inputCode.match(/[^0-9]+/)) {
            totpForm.eq(columnNum).val(inputCode.replace(/[^0-9]+/g, ""));
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

        $("#SecondaryAuthenticationCode").val(newTotpCode);

        setCodeSepalateForm(totpForm, newTotpCode);

        newTotpCode.length === 6 ? $("#SecondaryAuthenticate").focus() : totpForm.eq(newTotpCode.length).focus();
    });

    function setCodeSepalateForm(totpForm, totpCode) {
        for (var i = 0; i < 6; i++) {
            totpForm.eq(i).val(totpCode.charAt(i));
        }
    }
});
