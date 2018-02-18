$(function () {
    $(document).on('dblclick', '#Calendar .item', function () {
        location.href = $('#BaseUrl').val() + $(this).attr('data-id');
    });
    $(document).on('click', '#Calendar .item .ui-icon-pencil', function () {
        location.href = $('#BaseUrl').val() + $(this).parent().parent().attr('data-id');
    });
    $(document).on('mouseenter', '#Calendar .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').addClass('hover');
    });
    $(document).on('mouseleave', '#Calendar .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').removeClass('hover');
    });
    $(window).on('resize', function () {
        if ($('#Calendar').length === 1) {
            setTimeout(function () {
                $p.saveScroll();
                $p.setCalendar('#Grid');
                $p.loadScroll();
            }, 10);
        }
    });
});