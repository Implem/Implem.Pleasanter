$(function () {
    $(document).on(
        'change',
        '.confirm-unload input, .confirm-unload select, .confirm-unload textarea',
        function () {
            $p.setFormChanged($(this));
        });
    $(window).bind("beforeunload", function () {
        if ($p.formChanged) {
            return $p.display('ConfirmUnload');
        }
    });
});