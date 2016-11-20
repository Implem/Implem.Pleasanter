$(function () {
    $(document).on('click', '.control-basket > li > .delete', function () {
        var $control = $(this).closest('ol');
        $(this).parent().remove();
        $p.setData($control);
    });
});