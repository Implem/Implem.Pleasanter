$p.enableColumns = function (event, $control, columnHeader, columnsTypeControl) {
    if ($('#' + columnsTypeControl + " option:selected").attr("data-type") === 'multiple') {
        $p.send($control);
    }
    else {
        $p.moveColumns(event, $control, columnHeader);
    }
}
$p.moveColumns = function (event, $control, columnHeader, isKeepSource, isJoin, type) {
    if (formId === undefined) return false;
    if (type === undefined) type = 'Columns';
    return $p.moveColumnsById(event, $control,
        columnHeader + type,
        columnHeader + 'Source' + type,
        isKeepSource,
        isJoin !== undefined && isJoin === true ? columnHeader + 'Join' : undefined);
};
$p.moveAllColumns = function (event, $control, columnHeader, isKeepSource, isJoin, type) {
    $control.closest(".container-selectable").find(".ui-selectee").addClass("ui-selected");
    $p.moveColumns(event, $control, columnHeader, isKeepSource, isJoin, type);
}
$p.moveColumnsById = function (event, $control, columnsId, srcColumnsId, isKeepSource, joinId) {
    if ($p.outsideDialog($control)) {
        alert("outsideDialog");
        return false;
    }
    if (formId === undefined) return false;
    if ($control.attr('id') === undefined || $control.attr('id') === null) return false;
    $form = $('#' + formId);
    var controlId = $control.attr('id');
    var mode = 0;
    var keepSource = (isKeepSource !== undefined && isKeepSource === true);
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
    var o = null;
    var selected = $('#' + columnsId + ' li').map(function (i, elm) {
        if ($(this).hasClass('ui-selected')) return $(this).attr('data-value');
    });
    $('#' + columnsId + ' li').each(function (index, element) {
        beforeColumns.push($(element).attr("data-value"));
    });
    if (srcColumnsId !== '') {
        var srcSelected = $('#' + srcColumnsId + ' li').map(function (i, elm) {
            if ($(this).hasClass('ui-selected')) return $(this).attr('data-value');
        });
        $('#' + srcColumnsId + ' li').each(function (index, element) {
            beforeSourceColumns.push($(element).attr("data-value"));
        });
    }
    afterSourceColumns = [].concat(beforeSourceColumns);
    if (mode === 1 || mode === 2) {
        if (mode === 1) {
            beforeColumns = beforeColumns.reverse();
        }
        for (i = 0; i < beforeColumns.length; i++) {
            if (selected.get().indexOf(beforeColumns[i]) >= 0) {
                // 選択項目をPoolに退避する処理はCTRLキー併用クリック時にはやらず最後に処理する
                if (!event.ctrlKey) liListPool.push(beforeColumns[i]);
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
        // CTRLキー併用時に一番上 or 下に選択項目を移動する
        if (event.ctrlKey) {
            var toTopOrBottom = [].concat(selected.get());
            if (mode === 1) {
                afterColumns = toTopOrBottom.concat(afterColumns);
            }
            else {
                afterColumns = afterColumns.concat(toTopOrBottom);
            }
        }
    }
    else if (mode === 3) {
        $('#' + srcColumnsId + ' li').each(function (i, elm) {
            if ($(this).hasClass('ui-selected')) $(this).removeClass('ui-selected');
        });
        if ($('#' + columnsId + 'NessesaryColumns')) {
            var param = $('#' + columnsId + 'NessesaryColumns').val();
            if (param !== undefined) {
                var nessesaryColumns = JSON.parse(param);
                for (i = 0; i < selected.length; i++) {
                    if (nessesaryColumns.indexOf(selected[i]) >= 0) {
                        alert($('#' + columnsId + 'NessesaryMessage').val()
                            .replace("COLUMNNAME", $('#' + columnsId + ' li[data-value=\'' + selected[i] + '\'').html()));
                        return false;
                    }
                }
            }
        }
        afterColumns = [].concat(beforeColumns);
        var pos = afterSourceColumns.length;
        for (i = 0; i < selected.length; i++) {
            pos = afterSourceColumns.length;
            afterColumns.splice(afterColumns.indexOf(selected[i]), 1);
            if ((joinId === undefined
                || ($('#' + joinId + ' option:selected').val() !== '' && selected[i].indexOf($('#' + joinId + ' option:selected').val() + ',') >= 0)
                || ($('#' + joinId + ' option:selected').val() === '' && selected[i].indexOf(',') < 0))
                && !keepSource) {
                if ($('#' + columnsId + ' li[data-value=\'' + selected[i] + '\']').attr('data-order') !== undefined) {
                    for (j = 0; j < afterSourceColumns.length; j++) {
                        o = $('#' + columnsId + ' li[data-value=\'' + afterSourceColumns[j] + '\']');
                        if (!$(o).get(0)) o = $('#' + srcColumnsId + ' li[data-value=\'' + afterSourceColumns[j] + '\']');
                        if ($(o).attr('data-order') === undefined) break;
                        if (parseInt($('#' + columnsId + ' li[data-value=\'' + selected[i] + '\']').attr('data-order'), 10)
                            < parseInt($(o).attr('data-order'), 10)) {
                            pos = j;
                            break;
                        }
                    }
                }
                afterSourceColumns.splice(pos, 0, selected[i]);
            }
        }
    }
    else if (mode === 4) {
        $('#' + columnsId + ' li').each(function (i, elm) {
            if ($(this).hasClass('ui-selected')) $(this).removeClass('ui-selected');
        });
        afterColumns = [].concat(beforeColumns);
        for (i = 0; i < srcSelected.length; i++) {
            afterColumns.push(srcSelected[i]);
            if (!keepSource) afterSourceColumns.splice(afterSourceColumns.indexOf(srcSelected[i]), 1);
        }
    }
    var html = '';
    var srchtml = '';
    o = null;
    for (i = 0; i < afterColumns.length; i++) {
        o = $('#' + columnsId + ' li[data-value=\'' + afterColumns[i] + '\']');
        if (!$(o).get(0)) {
            o = $('#' + srcColumnsId + ' li[data-value=\'' + afterColumns[i] + '\']');
        }
        if ($(o).get(0)) {
            html += $(o).get(0).outerHTML;
        }
    }
    for (i = 0; i < afterSourceColumns.length; i++) {
        o = $('#' + columnsId + ' li[data-value=\'' + afterSourceColumns[i] + '\']');
        if (!$(o).get(0)) {
            o = $('#' + srcColumnsId + ' li[data-value=\'' + afterSourceColumns[i] + '\']');
        }
        if ($(o).get(0)) {
            srchtml += $(o).get(0).outerHTML;
        }
    }
    $('#' + columnsId).html(html);
    if (srcColumnsId !== '') {
        $('#' + srcColumnsId).html(srchtml);
        if (keepSource) {
            $('#' + srcColumnsId + ' li').removeClass('ui-selected');
        }
    }
};