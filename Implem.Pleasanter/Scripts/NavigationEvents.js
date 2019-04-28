$(function () {
    $(window).on('popstate', function (e) {
        if (e.originalEvent.currentTarget.location.pathname !== $('#BaseUrl').val() + $('#Id').val()) {
            $p.ajax(e.originalEvent.currentTarget.location, 'post');
        }
    });
    $p.setSwitchTargets();
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        $control.appendTo('body');
    }
});