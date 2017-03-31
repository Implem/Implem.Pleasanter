$(function () {
    $(document).on(
        'change',
        'form.confirm-reload input, form.confirm-reload select, form.confirm-reload textarea',
        function () {
            $p.formChanged = true;
        });
    $(window).bind("beforeunload", function () {
        if ($p.formChanged) {
            return $p.display('ConfirmReload');
        }
    });
});