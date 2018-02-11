$p.setValue = function ($control, value) {
    switch ($control.prop('type')) {
        case 'checkbox':
            $control.prop('checked', value);
            break;
        case 'radio':
            $control.val([value]);
            break;
        default:
            switch ($control.prop('tagName')) {
                case 'SPAN':
                    $control.html(value);
                case 'TIME':
                    $control.html(value);
                    $control.attr('datetime', value);
                    break;
                default:
                    $control.val(value);
                    break;
            }
    }
}

$p.setMessage = function (target, value) {
    var $control = target !== undefined
        ? $(target)
        : $('.message-dialog:visible');
    if ($control.length === 0) {
        $('#Message').html(value);
    } else {
        $control.html(value);
    }
}

$p.setErrorMessage = function (error) {
    $('#Message').html(
        '<span class="alert-error">' + $p.display(error) + '</span>');
}

$p.clearMessage = function () {
    $('[class*="message"]').html('');
}