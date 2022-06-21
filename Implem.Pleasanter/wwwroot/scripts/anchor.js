$p.toggleAnchor = function ($control) {
    if ($control.is(':visible')) {
        $p.toggleInput($control, false);
    } else {
        $p.toggleInput($control, true);
        $($control).focus();
    }
}

$p.toggleInput = function ($control, edit) {
    var id = $control.attr('id');
    if ($('[id="' + id + '.editor"]').length !== 0) {
        $('[id="' + id + '.viewer"]').toggle(!edit);
        $('[id="' + id + '"]').toggle(edit);
    }
}
