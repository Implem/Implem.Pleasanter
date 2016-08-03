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