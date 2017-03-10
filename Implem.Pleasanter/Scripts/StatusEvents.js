$(function () {
    $(document).on('change', '.control-dropdown', function () {
        var selectedCss = $(this).find('option:selected').attr('data-class');
        if (selectedCss !== undefined) {
            $(this).removeClass(function (index, css) {
                return (css.match(/\bstatus-\S+/g) || []).join(' ');
            });
            $(this).addClass(selectedCss);
        }
    });
});