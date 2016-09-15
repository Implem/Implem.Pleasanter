$(function () {
    $(document).on('change', '#Search', function () {
        $p.search($(this).val(), $(this).hasClass('redirect'));
    });
    $(document).on('keydown', '#Search', function (e) {
        if (e.which === 13) $p.search($(this).val(), $(this).hasClass('redirect'));
    });
    if ($('#SearchResults').length === 1) {
        $(window).on('scroll resize', function () {
            var $control = $('#SearchResults')
            if ($control.length) {
                if ($(window).scrollTop() + $(window).height() >= $control.offset().top + $control.height()) {
                    if ($('#SearchOffset').val() !== '-1') {
                        $p.search($('#Search').val(), false, $('#SearchOffset').val());
                    }
                }
            }
        });
        $(document).on('click', '#SearchResults .result', function () {
            location.href = $(this).attr('data-href');
        });
    }
});