$p.moveColumns = function ($control) {
    if ($p.outsideDialog($control)) {
        alert("outsideDialog");
        return false;
    }
    if (formId === undefined) return false;
    if ($control.attr('id') === undefined || $control.attr('id') === null) return false;
    $form = $('#' + formId);
    var columnsId = formId.replace('Form', 'Columns');
    var srcColumnsId = formId.replace('Form', 'SourceColumns');
    var controlId = $control.attr('id');
    var mode = 0;
    if (controlId.indexOf('MoveUp') === 0) mode = 1;
    if (controlId.indexOf('MoveDown') === 0) mode = 2;
    if (controlId.indexOf('ToDisable') === 0) mode = 3;
    if (controlId.indexOf('ToEnable') === 0) mode = 4;
    if (mode === 0) return false;
    var data = $p.getData($form);
    var liListPool = [];
    var beforeColumns = [];
    var afterColumns = [];
    var beforeSourceColumns = [];
    var afterSourceColumns = [];
    var i = 0; j = 0;
    var selected = $('#' + columnsId + ' li').map(function (i, elm) {
        if ($(this).hasClass('ui-selected')) return $(this).attr('data-value');
    });
    var srcSelected = $('#' + srcColumnsId + ' li').map(function (i, elm) {
        if ($(this).hasClass('ui-selected')) return $(this).attr('data-value');
    });
    $('#' + columnsId + ' li').each(function (index, element) {
        beforeColumns.push($(element).attr("data-value"));
    });
    $('#' + srcColumnsId + ' li').each(function (index, element) {
        beforeSourceColumns.push($(element).attr("data-value"));
    });
    afterSourceColumns = [].concat(beforeSourceColumns);
    if (mode === 1 || mode === 2) {
        if (mode === 1) {
            beforeColumns = beforeColumns.reverse();
        }
        for (i = 0; i < beforeColumns.length; i++) {
            if (selected.get().indexOf(beforeColumns[i]) >= 0) {
                liListPool.push(beforeColumns[i]);
            }
            else {
                afterColumns.push(beforeColumns[i]);
                if (liListPool.length > 0) {
                    afterColumns = afterColumns.concat(liListPool);
                    liListPool = [];
                }
            }
        }
        if (liListPool.length > 0) {
            afterColumns = afterColumns.concat(liListPool);
            liListPool = [];
        }
        if (mode === 1) {
            beforeColumns = beforeColumns.reverse();
            afterColumns = afterColumns.reverse();
        }
    }
    else if (mode === 3) {
        afterColumns = [].concat(beforeColumns);
        var pos = afterSourceColumns.length;
        for (i = 0; i < selected.length; i++) {
            pos = afterSourceColumns.length;
            afterColumns.splice(afterColumns.indexOf(selected[i]), 1);
            if ($('#' + formId + ' li[data-value="' + selected[i] + '"]').attr('data-order') !== undefined) {
                for (j = 0; j < afterSourceColumns.length; j++) {
                    if ($('#' + formId + ' li[data-value="' + afterSourceColumns[j] + '"]').attr('data-order') === undefined) break;
                    if (parseInt($('#' + formId + ' li[data-value="' + selected[i] + '"]').attr('data-order'), 10)
                        < parseInt($('#' + formId + ' li[data-value="' + afterSourceColumns[j] + '"]').attr('data-order'), 10)) {
                        pos = j;
                        break;
                    }
                }
            }
            afterSourceColumns.splice(pos, 0, selected[i]);
        }
    }
    else if (mode === 4) {
        afterColumns = [].concat(beforeColumns);
        for (i = 0; i < srcSelected.length; i++) {
            afterColumns.push(srcSelected[i]);
            afterSourceColumns.splice(afterSourceColumns.indexOf(srcSelected[i]), 1);
        }
    }
    var html = '';
    var srchtml = '';
    var o = null;
    for (i = 0; i < afterColumns.length; i++) {
        o = $('#' + formId + ' li[data-value="' + afterColumns[i] + '"]');
        if ($(o).get(0)) {
            html += $(o).get(0).outerHTML;
        }
    }
    for (i = 0; i < afterSourceColumns.length; i++) {
        o = $('#' + formId + ' li[data-value="' + afterSourceColumns[i] + '"]');
        if ($(o).get(0)) {
            srchtml += $(o).get(0).outerHTML;
        }
    }
    $('#' + columnsId).html(html);
    $('#' + srcColumnsId).html(srchtml);
};
