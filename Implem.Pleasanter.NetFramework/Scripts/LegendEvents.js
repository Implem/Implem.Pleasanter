$(function () {
    $(document).on('click', '[class*="enclosed"] .legend', function () {
        var icon = $(this).find('.ui-icon');
        if (icon.length === 1) {
            if ($(this).find('.ui-icon-triangle-1-s')[0]) {
                icon.removeClass('ui-icon-triangle-1-s');
                icon.addClass('ui-icon-triangle-1-e');
            }
            else {
                icon.removeClass('ui-icon-triangle-1-e');
                icon.addClass('ui-icon-triangle-1-s');
            }
            $(this).parent().children('div').toggle();
        }
    });
});