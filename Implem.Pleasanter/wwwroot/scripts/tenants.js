$p.uploadTenantImage = function ($control) {
    var data = new FormData();
    data.append('file', $('#TenantImage').prop('files')[0]);
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}

$p.openServerScriptScheduleDialog = function ($control) {
    $p.data.ServerScriptScheduleForm = {};
    $p.openSiteSettingsDialog($control, '#ServerScriptScheduleDialog');
    $('div.ui-multiselect-menu').css('z-index', 110); // JQueryUIのui-multiselect-menuのz-indexが固定値の為に書き換える。
}
