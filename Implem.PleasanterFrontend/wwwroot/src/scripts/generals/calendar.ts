import type {
    DateSelectArg,
    EventChangeArg,
    EventClickArg,
    EventMountArg,
    EventSourceFuncArg,
    EventInput
} from '@fullcalendar/core';

interface CalendarItem {
    Id: number;
    SiteId: number;
    Title: string;
    From: string;
    To?: string;
    Changed?: boolean;
    Time?: string;
    StatusHtml?: string;
    DateFormat?: string;
}

interface CalendarEventJsonItem {
    id: number;
    title: string;
    start: string;
    end: string;
    StatusHtml?: string;
    siteId: number;
    DateFormat: string;
}

// サーバで階層設定を解決した週の開始曜日 (0:日曜〜6:土曜) を取得する。
// サーバの描画と一致させるためにこの値のみを使用する。
// ダッシュボードの部品はページとは異なるサイトの設定で描画されるため、
// カレンダーごとに出力された値を参照する。
// 取得できない場合は従来の既定値である月曜を返す。
const calendarFirstDayOfWeek = (calendarSuffix: string = ''): number => {
    const value = (document.getElementById(`ResolvedCalendarFirstDayOfWeek${calendarSuffix}`) as HTMLInputElement)
        ?.value;
    const parsed = Number(value);
    return value === undefined || value === '' || Number.isNaN(parsed) ? 1 : ((parsed % 7) + 7) % 7;
};

// 週の何列目かを 1 起点で返す。開始曜日が 1 列目となる。
// 複数日にまたがるバーが行内に収まる長さを求めるために使用する。
const weekColumn = (date: Date, firstDayOfWeek: number): number => ((date.getDay() - firstDayOfWeek + 7) % 7) + 1;

// サーバへ渡す日付時刻文字列を組み立てる。
// 日付の部分は、標準カレンダーのセルを特定する data-id と同じ書式である
// $p.shortDateString に揃える。
// toLocaleString はブラウザのロケールで区切り文字や順序が変わり、
// サーバ側で解釈できない形式になるため使用しない。
// time には時刻だけを別の日時から採りたい場合に、その日時を渡す。
const dateTimeString = (date: Date, time: Date = date): string =>
    `${$p.shortDateString(date)} ${time.getHours()}:${time.getMinutes()}:${time.getSeconds()}`;

