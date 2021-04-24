$p.setDropDownSearch = function () {
    var $control = $('[id="' + $('#DropDownSearchTarget').val() + '"]');
    if ($control.attr('multiple') === 'multiple') {
        $control.multiselect('refresh');
    }
    $p.setData($control);
    if ($control.hasClass('auto-postback')) {
        $p.send($control);
    }
    if ($control.val() !== '' && $control.hasClass('error')) {
        $control.removeClass('error');
        $('[id="' + $control.attr('id') + '-error"]').remove();
    }
}

$p.openDropDownSearchDialog = function ($control) {
    var id = $control.attr('id');
    var $target = $('#DropDownSearchTarget');
    var multiple = $control.attr('multiple') === 'multiple';
    $('#DropDownSearchSelectedValues').val(JSON.stringify($control.val()));
    $('#DropDownSearchParentClass').val($("#" + id).attr('parent-data-class'));
    $('#DropDownSearchParentDataId').val($("#" + id).attr('parent-data-id'));
    $('#DropDownSearchMultiple').val(multiple);
    $target.val(id);
    var error = $p.syncSend($target);
    if (error === 0) {
        $($('#DropDownSearchDialog')).dialog({
            title: $('.field-label label[for="' + id + '"]').text(),
            modal: true,
            width: '750px',
            resizable: false,
            close: function () {
                $('#' + $target.val()).prop("disabled", false);
            }
        });
        if (multiple) {
            $p.setPaging('DropDownSearchSourceResults', 'DropDownSearchResultsOffset');
        } else {
            $p.setPaging('DropDownSearchResults');
        }
        $control.prop("disabled", true);
        $('#DropDownSearchText').focus();
    }
}