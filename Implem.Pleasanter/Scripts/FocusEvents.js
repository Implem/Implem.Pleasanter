$(function () {
    var $control = $('.control-textbox.focus');
    if ($control.length !== 0) {
        setTimeout(function () {
            $control.focus();
        }, 0);
    }
});