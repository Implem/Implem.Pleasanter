const newRecord = function (calendarSuffix) {
    return function (info) {
        var endDate = new Date(info.end);
        if (($('#CalendarEditorFormat' + calendarSuffix).val() === 'Ymd') && endDateFormat(endDate)) {
            endDate.setDate(endDate.getDate() - 1);
        }
        var form = document.createElement('form');
        form.setAttribute('action', $('#ApplicationPath').val() + 'items/' + $('#CalendarSiteData' + calendarSuffix).val() + '/new');
        form.setAttribute('method', 'post');
        form.style.display = 'none';
        document.body.appendChild(form);
        var start = document.createElement('input');
        start.setAttribute('type', 'hidden');
        start.setAttribute('name', 'PostInit_' + $('#CalendarReferenceType' + calendarSuffix).val() + '_StartTime');
        start.setAttribute('value', info.start.toLocaleString());
        form.appendChild(start);
        var end = document.createElement('input');
        end.setAttribute('type', 'hidden');
        end.setAttribute('name', 'PostInit_' + $('#CalendarReferenceType' + calendarSuffix).val() + '_CompletionTime');
        end.setAttribute('value', endDate.toLocaleString());
        form.appendChild(end);
        var fromTo = $('#CalendarFromTo' + calendarSuffix).val().split('-');
        const match = /^Date/;
        if (fromTo[1]) {
        } else if (match.test(fromTo)) {
            var from = document.createElement('input');
            from.setAttribute('type', 'hidden');
            from.setAttribute('name', 'PostInit_' + $('#CalendarReferenceType' + calendarSuffix).val() + '_' + fromTo);
            from.setAttribute('value', info.start.toLocaleString());
            form.appendChild(from);
        } else {
            return;
        }
        if ($('#Token').length) {
            var input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('name', 'Token');
            input.setAttribute('value', $('#Token').val());
            form.appendChild(input);
        }
        form.submit();
    }
}

function endDateFormat(endDate) {
    return endDate.getHours() === 0
        && endDate.getMinutes() === 0
        && endDate.getSeconds() === 0;
}
const updateRecord = function (calendarSuffix) {
    return function (info, successCallback, failureCallback) {
        $p.clearData();
        var data = $p.getData($('.main-form'));
        var fromTo = $('#CalendarFromTo' + calendarSuffix).val().split('-');
        var prefix = $('#CalendarReferenceType' + calendarSuffix).val() + '_';
        if (calendarSuffix !== '') {
            $p.set($('#CalendarSuffix' + calendarSuffix), $('#CalendarSuffix' + calendarSuffix).val());
            data.SiteId = info.event.extendedProps.siteId;
            data.Id = $('#Id').val();
            data['EventId'] = info.event.id;
            $p.set($('#CalendarStart' + calendarSuffix), $('#CalendarStart' + calendarSuffix).val());
            $p.set($('#CalendarEnd' + calendarSuffix), $('#CalendarEnd' + calendarSuffix).val());
            $p.set($('#CalendarViewType' + calendarSuffix), $('#CalendarViewType' + calendarSuffix).val());
        } else {
            data.Id = info.event.id;
        }
        data[prefix + fromTo[0]] = info.event.start.toLocaleString();
        if (info.event.end === null) {
            data[prefix + fromTo[1]] = info.event.start.toLocaleString();
        } else {
            var endDate = new Date(info.event.end);
            if ($('#CalendarEditorFormat' + calendarSuffix).val() === 'Ymd') {
                endDate.setDate(endDate.getDate() - 1);
            }
            data[prefix + fromTo[1]] = endDate.toLocaleString();
        }
        $p.saveScroll();
        $p.send($('#FullCalendarBody' + calendarSuffix));
    }

}
const getEventsDatas = function (calendarSuffix) {
    return function (info, successCallback, failureCallback) {
        if (($('#IsInit' + calendarSuffix).val() !== 'True') && !((info.start.getTime() == Date.parse($('#CalendarStart' + calendarSuffix).val()) && (info.end.getTime() == Date.parse($('#CalendarEnd' + calendarSuffix).val()))))) {
            $p.clearData();
            $p.set($('#CalendarStart' + calendarSuffix), info.start.toLocaleDateString());
            $p.set($('#CalendarEnd' + calendarSuffix), info.end.toLocaleDateString());
            $('#FullCalendarBody' + calendarSuffix).attr('data-action', 'calendar');
            if (calendarSuffix !== '') {
                $p.set($('#CalendarSuffix' + calendarSuffix), $('#CalendarSuffix' + calendarSuffix).val());
            }

            let calendarDiff = Math.round((info.end - info.start) / (1000 * 60 * 60 * 24));
            if (calendarDiff === 1) {
                $p.set($('#CalendarViewType' + calendarSuffix), 'timeGridDay');
            } else if (calendarDiff === 7) {
                $p.set($('#CalendarViewType' + calendarSuffix), 'timeGridWeek');
            } else if (calendarDiff < 32) {
                $p.set($('#CalendarViewType' + calendarSuffix), 'listMonth');
            } else {
                $p.set($('#CalendarViewType' + calendarSuffix), 'dayGridMonth');
            }

            let $control = $('#FullCalendarBody' + calendarSuffix);
            $p.send($control);
        } else {
            $('#IsInit' + calendarSuffix).val('False');
            if (JSON.parse($('#CalendarJson' + calendarSuffix).val()).length !== 0) {
                let eventData = JSON.parse($('#CalendarJson' + calendarSuffix).val())[0]['items'];
                successCallback(
                    eventData.map((item) => {
                        item.start = item.start.replace('Z', '');
                        item.end = item.end.replace('Z', '');
                        var endDate = new Date(item.end);
                        if ($('#CalendarEditorFormat' + calendarSuffix).val() === 'Ymd') {
                            endDate.setDate(endDate.getDate() + 1);
                        }
                        if (item.StatusHtml) {
                            return {
                                id: item.id,
                                title: item.title,
                                start: item.start,
                                end: endDate,
                                StatusHtml: item.StatusHtml,
                                siteId: item.siteId
                            }
                        }
                        else {
                            return {
                                id: item.id,
                                title: item.title,
                                start: item.start,
                                end: endDate,
                                siteId: item.siteId
                            }
                        }
                    }))
            }
        }
    }
}
function setCalendarGroup(group, data, language, calendarSuffix) {
    var hash = {};
    var beginSelector = (group === undefined)
        ? '#Calendar' + calendarSuffix + ' .container:first'
        : '#Calendar' + calendarSuffix + ' .container[data-value="' + group + '"]:first';
    var endSelector = (group === undefined)
        ? '#Calendar' + calendarSuffix + ' .container:last'
        : '#Calendar' + calendarSuffix + ' .container[data-value="' + group + '"]:last';
    var begin = new Date($(beginSelector).attr('data-id'));
    var end = new Date($(endSelector).attr('data-id'));

    switch ($('#CalendarTimePeriod' + calendarSuffix).val()) {
        case 'Yearly':
            setYearly(group, data, hash, begin, end, language, calendarSuffix);
            break;
        case 'Monthly':
        case 'Weekly':
            setMonthly(group, data, hash, begin, end, language, calendarSuffix);
            break;
    }
}

