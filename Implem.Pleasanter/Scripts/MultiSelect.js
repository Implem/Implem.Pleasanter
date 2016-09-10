$p.changeMultiSelect = function ($control) {
    $p.setData($control);
    if ($control.hasClass('auto-postback')) {
        $p.send($control, $p.getIdByInnerElement($control));
    }
}

$p.setMultiSelectData = function ($control) {
    $p.getDataByInnerElement($control)[$control.attr('id')] = JSON.stringify(
        $('[name="multiselect_' + $control.attr('id') + '"]')
            .filter(function () { return $(this).prop('checked'); })
            .map(function () { return $(this).val() })
            .toArray());
}