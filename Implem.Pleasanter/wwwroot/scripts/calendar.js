const newRecord = function (info) {
    var form = document.createElement("form");
    form.setAttribute("action", $('#NewMenuContainer div a').attr('href'));
    form.setAttribute("method", "post");
    form.style.display = "none";
    document.body.appendChild(form);
    var start = document.createElement("input");
    start.setAttribute("type", "hidden");
    start.setAttribute("name", "Issues_StartTime");
    start.setAttribute("value", info.start.toLocaleString());
    form.appendChild(start);
    var end = document.createElement("input");
    end.setAttribute("type", "hidden");
    end.setAttribute("name", "Issues_CompletionTime");
    end.setAttribute("value", info.end.toLocaleString());
    form.appendChild(end);
    var fromTo = $('#CalendarFromTo').val().split('-');

    const match = /^Date/;
    if (fromTo[1]) {
    } else if (match.test(fromTo)) {
        var from = document.createElement("input");
        from.setAttribute("type", "hidden");
        from.setAttribute("name", "Issues_" + fromTo);
        from.setAttribute("value", info.start.toLocaleString());
        form.appendChild(from);
    } else {
        return;
    }
    form.submit();
}
const updateRecord = function (info) {
    var data = $p.getData($('.main-form'));
    var fromTo = $('#CalendarFromTo').val().split('-');
    var prefix = $('#TableName').val() + '_';
    data.Id = info.event.id;
    data[prefix + fromTo[0]] = info.event.start.toLocaleString();
    if (info.event.end === null) {
        data[prefix + fromTo[1]] = info.event.start.toLocaleString();
    } else {
        data[prefix + fromTo[1]] = info.event.end.toLocaleString();
    }
    $p.saveScroll();
    $p.send($('#FullCalendarBody'));
}
const getEventsDatas = function (info, successCallback, failureCallback) {

    if (($('#IsInit').val() !== 'True') && !((info.start.getTime() == Date.parse($('#CalendarStart').val()) && (info.end.getTime() == Date.parse($('#CalendarEnd').val()))))) {
        $p.set($('#CalendarStart'), info.start.toLocaleDateString());
        $p.set($('#CalendarEnd'), info.end.toLocaleDateString());
        $('#FullCalendarBody').attr('data-action', 'calendar');

        let calendarDiff = Math.round((info.end - info.start) / (1000 * 60 * 60 * 24));
        if (calendarDiff === 1) {
            $p.set($('#CalendarViewType'), 'timeGridDay');
        } else if (calendarDiff === 7) {
            $p.set($('#CalendarViewType'), 'timeGridWeek');
        } else if (calendarDiff < 32) {
            $p.set($('#CalendarViewType'), 'listMonth');
        } else {
            $p.set($('#CalendarViewType'), 'dayGridMonth');
        }

        let $control = $('#FullCalendarBody');
        $p.send($control, undefined, true, false, function () { },
            function () {
                $p.setMessage(
                    "#Message",
                    JSON.stringify({ Css: "alert-error", Text: "データの取得に失敗しました。" })
                );
            }
        );
    } else {
        $('#IsInit').val('False');
        let eventData = JSON.parse($('#CalendarJson').val())[0]['items'];
        successCallback(
            eventData.map((item) => {
                if (item.StatusHtml) {
                    return {
                        id: item.id,
                        title: item.title,
                        start: item.start,
                        end: item.end,
                        StatusHtml: item.StatusHtml
                    }
                }
                else {
                    return {
                        id: item.id,
                        title: item.title,
                        start: item.start,
                        end: item.end,
                    }
                }
            }))
    }

}
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
        .attr('title', htmlEncode(element.Title) + ' -- ' +
            $p.dateTimeString(new Date(element.From)) +
            (element.To !== undefined && element.To !== element.From
                ? ' - ' + $p.dateTimeString(new Date(element.To))
                : ''))
        .append($('<span />').addClass('ui-icon ui-icon-pencil'))
        .append((element.Time !== undefined
            ? element.Time + ' '
            : '')
            + (element.StatusHtml
                ? element.StatusHtml
                : '')
            + htmlEncode(element.Title)));
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

function htmlEncode(str) {
    return String(str)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;");
}
$p.moveCalendar = function (type) {
    var $control = $('#CalendarDate');
    $control.val($('#Calendar' + type).val());
    $p.setData($control);
    $p.send($control);
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
$p.setCalendar = function () {
    if ($('#CalendarType').val() == "FullCalendar") {

        $('#FullCalendar').css('clear', 'both');
        var calendarEl = document.getElementById('FullCalendar');
        let calendarMiddle = new Date();
        if ($("#CalendarStart").val() !== '') {
            calendarMiddle = new Date((new Date($("#CalendarStart").val()).getTime() + new Date($("#CalendarEnd").val()).getTime()) / 2);
        }
        var language = $('#Language').val();
        var supportedLanguages = ['en', 'zh', 'ja', 'de', 'ko', 'es', 'vi'];
        if (language === 'vn') {
            language = 'vi';
        }
        $p.fullCalendar = new FullCalendar.Calendar(calendarEl, {
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth'
            },
            firstDay: 1,
            initialDate: calendarMiddle,
            selectable: true,
            navLinks: true,
            businessHours: true,
            editable: true,
            height: "auto",
            locale: supportedLanguages.includes(language) ? language : 'en',
            selectMirror: true,
            eventClick: (e) => {
                window.location.href = '/items/' + e.event.id + '/edit';
            },
            select: newRecord,
            events: getEventsDatas,
            eventDrop: updateRecord,
            eventResize: updateRecord,
            eventDidMount: function (info) {
                if (info.event.extendedProps.StatusHtml) {
                    if (info.view.type === 'listMonth') {
                        var eventElement = $(info.el).find('.fc-list-event-graphic');
                        eventElement.append($.parseHTML(info.event.extendedProps.StatusHtml)[0]);
                        $(".fc-list-event-dot").css('margin-right', '20px');
                    } else {
                        var eventElement = $(info.el).find('.fc-event-time');
                        eventElement.prepend($.parseHTML(info.event.extendedProps.StatusHtml)[0]);
                    }
                    $('.status-new').css('color', 'black');
                    $('.status-review').css('color', 'black');
                    $('.status-new').css('border', 'solid 1px #000');
                    $("[class^='status']").css('padding', '1px 3px');
                    $("[class^='status']").css('margin', '0px 3px');
                    $("[class^='status']").css('width', '15px');
                }
            },
            initialView: $('#CalendarViewType').val(),
            lazyFetching: false
        });
        $p.fullCalendar.render();
    } else {
        $('#Calendar .container > div > div:not(.day)').remove();
        var data = JSON.parse($('#CalendarJson').val());
        data.forEach(function (element) {
            setCalendarGroup(element.group, element.items);
        });

    }

}
