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
    var $text = $('#DropDownSearchText');
    var $target = $('#DropDownSearchTarget');
    $('#DropDownSearchParentClass').val($("#" + id).attr('parent-data-class'));
    $('#DropDownSearchParentDataId').val($("#" + id).attr('parent-data-id'));
    $('#DropDownSearchResults').empty();
    $target.val(id);
    $text.val('');
    $('#DropDownSearchOnEditor').val($('#Editor').length === 1);
    $('#DropDownSearchMultiple').val($control.attr('multiple') === 'multiple');
    $($('#DropDownSearchDialog')).dialog({
        title: $('label[for="' + id + '"]').text(),
        modal: true,
        width: '630px',
        close: function () {
            $('#' + $target.val()).prop("disabled", false);
        }
    });
    $p.send($text);
    $p.setPaging('DropDownSearchResults');
    $control.prop("disabled", true);
    $text.focus();
}