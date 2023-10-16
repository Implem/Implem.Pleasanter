$p.setKamban = function () {
    $('#KambanValueField').toggle($('#KambanAggregateType').val() !== 'Count');
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
        tolerance: 'intersect',
        drop: function (e, ui) {
            var data = $p.getData($('.main-form'));
            var tableNamePrefix = $('#TableName').val() + '_';
            var dataX = $(this).attr('data-x');
            var dataY = $(this).attr('data-y');
            data["KambanId"] = $(ui.draggable).attr('data-id');
            if (dataX !== undefined) {
                data[tableNamePrefix + $('#KambanGroupByX').val()] = dataX;
            }
            if (dataY !== undefined) {
                data[tableNamePrefix + $('#KambanGroupByY').val()] = dataY;
            }
            $p.send($('#KambanBody'));
        }
    });
    $('#KambanBody .kamban-item').each(function () {
        let offsetX, offsetY;
        $(this).on('touchstart', function (e) {
            const touch = e.touches[0];
            offsetX = touch.clientX;
            offsetY = touch.clientY;
        });
        $(this).on('touchmove', function (e) {
            e.preventDefault();
            const touch = e.touches[0];
            const x = touch.clientX - offsetX;
            const y = touch.clientY - offsetY;
            const rectDraggable = this.getBoundingClientRect();
            $(this).css('z-index', 2);
            $('#KambanBody .kamban-container').each(function () {
                const rectDroppable = this.getBoundingClientRect();
                if (
                    rectDraggable.left >= rectDroppable.left &&
                    rectDraggable.left <= rectDroppable.right &&
                    rectDraggable.top >= rectDroppable.top &&
                    rectDraggable.top <= rectDroppable.bottom
                ) {
                    $(this).css('background-color', '#f5f5f5');
                } else {
                    $(this).css('background-color', 'unset');
                }
            });
            $(this).css('transform', `translate(${x}px, ${y}px)`);
        });
        $(this).on('touchend', function () {
            const rectDraggable = this.getBoundingClientRect();
            const id = $(this).attr('data-id');
            let isDroppableIntoKambanContainer = false;
            $('#KambanBody .kamban-container').each(function () {
                const rectDroppable = this.getBoundingClientRect();
                if (
                    rectDraggable.left >= rectDroppable.left &&
                    rectDraggable.left <= rectDroppable.right &&
                    rectDraggable.top >= rectDroppable.top &&
                    rectDraggable.top <= rectDroppable.bottom
                ) {
                    var data = $p.getData($('.main-form'));
                    var tableNamePrefix = $('#TableName').val() + '_';
                    var dataX = $(this).attr('data-x');
                    var dataY = $(this).attr('data-y');
                    data["KambanId"] = id;
                    if (dataX !== undefined) {
                        data[tableNamePrefix + $('#KambanGroupByX').val()] = dataX;
                    }
                    if (dataY !== undefined) {
                        data[tableNamePrefix + $('#KambanGroupByY').val()] = dataY;
                    }
                    $p.send($('#KambanBody'));
                    isDroppableIntoKambanContainer = true;
                }
            });
            if (!isDroppableIntoKambanContainer) {
                $(this).css('transform', 'unset');
            }
        });
    });
}