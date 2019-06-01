$p.id = function () {
    return $('#Id').val();
}

$p.siteId = function (title) {
    return title === undefined
        ? $('#SiteId').val()
        : JSON.parse($('#JoinedSites').val()).find(data => {
            return data.Title === title;
        }).SiteId;
}

$p.getColumnName = function (name) {
    var data = JSON.parse($('#Columns').val()).filter(function (column) {
        return column.LabelText === name || column.ColumnName === name
    });
    return data.length > 0
        ? data[0].ColumnName
        : undefined;
}

$p.getControl = function (name) {
    var columnName = $p.getColumnName(name);
    return columnName !== undefined
        ? $('#' + $('#ReferenceType').val() + '_' + columnName)
        : undefined;
}

$p.getGridRow = function (id) {
    return $('#Grid > tbody > tr[data-id="' + id + '"]');
}

$p.getGridCell = function (id, name) {
    return $('#Grid > tbody > tr[data-id="' + id + '"] td:nth-child(' + ($p.getGridColumnIndex(name) + 1) + ')');
}

$p.getGridColumnIndex = function (name) {
    return $('#Grid > thead > tr > th').index($('#Grid > thead > tr > th[data-name="' + $p.getColumnName(name) + '"]'));
}

$p.on = function (events, name, func) {
    $(document).on(events, '#' + $p.getControl(name).attr('id'), func);
}