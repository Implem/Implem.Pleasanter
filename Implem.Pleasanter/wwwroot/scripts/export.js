﻿$p.openExportSelectorDialog = function ($control) {
    error = $p.syncSend($control);
    if (error === 0) {
        $('#ExportSelectorDialog').dialog({
            modal: true,
            width: '420px',
            resizable: false
        });
    }
}

$p.export = function () {
    var data = $p.getData($('.main-form'));
    var exp = JSON.parse($('#ExportId').val());
    var encoding = $('#ExportEncoding').val();
    if (exp.mailNotify === true) {
        data["ExportId"] = exp.id;
        $p.send($("#DoExport"), "MainForm");
    } else {
        var addInput = function (form, name, value) {
            if (!name) return;
            var input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('name', name);
            input.setAttribute('value', value);
            form.appendChild(input);
        }
        var form = document.createElement('form');
        form.style.display = 'none';
        var action = $('.main-form').attr('action').replace('_action_', 'export');
        form.setAttribute('action', action);
        form.setAttribute('method', 'post');
        addInput(form, 'ExportId', exp.id);
        addInput(form, 'ExportEncoding', encoding);
        addInput(form, 'GridCheckAll', data.GridCheckAll);
        addInput(form, 'GridUnCheckedItems', data.GridUnCheckedItems);
        addInput(form, 'GridCheckedItems', data.GridCheckedItems);
        if ($('#Token').length) {
            addInput(form, 'Token', $('#Token').val());
        }
        if ($('#DoExport').hasClass('save-view-types-none')) {
            (Object.keys(data)).forEach(function (e) {
                if (e.match(/^ViewFilters_/)) {
                    addInput(form, e, data[e]);
                }
            });
        }
        document.body.appendChild(form);
        form.submit();
    }
    $p.closeDialog($('#ExportSelectorDialog'));
}

$p.exportCrosstab = function () {
    $p.transition($('.main-form').attr('action').replace('_action_', 'exportcrosstab'));
}

$p.addExportAccessControl = function () {
    $('#SourceExportAccessControl li.ui-selected').appendTo('#CurrentExportAccessControl');
    $p.setData($('#CurrentExportAccessControl'));
}

$p.deleteExportAccessControl = function () {
    $('#CurrentExportAccessControl li.ui-selected').appendTo('#SourceExportAccessControl');
}