$p.addSelected = function ($control, $target) {
    $control
        .closest('.container-selectable')
        .find('.ui-selected')
        .appendTo($target);
    $p.setData($target);
    $p.send($control);
}

$p.deleteSelected = function ($control) {
    var $targets = $control
        .closest('.container-selectable')
        .find('.control-selectable')
    $targets
        .find('.ui-selected')
        .remove();
    $p.setData($control);
    $p.send($control);
}