$p.openImportSitePackageDialog = function ($control) {
    var error = $p.syncSend($control, 'MainForm');
    if (error === 0) {
        $('#ImportSitePackageDialog').dialog({
            modal: true,
            width: '520px'
        });
    }
    if ($p.responsive() && screen.width < 1025) {
        $p.openResponsiveMenu();
    }
};

$p.importSitePackage = function ($control) {
    $p.setData($('#IncludeData'));
    $p.setData($('#IncludeSitePermission'));
    $p.setData($('#IncludeRecordPermission'));
    $p.setData($('#IncludeColumnPermission'));
    $p.setData($('#IncludeNotifications'));
    $p.setData($('#IncludeReminders'));
    var data = new FormData();
    data.append('file', $('#Import').prop('files')[0]);
    data.append('IncludeData', $('#IncludeData').prop('checked'));
    data.append('IncludeSitePermission', $('#IncludeSitePermission').prop('checked'));
    data.append('IncludeRecordPermission', $('#IncludeRecordPermission').prop('checked'));
    data.append('IncludeColumnPermission', $('#IncludeColumnPermission').prop('checked'));
    data.append('IncludeNotifications', $('#IncludeNotifications').prop('checked'));
    data.append('IncludeReminders', $('#IncludeReminders').prop('checked'));
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control
    );
};

$p.openExportSitePackageDialog = function ($control) {
    var error = $p.syncSend($control, 'MainForm');
    if (error === 0) {
        $('#ExportSitePackageDialog').dialog({
            modal: true,
            width: '720px'
        });
        $('#SitePackagesSelectable li').each(function () {
            var $li = $(this);
            var data = $li.attr('data-value').split('-');
            if (data[1] === 'true') {
                $li.append('<span class="include-data">(' + $p.display('IncludeData') + ')</span>');
            }
        });
    }
    if ($p.responsive() && screen.width < 1025) {
        $p.openResponsiveMenu();
    }
};

$p.exportSitePackage = function () {
    var addInput = function (form, name, value) {
        if (!name) return;
        var input = document.createElement('input');
        input.setAttribute('type', 'hidden');
        input.setAttribute('name', name);
        input.setAttribute('value', value);
        form.appendChild(input);
    };
    var form = document.createElement('form');
    form.style.display = 'none';
    var action = $('#SitePackageForm').attr('action').replace('_action_', 'ExportSitePackage');
    form.setAttribute('action', action);
    form.setAttribute('method', 'post');
    $p.setData($('#SitePackagesSelectable'));
    $p.setData($('#SitePackagesSelectableAll'));
    $p.setData($('#SitePackagesSource'));
    $p.setData($('#UseIndentOption'));
    $p.setData($('#IncludeSitePermission'));
    $p.setData($('#IncludeRecordPermission'));
    $p.setData($('#IncludeColumnPermission'));
    $p.setData($('#IncludeNotifications'));
    $p.setData($('#IncludeReminders'));
    var data = $p.getData($('#SitePackageForm'));
    addInput(form, 'SitePackagesSelectable', data.SitePackagesSelectable);
    addInput(form, 'SitePackagesSelectableAll', data.SitePackagesSelectableAll);
    addInput(form, 'SitePackagesSource', data.SitePackagesSource);
    addInput(form, 'UseIndentOption', data.UseIndentOption);
    addInput(form, 'IncludeSitePermission', data.IncludeSitePermission);
    addInput(form, 'IncludeRecordPermission', data.IncludeRecordPermission);
    addInput(form, 'IncludeColumnPermission', data.IncludeColumnPermission);
    addInput(form, 'IncludeNotifications', data.IncludeNotifications);
    addInput(form, 'IncludeReminders', data.IncludeReminders);
    if ($('#Token').length) {
        addInput(form, 'Token', $('#Token').val());
    }
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
    $p.closeDialog($('#ExportSitePackageDialog'));
    $('#ExportSitePackageDialog').html('');
};

$p.siteSelected = function ($control, $target) {
    $control
        .closest('.container-selectable')
        .find('.ui-selected[data-value!=' + $('#Id').val() + ']')
        .appendTo($target);
    var container = document.getElementById('SitePackagesSelectable');
    var items = container.getElementsByTagName('li');
    var itemArray = Array.prototype.slice.call(items);
    function compareText(a, b) {
        var _a = parseInt(a.attributes.getNamedItem('data-order').value);
        var _b = parseInt(b.attributes.getNamedItem('data-order').value);
        if (_a > _b) return 1;
        else if (_a < _b) return -1;
        return 0;
    }
    itemArray.sort(compareText);
    for (var i = 0; i < itemArray.length; i++) {
        container.appendChild(container.removeChild(itemArray[i]));
    }
    $p.setData($target);
};

$p.setIncludeExportData = function ($control) {
    $('#SitePackagesSelectable')
        .find('.ui-selected')
        .each(function () {
            var $selected = $(this);
            if ($selected.attr('data-value') == undefined) {
                return true;
            }
            var data = $selected.attr('data-value').split('-');
            var type = $control.attr('id');
            $selected.attr('data-value', data[0] + '-' + (type === 'IncludeData'));
            $selected.find('span.include-data').remove();
            if (type === 'IncludeData') {
                $selected.append(
                    '<span class="include-data">(' + $p.display('IncludeData') + ')</span>'
                );
            }
        });
    $p.setData($('#SitePackagesSelectable'));
};
