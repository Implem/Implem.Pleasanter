$(function () {
    $(document).on('blur', '.control-markdown:not(.error)', function () {
        var $viewer = $('[id="' + this.id + '.viewer"]');
        $viewer.html($p.markup($(this).val()));
        $p.resizeEditor($(this), $viewer);
        $p.toggleEditor($viewer, false);
        $p.formChanged = true;
    });
    $(document).on('paste', '.upload-image', function (e) {
        if (e.originalEvent.clipboardData !== undefined) {
            var items = e.originalEvent.clipboardData.items;
            for (var i = 0 ; i < items.length ; i++) {
                var item = items[i];
                if (item.type.indexOf('image') !== -1) {
                    var url = $('.main-form')
                        .attr('action')
                        .replace('_action_', 'binaries/uploadimage');
                    var data = new FormData();
                    data.append('ControlId', this.id);
                    data.append('file', item.getAsFile());
                    $p.multiUpload(url, data);
                }
            }
        }
    });
});