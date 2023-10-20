$p.changeCalculationMethod = function ($control) {
    var action = $("option:selected", $control).attr('data-action');
    var url = action
        ? $('.main-form').attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    var data = {
        CalculationMethod: $control.val(),
        ControlId: $control.attr('id')
    };
    $p.ajax(url, 'post', data, $control);
    history.pushState(null, null, url);
}
