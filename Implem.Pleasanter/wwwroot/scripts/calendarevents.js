$(function () {
    $(document).on('dblclick', 'div[id$="Calendar"]:not(.dashboard-custom-html-body div[id$="Calendar"]) .item,div[id="Calendar"]:not(.dashboard-custom-html-body div[id="Calendar"]) .item', function () {
        $p.transition($('#BaseUrl').val() + $(this).attr('data-id'));
    });
    $(document).on('click', 'div[id$="Calendar"]:not(.dashboard-custom-html-body div[id$="Calendar"]) .item .ui-icon-pencil,div[id="Calendar"]:not(.dashboard-custom-html-body div[id="Calendar"]) .item .ui-icon-pencil', function () {
        $p.transition($('#BaseUrl').val() + $(this).parent().parent().attr('data-id'));
    });
    $(document).on('mouseenter', 'div[id$="Calendar"]:not(.dashboard-custom-html-body div[id$="Calendar"]) .item,div[id="Calendar"]:not(.dashboard-custom-html-body div[id="Calendar"]) .item', function () {
        $('[data-id="' + $(this).attr('data-id') + '"]').addClass('hover');
    });
    $(document).on('mouseleave', 'div[id$="Calendar"]:not(.dashboard-custom-html-body div[id$="Calendar"]) .item,div[id="Calendar"]:not(.dashboard-custom-html-body div[id="Calendar"]) .item', function () {
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
    $(document).on('dblclick', 'div[id$="Calendar"]:not(.dashboard-custom-html-body div[id$="Calendar"]) .ui-droppable,div[id="Calendar"]:not(.dashboard-custom-html-body div[id="Calendar"]) .ui-droppable', function (event) {
        var calendarPrefix = $(this).parent().parent().parent().parent().attr("id").replace(/[^0-9]/g, '');
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
            input.setAttribute('name', $('#' + calendarPrefix + 'CalendarReferenceType').val() + '_' + name);
            input.setAttribute('value', value);
            form.appendChild(input);
        }
        if ($(event.target).is('.title')) return;
        var baseDate = new Date($(this).attr('data-id'));
        var names = $('#' + calendarPrefix + 'CalendarFromTo').val().split('-');
        var form = document.createElement("form");
        form.setAttribute("action", $('#ApplicationPath').val() + 'items/' + $('#' + calendarPrefix + 'CalendarSiteData').val() + '/new');
        form.setAttribute("method", "post");
        form.style.display = "none";
        document.body.appendChild(form);
        addInput(form, names[0], addDate(baseDate, $('#' + calendarPrefix + 'CalendarFromDefaultInput').val()));
        addInput(form, names[1], addDate(baseDate, $('#' + calendarPrefix + 'CalendarToDefaultInput').val()));
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