$p.onSelectableSelected = function ($control) {
    $p.getData($control)[$control.attr('id')] = $control
        .find('.ui-selected')
        .map(function () { return $(this).attr('value'); })
        .get()
        .join(';');
}