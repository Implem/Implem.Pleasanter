$(function () {
    $(document).on('focusin', '.control-dropdown.search', function () {
        if ($('#EditorLoading').val() === '1') {
            $(this).blur();
            $('#EditorLoading').val(0);
        } else {
            $p.openDropDownSearchDialog($(this));
        }
    });
});