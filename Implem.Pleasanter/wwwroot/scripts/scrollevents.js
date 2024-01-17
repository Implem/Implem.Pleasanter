$(function () {
    $(window).on('scroll resize', function () {
        $p.paging('#Grid');
    });
    $('.grid-stack-item-content').on('scroll resize', function () {
        var gridId = $(this).find('[id^="Grid_"]').attr('id');
        var target = $('.grid-stack-item-content').has('#' + gridId).get();
        $p.dashboardPaging('#' + gridId, target);
    });
});