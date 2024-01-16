$p.setKamban = function (suffix) {
    var suffixElArr = $('#MainForm').find('div[id^="KambanBody"].kambanbody').get();
    if (suffix) {
        suffixElArr = $('#MainForm').find('div[id="KambanBody_' + suffix + '"].kambanbody').get();
    }
    $('.kambanbody').addClass('no-drag');
    $('#KambanValueField').toggle($('#KambanAggregateType').val() !== 'Count');
    $(suffixElArr).each(function (index, value) {
        var kambanSuffix = value.id.substring(value.id.indexOf('_'));
        kambanSuffix = kambanSuffix.indexOf('_') === -1 ? '' : kambanSuffix;
        $('#KambanBody' + kambanSuffix + ' .ui-widget-header').addClass('dashboard-kamban-header');
        $('#KambanBody' + kambanSuffix + ' .kamban-item').draggable({
            revert: 'invalid',
            start: function () {
                $(this).parent().droppable({
                    disabled: true
                });
            },
            helper: function () {
                return $('<div />')
                    .addClass('dragging')
                    .append($('<div />')
                        .append($(this).text())).width($(this).width() + 10).height($(this).height() + 10);
            }
        });
        $('#KambanBody' + kambanSuffix + ' .kamban-container').droppable({
            hoverClass: 'hover',
            tolerance: 'intersect',
            drop: function (e, ui) {
                $p.clearData();
                var control = ui.draggable;
                var data = $p.getData($('.main-form'));
                var tableNamePrefix = $('#KambanReferenceType' + kambanSuffix).val() + '_';
                var dataX = $(this).attr('data-x');
                var dataY = $(this).attr('data-y');
                data["KambanId"] = $(control).attr('data-id');
                if (dataX !== undefined) {
                    data[tableNamePrefix + $('#KambanGroupByX' + kambanSuffix).val()] = dataX;
                }
                if (dataY !== undefined) {
                    data[tableNamePrefix + $('#KambanGroupByY' + kambanSuffix).val()] = dataY;
                }
                if (kambanSuffix !== '') {
                    data.SiteId = control.attr('data-siteid');
                }
                $p.set($('#KambanSuffix' + kambanSuffix), $('#KambanSuffix' + kambanSuffix).val());
                $p.send($('#KambanBody' + kambanSuffix));
            }
        })
    });
    $(suffixElArr).each(function (index, value) {
        $(this).find('.kamban-item').each(function () {
        let offsetX, offsetY,kambanSuffix,siteId;
        $(this).on('touchstart', function (e) {
            kambanSuffix = $(this).parents('[id^=Kamban]').attr('id').substring($(this).parents('[id^=Kamban]').attr('id').indexOf('_'));
            kambanSuffix = kambanSuffix.indexOf('_') === -1 ? '' : kambanSuffix;
            const touch = e.touches[0];
            offsetX = touch.clientX;
            offsetY = touch.clientY;
            siteId = $(this).attr('data-siteid');
        });
        $(this).on('touchmove', function (e) {
            e.preventDefault();
            const touch = e.touches[0];
            const x = touch.clientX - offsetX;
            const y = touch.clientY - offsetY;
            const rectDraggable = this.getBoundingClientRect();
            $(this).css('z-index', 2);
            $('.kambanbody .kamban-container').each(function () {
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
            $('#KambanBody' + kambanSuffix + ' .kamban-container').each(function () {
                const rectDroppable = this.getBoundingClientRect();
                if (
                    rectDraggable.left >= rectDroppable.left &&
                    rectDraggable.left <= rectDroppable.right &&
                    rectDraggable.top >= rectDroppable.top &&
                    rectDraggable.top <= rectDroppable.bottom
                ) {
                    var data = $p.getData($('.main-form'));
                    var tableNamePrefix = $('#KambanReferenceType' + kambanSuffix).val() + '_';
                    var dataX = $(this).attr('data-x');
                    var dataY = $(this).attr('data-y');
                    data["KambanId"] = id;
                    if (dataX !== undefined) {
                        data[tableNamePrefix + $('#KambanGroupByX' + kambanSuffix).val()] = dataX;
                    }
                    if (dataY !== undefined) {
                        data[tableNamePrefix + $('#KambanGroupByY' + kambanSuffix).val()] = dataY;
                    }
                    if (kambanSuffix !== '') {
                        data.SiteId = siteId;
                    }
                    $p.set($('#KambanSuffix' + kambanSuffix), $('#KambanSuffix' + kambanSuffix).val());
                    $p.send($('#KambanBody' + kambanSuffix));
                    $p.clearData('KambanSuffix' + kambanSuffix);
                    isDroppableIntoKambanContainer = true;
                }
            });
            if (!isDroppableIntoKambanContainer) {
                $(this).css('transform', 'unset');
            }
        });
    });

    })
}