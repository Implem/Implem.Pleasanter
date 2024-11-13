$p.responsive = function () {
    return $('#Responsive[type="hidden"]').val() === '1';
}

$p.id = function () {
    return parseInt($('#Id').val());
}

$p.ver = function () {
    return parseInt($('#Ver').val());
}

$p.siteId = function (title) {
    if (title === undefined) {
        return parseInt($('#SiteId').val());
    } else {
        var sites = JSON.parse($('#JoinedSites').val()).filter(function (data) {
            return data.Title === title;
        });
        return sites.length > 0
            ? sites[0].SiteId
            : undefined
    }
}

$p.loginId = function () {
    return $('#LoginId').val();
}

$p.deptId = function () {
    return parseInt($('#DeptId').val());
}

$p.userId = function () {
    return parseInt($('#UserId').val());
}

$p.userName = function () {
    return $('#AccountUserName').text();
}

$p.theme = function () {
    return $('#Theme').val();
}

$p.referenceType = function () {
    return $('#ReferenceType').val();
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

$p.getField = function (name) {
    var columnName = $p.getColumnName(name);
    return columnName !== undefined
        ? $('#' + $('#ReferenceType').val() + '_' + columnName + 'Field')
        : undefined;
}

$p.getGridRow = function (id) {
    return $('#Grid > tbody > tr[data-id="' + id + '"]');
}

$p.getGridCell = function (id, name, excludeHistory) {
    return $('#Grid > tbody > tr[data-id="' + id + '"]' + (excludeHistory? ':not([data-history])' : '') + ' td:nth-child(' + ($p.getGridColumnIndex(name) + 1) + ')');
}

$p.getGridColumnIndex = function (name) {
    return $('#Grid > thead > tr > th').index($('#Grid > thead > tr > th[data-name="' + $p.getColumnName(name) + '"]'));
}

$p.getValue = function (name) {
    var columnName = $p.getColumnName(name);
    if (columnName === undefined) {
        return undefined;
    }
    var element = $('#' + $('#ReferenceType').val() + '_' + columnName)[0];
    if (element === undefined) {
        return undefined;
    } else if (element.className === "control-checkbox") {
        return element.checked
    } else if (element.getAttribute('data-readonly') || "0" === "1") {
        return element.getAttribute('data-value') !== null
            ? element.getAttribute('data-value') 
            : element.textContent;
    } else {
        return $p.getControl(name).val();
    }
}

$p.on = function (events, name, func) {
    $(document).on(events, '#' + $p.getControl(name).attr('id'), func);
}