const newRecord = (calendarSuffix: string) => {
    return (info: DateSelectArg) => {
        const endDate = new Date(info.end);
        if ($(`#CalendarEditorFormat${calendarSuffix}`).val() === 'Ymd' && isMidnight(endDate)) {
            endDate.setDate(endDate.getDate() - 1);
        }
        const form = document.createElement('form');
        form.setAttribute(
            'action',
            `${String($('#ApplicationPath').val())}items/${String($(`#CalendarSiteData${calendarSuffix}`).val())}/new`
        );
        form.setAttribute('method', 'post');
        form.style.display = 'none';
        document.body.appendChild(form);
        const start = document.createElement('input');
        start.setAttribute('type', 'hidden');
        start.setAttribute('name', `PostInit_${$(`#CalendarReferenceType${calendarSuffix}`).val()}_StartTime`);
        start.setAttribute('value', dateTimeString(info.start));
        form.appendChild(start);
        const end = document.createElement('input');
        end.setAttribute('type', 'hidden');
        end.setAttribute('name', `PostInit_${$(`#CalendarReferenceType${calendarSuffix}`).val()}_CompletionTime`);
        end.setAttribute('value', dateTimeString(endDate));
        form.appendChild(end);
        const fromTo = String($(`#CalendarFromTo${calendarSuffix}`).val()).split('-');
        // 開始と終了が別の項目に分かれている場合は、上で設定済みのため追加は不要。
        // 単一の項目の場合は、それが日付項目のときだけ値を設定し、
        // 日付項目でなければ日付を持たせられないため作成を中止する。
        if (!fromTo[1]) {
            const dateColumnPattern = /^Date/;
            if (!dateColumnPattern.test(fromTo[0])) {
                return;
            }
            const from = document.createElement('input');
            from.setAttribute('type', 'hidden');
            from.setAttribute('name', `PostInit_${$(`#CalendarReferenceType${calendarSuffix}`).val()}_${fromTo[0]}`);
            from.setAttribute('value', dateTimeString(info.start));
            form.appendChild(from);
        }
        if ($('#Token').length) {
            const input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('name', 'Token');
            input.setAttribute('value', String($('#Token').val()));
            form.appendChild(input);
        }
        form.submit();
    };
};

// 時刻を持たない日付として扱えるかどうかを返す。
const isMidnight = (date: Date) => {
    return date.getHours() === 0 && date.getMinutes() === 0 && date.getSeconds() === 0;
};
const updateRecord = (calendarSuffix: string) => {
    return (info: EventChangeArg) => {
        $p.clearData();
        const data = $p.getData($('.main-form'));
        const fromTo = String($(`#CalendarFromTo${calendarSuffix}`).val()).split('-');
        const prefix = `${$(`#CalendarReferenceType${calendarSuffix}`).val()}_`;
        if (calendarSuffix !== '') {
            $p.set($(`#CalendarSuffix${calendarSuffix}`), String($(`#CalendarSuffix${calendarSuffix}`).val()));
            data.SiteId = info.event.extendedProps.siteId;
            data.Id = $('#Id').val();
            data['EventId'] = info.event.id;
            $p.set($(`#CalendarStart${calendarSuffix}`), String($(`#CalendarStart${calendarSuffix}`).val()));
            $p.set($(`#CalendarEnd${calendarSuffix}`), String($(`#CalendarEnd${calendarSuffix}`).val()));
            $p.set($(`#CalendarViewType${calendarSuffix}`), String($(`#CalendarViewType${calendarSuffix}`).val()));
        } else {
            data.Id = info.event.id;
        }
        data[prefix + fromTo[0]] = dateTimeString(info.event.start!);
        if (info.event.end === null) {
            data[prefix + fromTo[1]] = dateTimeString(info.event.start!);
        } else {
            const endDate = new Date(info.event.end);
            if ($(`#CalendarEditorFormat${calendarSuffix}`).val() === 'Ymd') {
                endDate.setDate(endDate.getDate() - 1);
            }
            data[prefix + fromTo[1]] = dateTimeString(endDate);
        }
        $p.saveScroll();
        $p.send($(`#FullCalendarBody${calendarSuffix}`));
    };
};
const getEventsData = (calendarSuffix: string) => {
    return (
        info: EventSourceFuncArg,
        successCallback: (events: EventInput[]) => void,
        _failureCallback: (error: Error) => void
    ) => {
        if (
            $(`#IsInit${calendarSuffix}`).val() !== 'True' &&
            !(
                info.start.getTime() === Date.parse(String($(`#CalendarStart${calendarSuffix}`).val())) &&
                info.end.getTime() === Date.parse(String($(`#CalendarEnd${calendarSuffix}`).val()))
            )
        ) {
            $p.clearData();
            $p.set($(`#CalendarStart${calendarSuffix}`), $p.shortDateString(info.start));
            $p.set($(`#CalendarEnd${calendarSuffix}`), $p.shortDateString(info.end));
            $(`#FullCalendarBody${calendarSuffix}`).attr('data-action', 'calendar');
            if (calendarSuffix !== '') {
                $p.set($(`#CalendarSuffix${calendarSuffix}`), String($(`#CalendarSuffix${calendarSuffix}`).val()));
            }

            const calendarDiff = Math.round((info.end.getTime() - info.start.getTime()) / (1000 * 60 * 60 * 24));
            if (calendarDiff === 1) {
                $p.set($(`#CalendarViewType${calendarSuffix}`), 'timeGridDay');
            } else if (calendarDiff === 7) {
                $p.set($(`#CalendarViewType${calendarSuffix}`), 'timeGridWeek');
            } else if (calendarDiff < 32) {
                $p.set($(`#CalendarViewType${calendarSuffix}`), 'listMonth');
            } else {
                $p.set($(`#CalendarViewType${calendarSuffix}`), 'dayGridMonth');
            }

            const $control = $(`#FullCalendarBody${calendarSuffix}`);
            $p.send($control);
        } else {
            $(`#IsInit${calendarSuffix}`).val('False');
            let calendarJson: { items: CalendarEventJsonItem[] }[];
            try {
                calendarJson = JSON.parse(String($(`#CalendarJson${calendarSuffix}`).val()));
            } catch {
                successCallback([]);
                return;
            }
            if (calendarJson.length !== 0) {
                const eventData = calendarJson[0].items;
                successCallback(
                    eventData.map((item: CalendarEventJsonItem) => {
                        const endDate = new Date(removeTimeZoneSuffix(item.end)!);
                        if ($(`#CalendarEditorFormat${calendarSuffix}`).val() === 'Ymd') {
                            endDate.setDate(endDate.getDate() + 1);
                            endDate.setHours(0, 0, 0, 0);
                        }
                        return {
                            id: String(item.id),
                            title: item.title,
                            start: removeTimeZoneSuffix(item.start),
                            end: endDate,
                            ...(item.StatusHtml && { StatusHtml: item.StatusHtml }),
                            siteId: item.siteId,
                            format: item.DateFormat,
                            tooltipEnd: item.end
                        };
                    })
                );
            } else {
                successCallback([]);
            }
        }
    };
};
const setCalendarGroup = (group: string | undefined, data: CalendarItem[], calendarSuffix: string) => {
    const hash: Record<string, number> = {};
    const beginSelector =
        group === undefined
            ? `#Calendar${calendarSuffix} .container:first`
            : `#Calendar${calendarSuffix} .container[data-value="${$.escapeSelector(group)}"]:first`;
    const endSelector =
        group === undefined
            ? `#Calendar${calendarSuffix} .container:last`
            : `#Calendar${calendarSuffix} .container[data-value="${$.escapeSelector(group)}"]:last`;
    const begin = new Date($(beginSelector).attr('data-id')!);
    const end = new Date($(endSelector).attr('data-id')!);

    switch ($(`#CalendarTimePeriod${calendarSuffix}`).val()) {
        case 'Yearly':
            setYearly(group, data, hash, begin, end, calendarSuffix);
            break;
        case 'Monthly':
        case 'Weekly':
            setMonthly(group, data, hash, begin, end, calendarSuffix);
            break;
    }
};

const setYearly = (
    group: string | undefined,
    data: CalendarItem[],
    hash: Record<string, number>,
    begin: Date,
    end: Date,
    calendarSuffix: string
) => {
    const firstDayOfWeek = calendarFirstDayOfWeek(calendarSuffix);
    data.forEach((element: CalendarItem) => {
        element.From = removeTimeZoneSuffix(element.From)!;
        element.To = removeTimeZoneSuffix(element.To);
        let current = $p.beginningMonth(new Date(element.From));
        if (current < begin) {
            current = new Date(begin);
        }
        const rank = getRank(hash, $p.shortDateString(current));
        addItem(group, hash, element, current, calendarSuffix, firstDayOfWeek, { yearly: true });
        if (element.To !== undefined) {
            current.setMonth(current.getMonth() + 1);
            let to = new Date(element.To);
            if (to > end) {
                to = end;
            }
            while ($p.shortDate(to) >= $p.shortDate(current)) {
                addItem(group, hash, element, current, calendarSuffix, firstDayOfWeek, { sub: true, rank });
                current.setMonth(current.getMonth() + 1);
            }
        }
    });
};

const setMonthly = (
    group: string | undefined,
    data: CalendarItem[],
    hash: Record<string, number>,
    begin: Date,
    end: Date,
    calendarSuffix: string
) => {
    const firstDayOfWeek = calendarFirstDayOfWeek(calendarSuffix);
    data.forEach((element: CalendarItem) => {
        element.From = removeTimeZoneSuffix(element.From)!;
        element.To = removeTimeZoneSuffix(element.To);
        if (element.To && begin > new Date(element.To)) return;
        let current = new Date(element.From);
        if (current < begin) {
            current = new Date(begin);
        }
        let rank = getRank(hash, $p.shortDateString(current));
        addItem(group, hash, element, current, calendarSuffix, firstDayOfWeek, { rank: 0 });
        if (element.To !== undefined) {
            current.setDate(current.getDate() + 1);
            let to = new Date(element.To);
            if (to > end) {
                to = end;
            }
            while ($p.shortDate(to) >= $p.shortDate(current)) {
                // 週の先頭に達したら、その週で改めて段を確保して折り返す。
                if (current.getDay() === firstDayOfWeek) {
                    rank = getRank(hash, $p.shortDateString(current));
                }
                addItem(group, hash, element, current, calendarSuffix, firstDayOfWeek, {
                    sub: current.getDay() !== firstDayOfWeek,
                    rank
                });
                current.setDate(current.getDate() + 1);
            }
        }
    });
    if ($(`#CalendarCanUpdate${calendarSuffix}`).val() === '1') {
        $(`#Calendar${calendarSuffix} .item`).draggable({
            revert: 'invalid',
            cursorAt: { top: 0, left: 0 },
            start: function () {
                $(this).parent().droppable({
                    disabled: true
                });
            },
            helper: function () {
                return $('<div />').addClass('dragging').append($(this).text());
            }
        });
        $(`#Calendar${calendarSuffix} .container`).droppable({
            hoverClass: 'hover',
            tolerance: 'pointer',
            drop: function (e, ui) {
                $p.clearData();
                const $control = $(ui.draggable);
                const from = new Date($control.attr('data-from')!);
                const target = new Date($(this).attr('data-id')!);
                const data = $p.getData($('.main-form'));
                const fromTo = String($(`#CalendarFromTo${calendarSuffix}`).val()).split('-');
                const prefix = `${String($(`#CalendarReferenceType${calendarSuffix}`).val())}_`;
                const groupByName: string | undefined = (
                    document.querySelector(`#CalendarGroupByColumnName${calendarSuffix}`) as HTMLInputElement | null
                )?.value;
                const groupByValue = (e.target as HTMLElement).dataset.value;
                if (calendarSuffix !== '') {
                    $p.set($(`#CalendarSuffix${calendarSuffix}`), String($(`#CalendarSuffix${calendarSuffix}`).val()));
                    data.SiteId = $control.attr('data-siteid');
                    data.Id = $('#Id').val();
                    data['EventId'] = $control.attr('data-id');
                } else {
                    data.Id = $control.attr('data-id');
                }
                data[`${prefix}${fromTo[0]}`] = dateTimeString(target, from);
                const dataTo = $control.attr('data-to');
                if (dataTo) {
                    const diff = $p.dateDiff('d', target, $p.shortDate(from));
                    const to = $p.dateAdd('d', diff, new Date(dataTo));
                    data[`${prefix}${fromTo[1]}`] = dateTimeString(to);
                }
                if (groupByName && groupByValue !== undefined) {
                    data[`${prefix}${groupByName}`] = groupByValue;
                }
                $p.saveScroll();
                $p.send($(`#CalendarBody${calendarSuffix}`));
            }
        });
    }
};

const getRank = (hash: Record<string, number>, id: string) => {
    if (hash[id] === undefined) {
        hash[id] = 0;
    }
    return hash[id];
};

// カレンダーのセルに項目を 1 つ追加する。
// sub: 前のセルから続いている項目である場合に true。
//      この場合はタイトルを描画せず、継続を示す部分のみを描画する。
// rank: セル内で上から何段目に配置するか。
// yearly: 年間表示の場合に true。バーの長さを日数ではなく月数で求める。
interface AddItemOptions {
    sub?: boolean;
    rank?: number;
    yearly?: boolean;
}

const addItem = (
    group: string | undefined,
    hash: Record<string, number>,
    element: CalendarItem,
    current: Date,
    calendarSuffix: string,
    firstDayOfWeek: number,
    options: AddItemOptions = {}
) => {
    const { sub = false, rank = 0, yearly = false } = options;
    const id = $p.shortDateString(current);
    const groupSelector = group === undefined ? '' : `[data-value="${$.escapeSelector(group)}"]`;
    const $cell = $(`[id="Calendar${calendarSuffix}"] > div ${groupSelector}[data-id="${id}"] > div`);
    while (getRank(hash, id) < rank) {
        $cell.append($('<div />').addClass('dummy'));
        hash[id]++;
    }
    const item = $('<div />')
        .addClass('item')
        .addClass(element.Changed === true ? 'changed' : '')
        .attr('data-id', element.Id)
        .attr('data-from', element.From)
        .attr('data-siteid', element.SiteId);
    if (element.To !== undefined) {
        item.attr('data-to', element.To);
    }
    if (sub) {
        item.append(
            $('<div />')
                .attr('data-id', element.Id)
                .addClass('connection')
                .addClass(element.Changed === true ? 'changed' : '')
        );
    }
    item.append(
        $('<div />')
            .addClass('title')
            .css('width', () => {
                const width = $cell.parent().width() ?? 0;
                const margin = 16;
                if (sub) {
                    return '';
                } else if (element.To === undefined) {
                    return width - margin;
                } else if (yearly) {
                    let diff = 0;
                    const month = new Date(current);
                    month.setMonth(month.getMonth() + 1);
                    while (month <= new Date(element.To)) {
                        diff++;
                        month.setMonth(month.getMonth() + 1);
                    }
                    return width * (diff + 1) - margin;
                } else {
                    // 終了日までの日数だけバーを伸ばす。
                    // ただし週をまたぐ場合はその週の最終列で止め、
                    // 残りは次の行の項目として別途描画する。
                    let diff = $p.dateDiff('d', $p.shortDate(new Date(element.To)), $p.shortDate(current));
                    const col = weekColumn(current, firstDayOfWeek);
                    const lastColumn = 7;
                    if (col + diff > lastColumn) {
                        diff = lastColumn - col;
                    } else if (diff < 0) {
                        diff = 0;
                    }
                    return width * (diff + 1) - margin;
                }
            })
            .addClass(sub ? 'sub' : '')
            .attr(
                'title',
                tooltipTitle(
                    element.Title,
                    new Date(element.From),
                    element.To !== undefined && element.To !== element.From ? new Date(element.To) : undefined,
                    element.DateFormat ?? ''
                )
            )
            .append($('<span />').addClass('material-symbols-sharp edit-icon').text('edit'))
            .append(
                `${element.Time !== undefined ? `${element.Time} ` : ''}${element.StatusHtml ? element.StatusHtml : ''}`
            )
            .append($('<span />').addClass('title-label').text(htmlEncode(element.Title)))
    );
    $cell.append(item);
    hash[id]++;
};
const removeTimeZoneSuffix = (datetimeStr: string | undefined) => {
    if (datetimeStr === undefined) {
        return datetimeStr;
    } else {
        return datetimeStr.replace('Z', '');
    }
};

const htmlEncode = (str: string) => {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
};

// 項目にマウスを重ねたときに表示する文言を組み立てる。
// 期間として表示しない場合は to に undefined を渡す。
const tooltipTitle = (title: string, from: Date, to: Date | undefined, format: string) => {
    const period = to === undefined ? '' : ` - ${$p.dateTimeFormatString(to, format)}`;
    return `${htmlEncode(title)} -- ${$p.dateTimeFormatString(from, format)}${period}`;
};
$p.moveCalendar = (type: string, calendarSuffix: string | number) => {
    $p.clearData();
    calendarSuffix = Number(calendarSuffix) > 0 ? `_${calendarSuffix}` : calendarSuffix;
    const $control = $(`#CalendarDate${calendarSuffix}`);
    $control.val(String($(`#Calendar${type}${calendarSuffix}`).val()));
    if (calendarSuffix !== '') {
        $(`#CalendarDate${calendarSuffix}`).attr('data-action', 'calendar');
        $p.set($(`#CalendarSuffix${calendarSuffix}`), String($(`#CalendarSuffix${calendarSuffix}`).val()));
    }
    $p.setData($control);
    $p.send($control);
};

const setFullCalendar = (calendarSuffix: string, calendarEl: HTMLElement) => {
    let language = String($('#Language').val());
    const supportedLanguages = ['en', 'zh', 'ja', 'de', 'ko', 'es', 'vi'];
    if (language === 'vn') {
        language = 'vi';
    }
    const firstDay = calendarFirstDayOfWeek(calendarSuffix);
    $(`#FullCalendar${calendarSuffix}`).css('clear', 'both');
    let calendarMiddle = new Date();
    if ($(`#CalendarStart${calendarSuffix}`).val() !== '') {
        calendarMiddle = new Date(
            (new Date(String($(`#CalendarStart${calendarSuffix}`).val())).getTime() +
                new Date(String($(`#CalendarEnd${calendarSuffix}`).val())).getTime()) /
                2
        );
    }
    if ($p.fullCalendars === undefined) {
        $p.fullCalendars = {};
    }
    if ($p.fullCalendars[calendarSuffix]) {
        $p.fullCalendars[calendarSuffix].destroy();
    }
    $p.fullCalendars[calendarSuffix] = new FullCalendar.Calendar(calendarEl, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listMonth'
        },
        firstDay,
        initialDate: calendarMiddle,
        selectable: true,
        navLinks: true,
        businessHours: true,
        editable: true,
        height: 'auto',
        locale: supportedLanguages.includes(language) ? language : 'en',
        selectMirror: true,
        eventClick: (e: EventClickArg) => {
            window.location.href = `${String($('#ApplicationPath').val())}items/${e.event.id}/edit`;
        },
        select: newRecord(calendarSuffix),
        events: getEventsData(calendarSuffix),
        eventDrop: updateRecord(calendarSuffix),
        eventResize: updateRecord(calendarSuffix),
        eventDidMount: (info: EventMountArg) => {
            const eventElement = $(info.el);
            const startDate = info.event.start!;
            const endDate = new Date(removeTimeZoneSuffix(info.event.extendedProps.tooltipEnd)!);
            // 開始と終了が同じ日時であれば期間として表示しない。
            const isPeriod = info.event.end !== null && endDate.getTime() !== startDate.getTime();
            eventElement.attr(
                'title',
                tooltipTitle(
                    info.event.title,
                    startDate,
                    isPeriod ? endDate : undefined,
                    info.event.extendedProps.format
                )
            );
            if (info.event.extendedProps.StatusHtml) {
                if (info.view.type === 'listMonth') {
                    const $listGraphic = $(info.el).find('.fc-list-event-graphic');
                    $listGraphic.append($.parseHTML(info.event.extendedProps.StatusHtml)[0]);
                    $(info.el).find('.fc-list-event-dot').css('margin-right', '20px');
                } else {
                    const $eventTime = $(info.el).find('.fc-event-time');
                    $eventTime.prepend($.parseHTML(info.event.extendedProps.StatusHtml)[0]);
                }
                const $el = $(info.el);
                $el.find('.status-new').css({ color: 'black', border: 'solid 1px #000' });
                $el.find('.status-review').css('color', 'black');
                $el.find("[class^='status']").css({ padding: '1px 3px', margin: '0px 3px', width: '15px' });
            }
        },
        initialView: $(`#CalendarViewType${calendarSuffix}`).val(),
        lazyFetching: false,
        eventTimeFormat: {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        }
    });
    $p.fullCalendars[calendarSuffix].render();
    $('.fc-scrollgrid').addClass('no-drag');
};
$p.setCalendar = suffix => {
    let calendarElArr = $('#MainForm').find('.calendar-container').get();
    if (suffix) {
        calendarElArr = $('#MainForm').find(`div[id$="Calendar_${suffix}"].calendar-container`).get();
    }
    $(calendarElArr).each((_, value) => {
        let calendarSuffix = value.id.substring(value.id.indexOf('_'));
        calendarSuffix = calendarSuffix.indexOf('_') === -1 ? '' : calendarSuffix;
        if ($(`#CalendarType${calendarSuffix}`).val() === 'FullCalendar') {
            setFullCalendar(calendarSuffix, value);
        } else {
            $(`#Calendar${calendarSuffix} .container > div > div:not(.day)`).remove();
            let data: { group: string | undefined; items: CalendarItem[] }[];
            try {
                data = JSON.parse(String($(`#CalendarJson${calendarSuffix}`).val()));
            } catch {
                return;
            }
            data.forEach((element: { group: string | undefined; items: CalendarItem[] }) => {
                setCalendarGroup(element.group, element.items, calendarSuffix);
            });
            $(`#CalendarBody${calendarSuffix}`).addClass('no-drag');
        }
    });
};
