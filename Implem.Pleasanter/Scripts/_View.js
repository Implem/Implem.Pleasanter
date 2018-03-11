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