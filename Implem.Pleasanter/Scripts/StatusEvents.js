$(function () {
    $(document).on('change', '.control-dropdown', function () {
        var selectedCss = $(this).find('option:selected').attr('data-class');
        $(this).removeClass(function (index, css) {
            return (css.match(/\bstatus-\S+/g) || []).join(' ');
        });
        if (selectedCss !== undefined) {
            $(this).addClass(selectedCss);
        }
    });
});