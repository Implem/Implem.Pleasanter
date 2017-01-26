$(function () {
    $(document).on('focusin', '.control-dropdown.search', function () {
        $p.openDropDownSearchDialog($(this));
    });
});