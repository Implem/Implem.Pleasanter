$(function () {
    $(document).on('focusin', '.control-dropdown.search', function (e) {
        e.preventDefault();
        if ($('#EditorLoading').val() === '1') {
            $(this).blur();
            $('#EditorLoading').val(0);
        } else {
            $p.openDropDownSearchDialog($(this));
        }
    });
});
