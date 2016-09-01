$p.setKamban = function () {
    $('.kamban-item').draggable({
        revert: 'invalid',
        start: function () {
            $(this).parent().droppable({
                disabled: true
            });
        }
    });
    $('.kamban-container').droppable({
        hoverClass: 'hover',
        tolerance: 'pointer',
        drop: function (e, ui) {
            var data = $p.getData();
            data["KambanId"] = $(ui.draggable).attr('data-id');
            data[$('#TableName').val() + '_' + $('#KambanGroupByColumn').val()] =
                $(this).attr('data-value') !== undefined
                    ? $(this).attr('data-value')
                    : '';
            $p.send($('#KambanBody'));
        }
    });
}