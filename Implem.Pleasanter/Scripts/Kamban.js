$p.setKamban = function () {
    $('#KambanBody .kamban-item').draggable({
        revert: 'invalid',
        start: function () {
            $(this).parent().droppable({
                disabled: true
            });
        }
    });
    $('#KambanBody .kamban-container').droppable({
        hoverClass: 'hover',
        tolerance: 'pointer',
        drop: function (e, ui) {
            var data = $p.getData($('.main-form'));
            data["KambanId"] = $(ui.draggable).attr('data-id');
            data[$('#TableName').val() + '_' + $('#KambanGroupBy').val()] =
                $(this).attr('data-value') !== undefined
                    ? $(this).attr('data-value')
                    : '';
            $p.send($('#KambanBody'));
        }
    });
}