$p.focusMainForm = function () {
    $('#FieldSetGeneral').find('[class^="control-"]').each(function () {
        if (!$(this).is(':hidden') &&
            !$(this).hasClass('control-text') &&
            !$(this).hasClass('control-markup')) {
            $(this).focus();
            return false;
        }
    });
}

$(function () {
    if ($('.focus').length !== 0) {
        setTimeout(function () {
            $('.focus').focus();
        }, 0);
    }
});