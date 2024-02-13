$p.setValue = function ($control, value) {
    switch ($control.prop('type')) {
        case 'checkbox':
            $control.prop('checked', value);
            break;
        case 'radio':
            $control.val([value]);
            break;
        case 'textarea':
            $control.val(value);
            $p.showMarkDownViewer($control);
            break;
        default:
            switch ($control.prop('tagName')) {
                case 'SELECT':
                    if ($control.attr('multiple')) {
                        $p.selectMultiSelect($control, value);
                    } else {
                        $control.val(value);
                    }
                    break;
                case 'SPAN':
                    $control.html(value);
                case 'TIME':
                    $control.html(value);
                    $control.attr('datetime', value);
                    break;
                default:
                    if ($control.hasClass('radio-value')) {
                        $('input[name="' + $control.attr('id') + '"]').val([value]);
                    }
                    $control.val(value);
                    break;
            }
    }
}