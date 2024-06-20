$p.setGrid = function () {
    $p.paging('#Grid');
}
$p.setDashboardGrid = function () {
    var elementsWithGridId = document.querySelectorAll('[id*="Grid_"]');
    elementsWithGridId.forEach(function (element) {
        $(element).closest('.grid-stack-item').on('scroll resize', function () {
            $p.dashboardPaging(element.id, $(element).closest('.grid-stack-item'));
        });
    });
}

$p.openEditorDialog = function (id) {
    $p.data.DialogEditorForm = {};
    $('#EditorDialog').dialog({
        modal: true,
        width: '90%',
        resizable: false,
        open: function () {
            $('#EditorLoading').val(0);
            $p.initRelatingColumn();
        }
    });
}

$p.editOnGrid = function ($control, val) {
    if (val === 0) {
        if (!$p.confirmReload()) return false;
    }
    $('#EditOnGrid').val(val);
    $p.send($control);
}

$p.newOnGrid = function ($control) {
    $p.send($control, 'MainForm');
    $(window).scrollTop(0);
}

$p.copyRow = function ($control) {
    $p.getData($control).OriginalId = $control.closest('.grid-row').attr('data-id');
    $p.send($control);
    $(window).scrollTop(0);
}

$p.cancelNewRow = function ($control) {
    $p.getData($control).CancelRowId = $control.closest('.grid-row').attr('data-id');
    $p.send($control);
}

$p.selectedIds = function () {
    var $form = $('#MainForm');
    var url = $form.attr('action').replace('_action_', 'SelectedIds');
    var ret;
    if ($('#Token').length) {
        $p.data.MainForm.Token = $('#Token').val();
    }
    $.ajax({
        url: url,
        type: 'post',
        async: false,
        cache: false,
        data: $p.data.MainForm,
        dataType: 'json'
    })
        .done(function (json, textStatus, jqXHR) {
            ret = json;
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            ret = -1;
        });
    return ret;
}