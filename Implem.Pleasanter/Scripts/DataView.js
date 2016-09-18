$p.dataView = function ($control) {
    var formId = $p.getFormId($control);
    var url = $('#' + formId).attr('action').replace('_action_', $('#DataViewSelector').val());
    $p.ajax(url, 'post', $p.getData(), $control);
    history.pushState(null, null, url);
}