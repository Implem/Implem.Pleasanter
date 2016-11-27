$(function () {
    $(document).on('blur', '.control-markdown:not(.error)', function () {
        var $viewer = $('[id="' + this.id + '.viewer"]');
        $viewer.html($p.markup($(this).val()));
        $p.resizeEditor($(this), $viewer);
        $p.toggleEditor($viewer, false);
    });
    $(document).on('paste', '.upload-image', function (e) {
        if (e.originalEvent.clipboardData !== undefined) {
            var items = e.originalEvent.clipboardData.items;
            for (var i = 0 ; i < items.length ; i++) {
                var item = items[i];
                if (item.type.indexOf('image') !== -1) {
                    var data = new FormData();
                    data.append('Images_Bin', item.getAsFile());
                    requestFile('/images/create', 'post', data);
                }
            }
        }
    });
});