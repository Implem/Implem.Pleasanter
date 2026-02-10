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
    $(document).on('dblclick', '#DropDownSearchResults > li', function () {
        if ($('#ToDisableDropDownSearchResults').length) {
            $('#ToDisableDropDownSearchResults').click();
        } else {
            $('[data-action="SelectSearchDropDown"]').click();
        }
    });
    $(document).on('dblclick', '#DropDownSearchSourceResults > li', function () {
        $('#ToEnableDropDownSearchResults').click();
    });
});
