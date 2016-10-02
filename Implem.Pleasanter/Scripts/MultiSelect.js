$p.changeMultiSelect = function ($control) {
    $p.setData($control);
    if ($control.hasClass('auto-postback') && !$control.hasClass('no-postback')) {
        $p.send($control);
    }
    $control.removeClass('no-postback');
}

$p.setMultiSelectData = function ($control) {
    $p.getData($control)[$control.attr('id')] = JSON.stringify(
        $('[name="multiselect_' + $control.attr('id') + '"]')
            .filter(function () { return $(this).prop('checked'); })
            .map(function () { return $(this).val() })
            .toArray());
}