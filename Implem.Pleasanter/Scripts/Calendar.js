$p.moveCalendar = function (type) {
    $control = $('#CalendarMonth');
    $control.val($('#Calendar' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCalendar = function () {
    if ($('#CalendarCanUpdate').val() === '1') {
        $('#Calendar .item').draggable({
            revert: 'invalid',
            start: function () {
                $(this).parent().droppable({
                    disabled: true
                });
            }
        });
        $('#Calendar .container').droppable({
            hoverClass: 'hover',
            tolerance: 'pointer',
            drop: function (e, ui) {
                var data = $p.getData($('.main-form'));
                data["CalendarId"] = $(ui.draggable).attr('data-id');
                var d = new Date($(this).attr('data-id'));
                var t = new Date($(ui.draggable).attr('data-value'));
                var v =
                    d.getFullYear() + '/' +
                    (d.getMonth() + 1) + '/' +
                    d.getDate() + ' ' +
                    t.getHours() + ':' +
                    t.getMinutes() + ':' +
                    t.getSeconds();
                data[$('#TableName').val() + '_' + $('#CalendarColumn').val()] = v;
                $p.send($('#CalendarBody'));
            }
        });
    }
}