$p.viewMode = function ($control) {
    var url = $('.main-form').attr('action')
        .replace('_action_', $control.attr('data-action').toLowerCase());
    $p.ajax(url, 'post', $p.getData($control), $control);
    history.pushState(null, null, url);
    if ($p.responsive() && screen.width < 1025) {
        $p.openResponsiveMenu();
    }
}

$p.changeViewSelector = function ($control) {
    var action = $("option:selected", $control).attr('data-action');
    var url = action
        ? $('.main-form').attr('action').replace('_action_', action.toLowerCase())
        : location.href;
    var data = {
        ViewSelector: $control.val(),
        ControlId: $control.attr('id')
    };
    $p.ajax(url, 'post', data, $control);
    history.pushState(null, null, url);
}
