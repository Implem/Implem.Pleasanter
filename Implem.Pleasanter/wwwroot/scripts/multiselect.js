$p.changeMultiSelect = function ($control) {
    $p.setData($control);
    if (!$control.hasClass('no-postback')) {
        if ($control.hasClass('control-auto-postback')) {
            $p.controlAutoPostBack($(this));
        } else if ($control.hasClass('auto-postback')) {
            $p.send($control);
        }
    }
    $control.removeClass('no-postback');
};

$p.setMultiSelectData = function ($control) {
    $p.getData($control)[$control.attr('id')] = JSON.stringify(
        $('[name="multiselect_' + $control.attr('id') + '"]')
            .filter(function () { return $(this).prop('checked'); })
            .map(function () { return $(this).val() })
            .toArray());
};

$p.selectMultiSelect = function ($control, json) {
    $control.find('option').each(function (index, element) {
        var $element = $(element);
        $element.prop('selected', false);
        var selected = json
            ? json
            : '[]';
        if (JSON.parse(selected).indexOf($element.val()) > -1) {
            $element.prop('selected', true);
        }
    });
    $control.multiselect('refresh');
};

$p.RefreshMultiSelectRelatingColum = function ($target) {
    if ($target.length === 0) {
        return;
    }
    if ($target.prop('multiple')) {
        $target.multiselect('refresh');
        var $currentOptions = $('#' + $target.attr('id') + ' option:selected');
        var curOptions = [];
        $currentOptions.each(function (index, element) {
            curOptions.push($(element).val());
        });
        var selected = $target.attr('selected-options')
            ? $target.attr('selected-options')
            : '[]';
        var oldOptions = JSON.parse(selected);
        if (!Array.isArray(oldOptions)) {
            return false;
        }
        if (curOptions.length !== oldOptions.length) {
            $p.setData($target);
            $p.send($target);
            return;
        }
        var equals = true;
        curOptions.forEach(function (item) {
            if (!oldOptions.includes(item)) {
                equals = false;
            }
        });
        if (!equals) {
            $p.setData($target);
            $p.send($target);
        }
    }
};