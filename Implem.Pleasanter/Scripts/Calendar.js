$p.moveCalendar = function (type) {
    var $control = $('#CalendarMonth');
    $control.val($('#Calendar' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCalendar = function () {
    var data = JSON.parse($('#CalendarJson').val());
    var hash = {};
    $('#Calendar .container > div > div:not(.day)').remove();
    data.forEach(function (element) {
        var begin = new Date($('#Calendar .container:first').attr('data-id'));
        var end = new Date($('#Calendar .container:last').attr('data-id'));
        var current = new Date(element.From) > begin
            ? new Date(element.From)
            : begin;
        rank = Rank(hash, $p.shortDateString(current));
        addItem(hash, element, current);
        if (element.To !== undefined) {
            current.setDate(current.getDate() + 1);
            var to = new Date(element.To) < end
                ? new Date(element.To)
                : end;
            while (to >= current) {
                if (current.getDay() === 1) {
                    rank = Rank(hash, $p.shortDateString(current));
                }
                addItem(
                    hash,
                    element,
                    current,
                    current.getDay() !== 1,
                    rank);
                current.setDate(current.getDate() + 1);
            }
        }
    });

    if ($('#CalendarCanUpdate').val() === '1') {
        $('#Calendar .item').draggable({
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
                        .append($(this).text()));
            }
        });
        $('#Calendar .container').droppable({
            hoverClass: 'hover',
            tolerance: 'intersect',
            drop: function (e, ui) {
                var $control = $(ui.draggable);
                var from = new Date($control.attr('data-from'));
                var target = new Date($(this).attr('data-id'));
                var data = $p.getData($('.main-form'));
                var fromTo = $('#CalendarFromTo').val().split('-');
                var prefix = $('#TableName').val() + '_';
                data.Id = $control.attr('data-id');
                data[prefix + fromTo[0]] = margeTime(target, from);
                if ($control.attr('data-to') !== undefined) {
                    var diff = $p.dateDiff('d', target, $p.shortDate(from));
                    var to = $p.dateAdd('d', diff, new Date($control.attr('data-to')));
                    data[prefix + fromTo[1]] = margeTime(to);
                }
                $p.saveScroll();
                $p.send($('#CalendarBody'));
            }
        });
    }

    function Rank(hash, id) {
        if (hash[id] === undefined) {
            hash[id] = 0;
        }
        return hash[id];
    }

    function addItem(hash, element, current, sub, rank) {
        var id = $p.shortDateString(current);
        var $cell = $('[data-id="' + id + '"] > div');
        while (Rank(hash, id) < rank) {
            $cell.append($('<div />').addClass('dummy'));
            hash[id]++;
        }
        var item = $('<div />')
            .addClass('item')
            .addClass(element.Changed === true ? 'changed' : '')
            .attr('data-id', element.Id)
            .attr('data-from', element.From)
            .attr('data-to', element.To);
        if (sub) {
            item.append($('<div />')
                .attr('data-id', element.Id)
                .addClass('connection')
                .addClass(element.Changed === true
                    ? 'changed'
                    : ''));
        }
        item.append($('<div />')
            .addClass('title')
            .css('width', function () {
                var width = $cell.parent().width();
                var margin = 16;
                if (sub) {
                    return '';
                }
                if (element.To === undefined) {
                    return width - margin;
                } else {
                    var diff = $p.dateDiff('d', new Date(element.To), current);
                    var col = current.getDay() !== 0
                        ? current.getDay()
                        : 7;
                    if (col + diff > 6) {
                        diff = (7 - col);
                    } else if (diff < 0) {
                        diff = 0;
                    }
                    return (width * (diff + 1)) - margin;
                }
            })
            .addClass(sub ? 'sub' : '')
            .attr('title', element.Title + ' -- ' +
                $p.dateTimeString(new Date(element.From)) +
                    (element.To !== undefined && element.To !== element.From
                        ? ' - ' + $p.dateTimeString(new Date(element.To))
                        : ''))
            .append($('<span />').addClass('ui-icon ui-icon-pencil'))
            .append((element.Time !== undefined
                ? element.Time + ' '
                : '') +
                    element.Title));
        $cell.append(item);
        hash[id]++;
    }

    function margeTime(date, dateTime) {
        if (dateTime === undefined) dateTime = date;
        return date.getFullYear() + '/' +
            (date.getMonth() + 1) + '/' +
            date.getDate() + ' ' +
            dateTime.getHours() + ':' +
            dateTime.getMinutes() + ':' +
            dateTime.getSeconds();
    }
}