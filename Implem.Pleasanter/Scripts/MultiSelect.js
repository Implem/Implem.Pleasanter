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

$p.selectMultiSelect = function ($control, json) {
    $control.find('option').each(function (index, element) {
        var $element = $(element);
        $element.prop('selected', false);
        if (JSON.parse(json).indexOf($element.val()) > -1) {
            $element.prop('selected', true);
        }
    })
    $control.multiselect('refresh');
}