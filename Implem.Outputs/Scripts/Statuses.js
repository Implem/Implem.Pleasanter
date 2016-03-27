$(function () {
    $(document).on('change', '.status', function () {
        $(this).removeClass(function (index, css) {
            return (css.match(/\bstatus-\S+/g) || []).join(' ');
        });
        $(this).addClass($(this).find('option:selected').attr('data-class'));
    });
});