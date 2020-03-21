$p.uploadTenantImage = function ($control) {
    var data = new FormData();
    data.append('file', $('#TenantImage').prop('files')[0]);
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
}
