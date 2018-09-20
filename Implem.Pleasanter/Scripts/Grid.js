$p.setGrid = function () {
    $p.paging('#Grid');
}

$p.openEditorDialog = function (id) {
    $p.data.DialogEditorForm = {};
    $('#EditorDialog').dialog({
        modal: true,
        width: '90%',
        open: function () {
            $('#EditorLoading').val(0);
        }
    });
}