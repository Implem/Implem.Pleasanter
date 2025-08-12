$p.toggleAnchor = function ($control) {
    if ($control.is(':visible')) {
        $p.toggleInput($control, false);
    } else {
        $p.toggleInput($control, true);
        $($control).focus();
    }
};

$p.toggleInput = function ($control, edit) {
    var id = $control.attr('id');
    if ($('[id="' + id + '.editor"]').length !== 0) {
        $('[id="' + id + '.viewer"]').toggle(!edit);
        $('[id="' + id + '"]').toggle(edit);
    }
};

$p.showAnchorViewer = function ($control) {
    var $viewer = $('[id="' + $control.attr('id') + '.viewer"]');
    if ($viewer.length === 1) {
        var anchorTag = $viewer.children('a')[0];
        var anchorFormat = $viewer.attr('data-format');
        anchorTag.href = anchorFormat.replace('{Value}', $control.val());
        anchorTag.text = $control.val();
        $p.toggleInput($control, false);
    }
};
