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
            var tableNamePrefix = $('#TableName').val() + '_';
            var dataX = $(this).attr('data-x');
            var dataY = $(this).attr('data-y');
            data["KambanId"] = $(ui.draggable).attr('data-id');
            if (dataX !== undefined){
                data[tableNamePrefix + $('#KambanGroupByX').val()] = dataX;
            }
            if (dataY !== undefined) {
                data[tableNamePrefix + $('#KambanGroupByY').val()] = dataY;
            }
            $p.send($('#KambanBody'));
        }
    });
}