$p.uploadTenantImage = function ($control) {
    var data = new FormData();
    data.append('file', $('#TenantImage').prop('files')[0]);
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control
    );
};

$p.openServerScriptScheduleDialog = function ($control) {
    $p.data.ServerScriptScheduleForm = {};
    $p.openSiteSettingsDialog($control, '#ServerScriptScheduleDialog');
    // JQueryUIのui-multiselect-menuのz-indexが固定値の為に書き換える。
    $('div.ui-multiselect-menu').css('z-index', 110);
};

$p.openScimTokenDialog = function ($control) {
    $p.data.ScimTokenForm = {};
    $(document)
        .off('dialogopen.scimToken', '#ScimTokenDialog')
        .one('dialogopen.scimToken', '#ScimTokenDialog', function () {
            setTimeout(function () {
                var $dialog = $('#ScimTokenDialog');
                var activeElement = document.activeElement;
                var $expiresTime = $dialog.find('#ScimTokenExpiresTime');

                $expiresTime.each(function () {
                    if (this._flatpickr) {
                        this._flatpickr.close();
                    }
                });
                if (
                    activeElement &&
                    activeElement !== document.body &&
                    $dialog.closest('.ui-dialog').has(activeElement).length === 1
                ) {
                    activeElement.blur();
                }
            });
        });
    $p.openSiteSettingsDialog($control, '#ScimTokenDialog', '720px');
};

$p.setScimToken = function ($control) {
    $p.setData($('#EditScimToken'), $p.getData($control));
    $p.send($control);
};
