$(function () {
    $(document).on(
        'change',
        '.confirm-unload input, .confirm-unload select, .confirm-unload textarea,  .confirm-unload .control-spinner',
        function () {
            $p.setFormChanged($(this));
        });
    $(document).on(
        'spin',
        '.confirm-unload .control-spinner',
        function () {
            $p.setFormChanged($(this));
    });
    $(window).bind("beforeunload", function () {
        if ($p.formChanged) {
            return $p.display('ConfirmUnload');
        }
    });
});