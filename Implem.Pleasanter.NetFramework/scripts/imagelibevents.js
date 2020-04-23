$(function () {
    $(window).on('scroll resize', function () {
        if ($('#ImageLib').length === 1) {
            $p.paging('#ImageLib');
        }
    });
});