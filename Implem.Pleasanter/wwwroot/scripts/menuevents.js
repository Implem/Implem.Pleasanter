$(function () {
    $(document).on('mouseenter', '#NavigationMenu > li', function () {
        var $container = $(this).find(':first-child');
        $container.addClass('hover');
        $('#' + $container.attr('data-id')).show();
    });
    $(document).on('mouseleave', '#NavigationMenu > li', function () {
        var $container = $(this).find(':first-child');
        $container.removeClass('hover');
        $('#' + $container.attr('data-id')).hide();
    });
});

$p.expandSideMenu = function () {
    $('#hamburger').prop('checked', true);
}

$p.closeSideMenu = function () {
    $('.menubox > input[class="toggle"]').prop('checked', false);
}