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
    // 新規作成画面の場合にはIsNewのプロパティを$p.data.MainFormに転記
    if ($('#IsNew').val() === '1') {
        $p.data.MainForm.IsNew = '1';
    }
    // always-sendが指定されている項目をMainFormにセット
    $p.setMustData($('#MainForm'));
    // SetChoiceHashByFilterExpressions内で入力データを検証するためメインフォームの内容を転記
    $p.data.DropDownSearchDialogForm = $p.data.MainForm;
    var referenceId = $p.id();
    // 一覧編集の場合にはtrからreferenceIdを取得
    var $tr = $control.closest('tr');
    if ($tr.length) {
        referenceId = $tr.data('id');
    }
    // ダイアログ編集の場合にはEditorInDialogRecordIdからreferenceIdを取得
    // ダイアログ編集の場合には$p.data.DialogEditorFormから入力データを取得
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
    } else {
        $($('#DropDownSearchDialog')).dialog('close');
    }
}