function setYearly(group, data, hash, begin, end, language, calendarSuffix) {
    data.forEach(function (element) {
        element.From = (element.From.replace('Z', ''));
        element.To = (element.To.replace('Z', ''));
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
            language,
            calendarSuffix,
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
                    language,
                    calendarSuffix,
                    1,
                    rank);
                current.setMonth(current.getMonth() + 1);
            }
        }
    });
}

function setMonthly(group, data, hash, begin, end, language, calendarSuffix) {
    data.forEach(function (element) {
        element.From = (element.From.replace('Z', ''));
        element.To = (element.To.replace('Z', ''));
        var current = new Date(element.From);
        if (current < begin) {
            current = new Date(begin);
        }
        rank = Rank(hash, $p.shortDateString(current));
        addItem(
            group,
            hash,
            element,
            current,
            language,
            calendarSuffix);
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
                    language,
                    calendarSuffix,
                    current.getDay() !== 1,
                    rank);
                current.setDate(current.getDate() + 1);
            }
        }
    });
    if ($('#CalendarCanUpdate' + calendarSuffix).val() === '1') {
        $('#Calendar' + calendarSuffix + ' .item').draggable({
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
        $('#Calendar' + calendarSuffix + ' .container').droppable({
            hoverClass: 'hover',
            tolerance: 'intersect',
            drop: function (e, ui) {
                $p.clearData();
                var $control = $(ui.draggable);
                var from = new Date($control.attr('data-from'));
                var target = new Date($(this).attr('data-id'));
                var data = $p.getData($('.main-form'));
                var fromTo = $('#CalendarFromTo' + calendarSuffix).val().split('-');
                var prefix = $('#CalendarReferenceType' + calendarSuffix).val() + '_';
                if (calendarSuffix !== '') {
                    $p.set($('#CalendarSuffix' + calendarSuffix), $('#CalendarSuffix' + calendarSuffix).val());
                    data.SiteId = $control.attr('data-siteid');
                    data.Id = $('#Id').val();
                    data['EventId'] = $control.attr('data-id');
                } else {
                    data.Id = $control.attr('data-id');
                }
                data[prefix + fromTo[0]] = margeTime(target, from);
                if ($control.attr('data-to') !== undefined) {
                    var diff = $p.dateDiff('d', target, $p.shortDate(from));
                    var to = $p.dateAdd('d', diff, new Date($control.attr('data-to')));
                    data[prefix + fromTo[1]] = margeTime(to);
                }
                $p.saveScroll();
                $p.send($('#CalendarBody' + calendarSuffix));
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

function addItem(group, hash, element, current, language, calendarSuffix, sub, rank, yearly) {
    var id = $p.shortDateString(current);
    var groupSelector = (group === undefined)
        ? ''
        : '[data-value="' + group + '"]';
    var $cell = $('[id="' + 'Calendar' + calendarSuffix + '"] > div ' + groupSelector + '[data-id="' + id + '"] > div');
    while (Rank(hash, id) < rank) {
        $cell.append($('<div />').addClass('dummy'));
        hash[id]++;
    }
    var item = $(' <div />')
        .addClass('item')
        .addClass(element.Changed === true ? 'changed' : '')
        .attr('data-id', element.Id)
        .attr('data-from', element.From)
        .attr('data-to', element.To)
        .attr('data-siteid', element.SiteId);
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
        $p.dateTimeFormatString(new Date(element.From.replace('Z', '')), language) +
            (element.To !== undefined && element.To !== element.From
            ? ' - ' + $p.dateTimeFormatString(new Date(element.To.replace('Z', '')), language)
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
$p.moveCalendar = function (type, calendarSuffix) {
    $p.clearData();
    calendarSuffix = calendarSuffix > 0 ? '_' + calendarSuffix : calendarSuffix;
    var $control = $('#CalendarDate' + calendarSuffix);
    $control.val($('#Calendar' + type + calendarSuffix).val());
    if (calendarSuffix !== '') {
        $('#CalendarDate' + calendarSuffix).attr('data-action', 'calendar');
        $p.set($('#CalendarSuffix' + calendarSuffix), $('#CalendarSuffix' + calendarSuffix).val());
    }
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

function setFullCalendar(calendarSuffix, calendarEl, language) {
    var supportedLanguages = ['en', 'zh', 'ja', 'de', 'ko', 'es', 'vi'];
    $('#FullCalendar' + calendarSuffix).css('clear', 'both');
    let calendarMiddle = new Date();
    if ($('#CalendarStart' + calendarSuffix).val() !== '') {
        calendarMiddle = new Date((new Date($('#CalendarStart' + calendarSuffix).val()).getTime() + new Date($('#CalendarEnd' + calendarSuffix).val()).getTime()) / 2);
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
            window.location.href = $('#ApplicationPath').val() + 'items/' + e.event.id + '/edit';
        },
        select: newRecord(calendarSuffix),
        events: getEventsDatas(calendarSuffix),
        eventDrop: updateRecord(calendarSuffix),
        eventResize: updateRecord(calendarSuffix),
        eventDidMount: function (info) {
            var eventElement = $(info.el);
            var endDate = new Date(info.event.end);
            if ($('#CalendarEditorFormat' + calendarSuffix).val() === 'Ymd') {
                endDate.setDate(endDate.getDate() - 1);
            }
            eventElement.attr('title', htmlEncode(info.event.title) + ' -- ' +
                $p.dateTimeFormatString(new Date(info.event.start), language) +
                (info.event.end !== null && endDate.toLocaleString() !== info.event.start.toLocaleString()
                ? ' - ' + $p.dateTimeFormatString(new Date(endDate), language)
                    : ''))
                + htmlEncode(info.event.title);
            if (info.event.extendedProps.StatusHtml) {
                if (info.view.type === 'listMonth') {
                    var eventElement = $(info.el).find('.fc-list-event-graphic');
                    eventElement.append($.parseHTML(info.event.extendedProps.StatusHtml)[0]);
                    $('.fc-list-event-dot').css('margin-right', '20px');
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
        initialView: $('#CalendarViewType' + calendarSuffix).val(),
        lazyFetching: false,
        eventTimeFormat: {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        }
    });
    $p.fullCalendar.render();
    $('.fc-scrollgrid').addClass('no-drag');
}
$p.setCalendar = function (suffix) {
    var calendarElArr = $($('#MainForm').find('.calendar-container')).get();
    if (suffix) {
        calendarElArr = $('#MainForm').find('div[id$="Calendar_' + suffix + '"].calendar-container').get();
    }
    var language = $('#Language').val();
    if (language === 'vn') {
        language = 'vi';
    }
    $(calendarElArr).each(function (index, value) {
        var calendarSuffix = value.id.substring(value.id.indexOf('_'));
        calendarSuffix = calendarSuffix.indexOf('_') === -1 ? '' : calendarSuffix;

        if ($('#CalendarType' + calendarSuffix).val() == 'FullCalendar') {
            setFullCalendar(calendarSuffix, value, language);
        } else {
            $('#Calendar' + calendarSuffix + ' .container > div > div:not(.day)').remove();
            var data = JSON.parse($('#CalendarJson' + calendarSuffix).val());
            data.forEach(function (element) {
                setCalendarGroup(element.group, element.items, language, calendarSuffix);
            });
            $('#CalendarBody' + calendarSuffix).addClass('no-drag');
        }
    });
}
