$(function () {
    $(window).on('load scroll resize', function () {
        if ($('#Grid').length) {
            if ($(window).scrollTop() + $(window).height() >= $('#Grid').offset().top + $('#Grid').height()) {
                if ($('#GridOffset').val() !== '-1') {
                    $p.setData($('#GridOffset'));
                    $('#GridOffset').val(-1);
                    $p.send($('#Grid'));
                }
            }
        }
    });
})