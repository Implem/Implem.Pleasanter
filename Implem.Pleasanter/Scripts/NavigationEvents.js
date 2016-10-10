$(function () {
    $(window).on('popstate', function (e) {
        $p.ajax(e.originalEvent.currentTarget.location, 'post');
    });
    $p.setSwitchTargets();
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        $control.appendTo('body');
    }
});