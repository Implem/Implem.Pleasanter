$(function () {
    $(document).on('dblclick', '.Calendar .item', function () {
        $p.transition($('#BaseUrl').val() + $(this).attr('data-id'));
    });
    $(document).on('click', '.Calendar .item .ui-icon-pencil', function () {
        $p.transition($('#BaseUrl').val() + $(this).parent().parent().attr('data-id'));
    });
    $(document).on('mouseenter', '.Calendar .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').addClass('hover');
    });
    $(document).on('mouseleave', '.Calendar .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').removeClass('hover');
    });
    $(window).on('resize', function () {
        if ($('#Calendar').length === 1) {
            setTimeout(function () {
                $p.saveScroll();
                $p.setCalendar('#Grid');
                $p.loadScroll();
            }, 10);
        }
    });
    $(document).on('dblclick', '.Calendar .ui-droppable', function (event) {
        var calendarSuffix = $(this).parents('div[id^="Calendar"]').attr("id").substring($(this).parents('div[id^="Calendar"]').attr("id").indexOf("_"));
        calendarSuffix = calendarSuffix.indexOf('_') === -1 ? '' : calendarSuffix;
        var addDate = function (baseDate, add) {
            if (add === '') return '';
            var date = new Date(baseDate.getTime());
            date.setDate(date.getDate() + parseInt(add, 10));
            return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
        }
        var addInput = function (form, name, value) {
            if (!name) return;
            var input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('name', $('#CalendarReferenceType' + calendarSuffix).val() + '_' + name);
            input.setAttribute('value', value);
            form.appendChild(input);
        }
        if ($(event.target).is('.title')) return;
        var baseDate = new Date($(this).attr('data-id'));
        var names = $('#CalendarFromTo' + calendarSuffix).val().split('-');
        var form = document.createElement("form");
        form.setAttribute("action", $('#ApplicationPath').val() + 'items/' + $('#CalendarSiteData' + calendarSuffix).val() + '/new');
        form.setAttribute("method", "post");
        form.style.display = "none";
        document.body.appendChild(form);
        addInput(form, names[0], addDate(baseDate, $('#CalendarFromDefaultInput' + calendarSuffix).val()));
        addInput(form, names[1], addDate(baseDate, $('#CalendarToDefaultInput' + calendarSuffix).val()));
        if ($('#Token').length) {
            var input = document.createElement('input');
            input.setAttribute('type', 'hidden');
            input.setAttribute('name', 'Token');
            input.setAttribute('value', $('#Token').val());
            form.appendChild(input);
        }
        form.submit();
    });
    $(document).on('click', '.calendar-to-monthly', function () {
        var data = {
            'CalendarTimePeriod': 'Monthly',
            'CalendarDate': $(this).attr('data-id')
        };
        $p.ajax(location.href, 'post', data);
    });
});