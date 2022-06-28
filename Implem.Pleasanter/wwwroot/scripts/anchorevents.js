$(function () {
    $(document).on('click', 'a', function (e) {
        e.stopPropagation();
    });
    $(document).on('blur', '.control-textbox.anchor:not(.error)', function () {
        $p.showAnchorViewer($(this));
    });
});