$p.uploadSiteImage = function ($control) {
    var data = new FormData();
    data.append('file', $('#SiteImage').prop('files')[0]);
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}

$p.openSiteSettingsDialog = function ($control, selector, width) {
    // ダイアログを表示する際に旧バージョンのデータが表示できるよう$p.ver()を渡す
    var data = $p.getData($control);
    data.Ver = $p.ver();
    var error = $p.syncSend($control);
    if (error === 0) {
        $(selector).dialog({
            modal: true,
            width: width !== undefined ? width : '90%',
            height: 'auto',
            appendTo: '#Editor',
            resizable: false
        });
    }
}

$p.openGridColumnDialog = function ($control) {
    $p.data.GridColumnForm = {};
    $p.openSiteSettingsDialog($control, '#GridColumnDialog');
}

$p.setGridColumn = function ($control) {
    $p.setData($('#UseGridDesign'));
    $p.setData($('#GridDesign'));
    $p.send($control);
}

$p.openFilterColumnDialog = function ($control) {
    $p.data.FilterColumnForm = {};
    $p.openSiteSettingsDialog($control, '#FilterColumnDialog');
}

$p.openAggregationDetailsDialog = function ($control) {
    $p.data.AggregationDetailsForm = {};
    $p.openSiteSettingsDialog($control, '#AggregationDetailsDialog', '420px');
}

$p.setAggregationDetails = function ($control) {
    $p.setData($('#AggregationType'));
    $p.setData($('#AggregationTarget'));
    $p.setData($('#SelectedAggregation'));
    $p.send($control);
}

$p.openEditorColumnDialog = function ($control) {
    $p.data.EditorColumnForm = {};
    $p.openSiteSettingsDialog($control, '#EditorColumnDialog');
}

$p.resetEditorColumn = function ($control) {
    $p.syncSend($control);
    var data = $p.getData($('#EditorColumnForm'));
    $('#EditorColumnForm [class^="control-"]').each(function (index, control) {
        $p.setData($(control), data);
    });
}

$p.openTabDialog = function ($control) {
    $p.data.TabForm = {};
    $p.openSiteSettingsDialog($control, '#TabDialog', '840px');
}

$p.openSummaryDialog = function ($control) {
    $p.data.SummaryForm = {};
    $p.openSiteSettingsDialog($control, '#SummaryDialog');
}

$p.setSummary = function ($control) {
    $p.setData($('#EditSummary'), $p.getData($control));
    $p.send($control);
}

$p.openFormulaDialog = function ($control) {
    $p.data.FormulaForm = {};
    $p.openSiteSettingsDialog($control, '#FormulaDialog');
}

$p.openProcessDialog = function ($control) {
    $p.data.ProcessForm = {};
    $p.openSiteSettingsDialog($control, '#ProcessDialog');
}

$p.openStatusControlDialog = function ($control) {
    $p.data.StatusControlForm = {};
    $p.openSiteSettingsDialog($control, '#StatusControlDialog');
    $('#StatusControlColumnHash li').each(function () {
        var $item = $(this);
        var type = $item.attr('data-value').split(',')[1];
        if (type !== 'None') {
            $item.append('<span class="column-control-types">(' + $p.display(type) + ')</span>');
        }
    });
}

$p.setStatusControlColumnHash = function ($control) {
    $('#StatusControlColumnHash').find('.ui-selected').each(function () {
        var $item = $(this);
        var columnName = $item.attr('data-value').split(',')[0];
        var type = $control.attr('data-type');
        $item.attr('data-value', columnName + ',' + type);
        $item.find('span.column-control-types').remove();
        if (type !== 'None') {
            $item.append('<span class="column-control-types">(' + $p.display(type) + ')</span>');
        }
    });
    $p.setData($('#SitePackagesSelectable'));
}

$p.openProcessValidateInputDialog = function ($control) {
    $p.data.ProcessValidateInputForm = {};
    $p.openSiteSettingsDialog($control, '#ProcessValidateInputDialog', '75%');
}

$p.openProcessDataChangeDialog = function ($control) {
    $p.data.ProcessDataChangeForm = {};
    $p.openSiteSettingsDialog($control, '#ProcessDataChangeDialog', '75%');
}

$p.openProcessNotificationDialog = function ($control) {
    $p.data.ProcessNotificationForm = {};
    $p.openSiteSettingsDialog($control, '#ProcessNotificationDialog', '75%');
}

$p.openViewDialog = function ($control) {
    $p.data.ViewForm = {};
    $p.openSiteSettingsDialog($control, '#ViewDialog');
}

$p.openNotificationDialog = function ($control) {
    $p.data.NotificationForm = {};
    $p.openSiteSettingsDialog($control, '#NotificationDialog');
}

$p.setNotification = function ($control) {
    $p.setData($('#EditNotification'), $p.getData($control));
    $p.send($control);
}

$p.openReminderDialog = function ($control) {
    $p.data.ReminderForm = {};
    $p.openSiteSettingsDialog($control, '#ReminderDialog');
}

$p.setReminder = function ($control) {
    $p.setData($('#EditReminder'), $p.getData($control));
    $p.send($control);
}

$p.openExportDialog = function ($control) {
    $p.data.ExportForm = {};
    $p.openSiteSettingsDialog($control, '#ExportDialog');
}

$p.setExport = function ($control) {
    $p.setData($('#EditExport'), $p.getData($control));
    $p.send($control);
}

$p.openExportColumnsDialog = function ($control) {
    $p.data.ExportColumnsForm = {};
    $p.openSiteSettingsDialog($control, '#ExportColumnsDialog', '420px');
}

