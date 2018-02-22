$(function () {
    $(document).on('blur', '.control-markdown:not(.error)', function () {
        $p.showMarkDownViewer($(this));
    });
    $(document).on('paste', '.upload-image', function (e) {
        if (e.originalEvent.clipboardData !== undefined &&
            e.originalEvent.clipboardData.types.indexOf('text/plain') === -1) {
            var items = e.originalEvent.clipboardData.items;
            for (var i = 0 ; i < items.length ; i++) {
                var item = items[i];
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