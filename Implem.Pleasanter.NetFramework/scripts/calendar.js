$p.moveCalendar = function (type) {
    var $control = $('#CalendarDate');
    $control.val($('#Calendar' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCalendar = function () {
    $('#Calendar .container > div > div:not(.day)').remove();
    var data = JSON.parse($('#CalendarJson').val());
    data.forEach(function (element) {
        setCalendarGroup(element.group, element.items);
    });

    function setCalendarGroup(group, data) {
    var hash = {};
        var beginSelector = (group === undefined)
            ? '#Calendar .container:first'
            : '#Calendar .container[data-value="' + group + '"]:first';
        var endSelector = (group === undefined)
            ? '#Calendar .container:last'
            : '#Calendar .container[data-value="' + group + '"]:last';
        var begin = new Date($(beginSelector).attr('data-id'));
        var end = new Date($(endSelector).attr('data-id'));

    switch ($('#CalendarTimePeriod').val()) {
        case 'Yearly':
                setYearly(group, data, hash, begin, end);
            break;
        case 'Monthly':
            case 'Weekly':
                setMonthly(group, data, hash, begin, end);
            break;
    }
    }

    function setYearly(group, data, hash, begin, end) {
        data.forEach(function (element) {
            var current = $p.beginningMonth(new Date(element.From))
            if (current < begin) {
                current = new Date(begin);
            }
            rank = Rank(hash, $p.shortDateString(current));
            addItem(
                group,
                hash,
                element,
                current,
                undefined,
                undefined,
                1);
            if (element.To !== undefined) {
                current.setMonth(current.getMonth() + 1);
                var to = new Date(element.To);
                if (to > end) {
                    to = end;
                }
                while ($p.shortDate(to) >= $p.shortDate(current)) {
                    addItem(
                        group,
                        hash,
                        element,
                        current,
                        1,
                        rank);
                    current.setMonth(current.getMonth() + 1);
                }
            }
        });
    }

    function setMonthly(group, data, hash, begin, end) {
        data.forEach(function (element) {
            var current = new Date(element.From);
            if (current < begin) {
                current = new Date(begin);
            }
            rank = Rank(hash, $p.shortDateString(current));
            addItem(
                group,
                hash,
                element,
                current);
            if (element.To !== undefined) {
                current.setDate(current.getDate() + 1);
                var to = new Date(element.To);
                if (to > end) {
                    to = end;
                }
                while ($p.shortDate(to) >= $p.shortDate(current)) {
                    if (current.getDay() === 1) {
                        rank = Rank(hash, $p.shortDateString(current));
                    }
                    addItem(
                        group,
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
    }

    function Rank(hash, id) {
        if (hash[id] === undefined) {
            hash[id] = 0;
        }
        return hash[id];
    }

    function addItem(group, hash, element, current, sub, rank, yearly) {
        var id = $p.shortDateString(current);
        var groupSelector = (group === undefined)
            ? ''
            : '[data-value="' + group + '"]';
        var $cell = $(groupSelector + '[data-id="' + id + '"] > div');
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
                else if (element.To === undefined) {
                    return width - margin;
                }
                else if (yearly) {
                    var diff = 0;
                    var month = new Date(current);
                    month.setMonth(month.getMonth() + 1);
                    while (month <= new Date(element.To)) {
                        diff++;
                        month.setMonth(month.getMonth() + 1);
                    }
                    return (width * (diff + 1)) - margin;
                } else {
                    var diff = $p.dateDiff(
                        'd',
                        $p.shortDate(new Date(element.To)),
                        $p.shortDate(current));
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
};