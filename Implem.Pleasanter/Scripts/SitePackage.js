$p.openImportSitePackageDialog = function ($control) {
    error = $p.syncSend($control, 'MainForm');
    if (error === 0) {
        $('#ImportSitePackageDialog').dialog({
            modal: true,
            width: '520px'
        });
    }
}

$p.importSitePackage = function ($control) {
    $p.setData($('#IncludeData'));
    $p.setData($('#IncludeSitePermission'));
    $p.setData($('#IncludeRecordPermission'));
    $p.setData($('#IncludeColumnPermission'));
    var data = new FormData();
    data.append('file', $('#Import').prop('files')[0]);
    data.append('IncludeData', $('#IncludeData').prop('checked'));
    data.append('IncludeSitePermission', $('#IncludeSitePermission').prop('checked'));
    data.append('IncludeRecordPermission', $('#IncludeRecordPermission').prop('checked'));
    data.append('IncludeColumnPermission', $('#IncludeColumnPermission').prop('checked'));
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}

$p.openExportSitePackageDialog = function ($control) {
    error = $p.syncSend($control, 'MainForm');
    if (error === 0) {
        $('#ExportSitePackageDialog').dialog({
            modal: true,
            width: '720px'
        });
    }
}

$p.exportSitePackage = function () {
    $p.setData($('#SitePackagesSelectable'));
    $p.setData($('#SitePackagesSelectableAll'));
    $p.setData($('#SitePackagesSource'));
    $p.setData($('#UseIndentOption'));
    $p.setData($('#IncludeSitePermission'));
    $p.setData($('#IncludeRecordPermission'));
    $p.setData($('#IncludeColumnPermission'));
    var data = $p.getData($('#SitePackageForm'));
    location.href = $('.main-form').attr('action').replace('_action_', 'ExportSitePackage')
        + '?SitePackagesSelectableAll=' + data.SitePackagesSelectableAll
        + '&UseIndentOption=' + data.UseIndentOption
        + '&IncludeSitePermission=' + data.IncludeSitePermission
        + '&IncludeRecordPermission=' + data.IncludeRecordPermission
        + '&IncludeColumnPermission=' + data.IncludeColumnPermission;
    $p.closeDialog($('#ExportSitePackageDialog'));
    $('#ExportSitePackageDialog').html('');
}

$p.siteSelected = function ($control, $target) {
    $control
        .closest('.container-selectable')
        .find('.ui-selected[data-value!=' + $('#Id').val() + ']')
        .appendTo($target);
    var $itemsContainer = $('#SitePackagesSelectable');
    var $items = $('#SitePackagesSelectable li');
    $items.sort(function (a, b) {
        return parseInt($(a).attr('data-order')) > parseInt($(b).attr('data-order'))
            ? 1
            : -1;
    });
    $itemsContainer.html('');
    $items.each(function () {
        $itemsContainer.append($(this));
    });
    $p.setData($target);
}

$p.setIncludeExportData = function ($control) {
    $('#SitePackagesSelectable').find('.ui-selected').each(function () {
        var $selected = $(this);
        if ($selected.attr('data-value') == undefined) {
            return true;
        }
        var data = $selected.attr('data-value').split('-');
        var type = $control.attr('id');
        $selected.attr('data-value', data[0] + '-' + (type === 'IncludeData'));
        $selected.find('span.include-data').remove();
        if (type === 'IncludeData') {
            $selected.append('<span class="include-data">(' + $p.display('IncludeData') + ')</span>');
        }
    });
    $p.setData($('#SitePackagesSelectable'));
}