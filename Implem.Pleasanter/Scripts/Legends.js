$(function () {
    $(document).on('click', '.legend', function () {
        if ($(this).find('.ui-icon')[0]) {
            if ($(this).find('.ui-icon-carat-1-s')[0]) {
                $(this).find('.ui-icon').addClass('ui-icon-carat-1-e');
                $(this).find('.ui-icon').removeClass('ui-icon-carat-1-s');
            }
            else {
                $(this).find('.ui-icon').addClass('ui-icon-carat-1-s');
                $(this).find('.ui-icon').removeClass('ui-icon-carat-1-e');
            }
            $(this).parent().children('div').toggle();
        }
    });
});