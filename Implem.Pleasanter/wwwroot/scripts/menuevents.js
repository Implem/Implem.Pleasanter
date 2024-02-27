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

$p.closeSideMenu = function ($control) {
    var $checkbox = $('.menubox > input[class="toggle"]');
    $checkbox.prop('checked', false);
}

$p.expandSideMenu = function ($control) {
    var $hamburgerCheckbox = $('#hamburger');
    $hamburgerCheckbox.prop('checked', true);
}