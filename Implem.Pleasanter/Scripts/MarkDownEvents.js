$(function () {
    $(document).on('blur', '.control-markdown:not(.error)', function () {
        $p.showMarkDownViewer($(this));
    });
    $(document).on('paste', '.upload-image', function (e) {
        if (e.originalEvent.clipboardData !== undefined) {
            var items = e.originalEvent.clipboardData.items;
            if (items.length > 0) {
                var item = items[0];
                if (item.type.indexOf('image') !== -1) {
                    $p.uploadImage(this.id, item.getAsFile());
                }
            }
        }
    });
    $(document).on('change', '.upload-image-file', function () {
        if (this.files.length === 1) {
            $p.uploadImage($(this).attr('data-id'), this.files[0]);
            this.value = '';
        }
    });
});