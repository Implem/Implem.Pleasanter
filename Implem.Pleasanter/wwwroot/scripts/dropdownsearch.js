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
    if ($p.data.MainForm === undefined) {
        $p.data.MainForm = {};
    }
    if ($('#IsNew').val() === '1') {
        $p.data.MainForm.IsNew = '1';
    }
    $p.setMustData($('#MainForm'));
    $p.data.DropDownSearchDialogForm = $p.data.MainForm;
    var referenceId = $p.id();
    var $tr = $control.closest('tr');
    if ($tr.length) {
        referenceId = $tr.data('id');
    }
    var $dialogId = $('#EditorInDialogRecordId');
    if ($dialogId.length) {
        $p.data.DropDownSearchDialogForm = $p.data.DialogEditorForm;
        referenceId = $dialogId.val();
    }
    $('#DropDownSearchReferenceId').val(referenceId);
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
            width: '860px',
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
    } else {
        $($('#DropDownSearchDialog')).dialog('close');
    }
}