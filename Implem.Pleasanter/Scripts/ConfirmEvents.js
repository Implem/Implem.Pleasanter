$(function () {
    $(document).on(
        'change',
        'form.confirm-reload input, form.confirm-reload select, form.confirm-reload textarea',
        function () {
            $p.setFormChanged($(this));
        });
    $(window).bind("beforeunload", function () {
        if ($p.formChanged && $('#Editor').length === 1) {
            return $p.display('ConfirmReload');
        }
    });
});