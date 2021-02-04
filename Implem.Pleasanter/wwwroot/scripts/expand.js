$(function () {
    $('.field-section.expand').click(function () {
        var $section = $('#' + $(this).attr('for'));
        $section.toggle();
        if ($section.css('display') === 'none') {
            $(this).find('span').removeClass('ui-icon-triangle-1-s');
            $(this).find('span').addClass('ui-icon-triangle-1-e');
        } else {
            $(this).find('span').removeClass('ui-icon-triangle-1-e');
            $(this).find('span').addClass('ui-icon-triangle-1-s');
        }
    });
});