$p.setExportColumn = function ($control) {
    $p.setData($('#EditExport'), $p.getData($control));
    $p.send($control);
}

$p.openScriptDialog = function ($control) {
    $p.data.ScriptForm = {};
    $p.openSiteSettingsDialog($control, '#ScriptDialog');
}

$p.setScript = function ($control) {
    $p.setData($('#EditScript'), $p.getData($control));
    $p.send($control);
}

$p.openHtmlDialog = function ($control) {
    $p.data.HtmlForm = {};
    $p.openSiteSettingsDialog($control, '#HtmlDialog');
}

$p.setHtml = function ($control) {
    $p.setData($('#EditHtml'), $p.getData($control));
    $p.send($control);
}

$p.openServerScriptDialog = function ($control) {
    $p.data.ServerScriptForm = {};
    $p.openSiteSettingsDialog($control, '#ServerScriptDialog');
}

$p.setServerScript = function ($control) {
    $p.setData($('#EditServerScript'), $p.getData($control));
    $p.send($control);
}

$p.openStyleDialog = function ($control) {
    $p.data.StyleForm = {};
    $p.openSiteSettingsDialog($control, '#StyleDialog');
}

$p.setStyle = function ($control) {
    $p.setData($('#EditStyle'), $p.getData($control));
    $p.send($control);
}

$p.addSummary = function ($control) {
    $p.setData($('#SummarySiteId'));
    $p.setData($('#SummaryDestinationColumn'));
    $p.setData($('#SummaryLinkColumn'));
    $p.setData($('#SummaryType'));
    $p.setData($('#SummarySourceColumn'));
    $p.send($control);
}

$p.openBulkUpdateColumnDialog = function ($control) {
    $p.data.BulkUpdateColumnForm = {};
    $p.openSiteSettingsDialog($control, '#BulkUpdateColumnDialog');
}

$p.setBulkUpdateColumn = function ($control) {
    $p.setData($('#EditBulkUpdateColumns'), $p.getData($control));
    $p.send($control);
}

$p.openBulkUpdateColumnDetailDialog = function ($control) {
    $p.data.BulkUpdateColumnDetailForm = {};
    $p.openSiteSettingsDialog($control, '#BulkUpdateColumnDetailDialog', '840px');
}

$p.setBulkUpdateColumnDetail = function ($control) {
    $p.setData($('#EditBulkUpdateColumnDetails'), $p.getData($control));
    $p.send($control);
}

$p.openRelatingColumnDialog = function ($control) {
    $p.data.RelatingColumnForm = {};
    $p.openSiteSettingsDialog($control, '#RelatingColumnDialog');
}

$p.setRelatingColumn = function ($control) {
    $p.setData($('#EditRelatingColumns'), $p.getData($control));
    $p.send($control);
}

$p.openDashboardPartDialog = function ($control) {
    $p.data.DashboardPartForm = {};
    $p.openSiteSettingsDialog($control, '#DashboardPartDialog');
}

$p.setDashboardPart = function ($control) {
    $p.setData($('#EditDashboardPart'), $p.getData($control));
    $p.send($control);
}

$p.openDashboardPartTimeLineSitesDialog = function ($control) {
    $p.data.TimeLineSitesForm = {};
    $p.openSiteSettingsDialog($control, '#DashboardPartTimeLineSitesDialog');
}

$p.openDashboardPartCalendarSitesDialog = function ($control) {
    $p.data.TimeLineSitesForm = {};
    $p.openSiteSettingsDialog($control, '#DashboardPartCalendarSitesDialog');
}

$p.openDashboardPartIndexSitesDialog = function ($control) {
    $p.data.TimeLineSitesForm = {};
    $p.openSiteSettingsDialog($control, '#DashboardPartIndexSitesDialog');
}

$p.updateDashboardPartTimeLineSites = function ($control) {
    $p.send($control);
}

$p.confirmTimeLineSites = function (value) {
    var args = JSON.parse(value);
    var result = confirm($p.display('ResetTimeLineView'));
    if (result) {
        $('#DashboardPartTimeLineSitesValue').text(args.timeLineSites);
        $p.set($('#DashboardPartTimeLineSites'), args.timeLineSites);
        $p.set($('#DashboardPartBaseSiteId'), args.baseSiteId);
        $p.send($("#ClearDashboardView"));
        $p.closeDialog($("#DashboardPartTimeLineSitesDialog"));
    }
}

$p.confirmCalendarSites = function (value) {
    var args = JSON.parse(value);
    var result = confirm($p.display('ResetCalendarView'));
    if (result) {
        $('#DashboardPartCalendarSitesValue').text(args.calendarSites);
        $p.set($('#DashboardPartCalendarSites'), args.calendarSites);
        $p.set($('#DashboardPartBaseSiteId'), args.baseSiteId);
        $p.send($("#ClearDashboardView"));
        $p.closeDialog($("#DashboardPartCalendarSitesDialog"));
    }
}

$p.confirmIndexSites = function (value) {
    var args = JSON.parse(value);
    var result = confirm($p.display('ResetIndexView'));
    if (result) {
        $('#DashboardPartIndexSitesValue').text(args.indexSites);
        $p.set($('#DashboardPartIndexSites'), args.indexSites);
        $p.set($('#DashboardPartBaseSiteId'), args.baseSiteId);
        $p.send($("#ClearDashboardIndexView"));
        $p.clearData('DashboardPartView', 'DashboardPartForm', 'startsWith');
        $p.closeDialog($("#DashboardPartIndexSitesDialog"));
    }
}