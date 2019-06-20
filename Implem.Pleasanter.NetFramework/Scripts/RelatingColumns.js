$(document).ready(function () {
    var param = $('#TriggerRelatingColumns').val();
    if (param === undefined) return;
    var rcols = JSON.parse(param);
    for (var k in rcols) {
        var prekey = '';
        for (var k2 in rcols[k].Columns) {
            if (prekey !== '' && rcols[k].Columns[k2] !== null && rcols[k].ColumnsLinkedClass[rcols[k].Columns[k2]] !== null) {
                $p.applyRelatingColumn(prekey, rcols[k].Columns[k2], rcols[k].ColumnsLinkedClass[rcols[k].Columns[k2]]);
            }
            prekey = rcols[k].Columns[k2];
        }
    }
});

$p.applyRelatingColumn = function (prnt, chld, linkedClass) {
    $(document).ready(function () {
        var tablename = $('#TableName').val();
        if (tablename === undefined) return;
        var siteid = $('#' + tablename + '_' + chld).attr('data-id');
        if (siteid === undefined || isNaN(siteid)) return;
        c_change(tablename, siteid, linkedClass);
        $('#' + tablename + '_' + prnt).change(function () {
            c_change(tablename, siteid, linkedClass);
        });
    });

    var c_change = function (tablename, siteid, linkedClass) {
        var parentId = $('#' + tablename + '_' + prnt + ' option:selected').val();
        var childId = $('#' + tablename + '_' + chld + ' option:selected').val();
        $('#' + tablename + '_' + chld).prop('disabled', true);
        $('#' + tablename + '_' + chld).empty();
        $('#' + tablename + '_' + chld)
            .append($('<option>')
                .val(childId)
                .prop('id', 'Temporary_' + chld)
                .prop('selected', true));
        $('#' + tablename + '_' + chld).append($('<option>').val(''));
        refreshCombo(tablename, siteid, null, parentId, childId, false);
    };

    var refreshCombo = function (tablename, siteid, json, parentSelectedId, childSelectedId, childSelected) {
        $('#' + tablename + '_' + chld).attr('parent-data-class', linkedClass);
        $('#' + tablename + '_' + chld).attr('parent-data-id', parentSelectedId === '' ? '-1' : parentSelectedId);
        var offset = 0;
        var pagesize = 0;
        var totalcount = 0;
        if (json !== null) {
            offset = json.Response.Offset;
            pagesize = json.Response.PageSize;
            totalcount = json.Response.TotalCount;
            var loopmax = ((pagesize + offset) < totalcount) ?
                pagesize : (totalcount - offset);
            for (var i = 0; i < loopmax; i++) {
                var id = json.Response.Data[i].ResultId;
                if (id == undefined) id = json.Response.Data[i].IssueId;
                var title = json.Response.Data[i].ItemTitle;
                var isSelected = false;
                if (id == childSelectedId) {
                    childSelected = true;
                    isSelected = true;
                }
                $('#' + tablename + '_' + chld).append($('<option>').val(id).prop('selected', isSelected).text(title));
            }
        }
        if (json === null || (offset + pagesize) < totalcount) {
            var param = new Object();
            param.ApiKey = '';
            param.Offset = offset + pagesize;
            param.View = new Object();
            param.View.ColumnFilterHash = new Object();
            param.View.ColumnFilterHash[linkedClass] = '["' + parentSelectedId + '"]';
            param.View.ColumnSorterHash = new Object();
            param.View.ColumnSorterHash['ItemTitle'] = 0;
            var urlpath = $('#ApplicationPath').val() +
                'api/items/' + escape((siteid - 0)) + '/get';
            $.ajax({
                type: 'POST',
                url: urlpath,
                dataType: 'json',
                data: JSON.stringify(param),
                contentType: 'application/json',
                scriptCharset: 'utf-8',
                async: false
            }).done(function (json) {
                if (json.StatusCode == 200) {
                    refreshCombo(tablename, siteid, json, parentSelectedId, childSelectedId, childSelected);
                } else {
                    alert('Error\r\nStatusCode:' + json.StatusCode);
                }
            }).fail(function (XMLHttpRequest, textStatus, errorThrown) {
                alert(textStatus + '\r\n' + errorThrown);
                $('#' + tablename + '_' + chld).prop('disabled', false);
            });
        } else {
            if (childSelectedId > 0
                && childSelectedId !== undefined
                && !childSelected) {
                $('#' + tablename + '_' + chld).val('');
                $('#' + tablename + '_' + chld).trigger('change');
            }
            $('#Temporary_' + chld).remove();
            $('#' + tablename + '_' + chld).prop('disabled', false);
        }
    };
}