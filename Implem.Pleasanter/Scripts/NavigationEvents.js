$(function () {
    $(window).on('popstate', function (e) {
        $p.ajax(e.originalEvent.currentTarget.location, 'post');
    });
    if ($('#SwitchTargets').length === 1) {
        $('#SwitchTargets').appendTo('body');
        $p.setCurrentIndex();
    }
    var $control = $('#BackUrl');
    if ($control.length === 1) {
        $control.appendTo('body');
    }
});