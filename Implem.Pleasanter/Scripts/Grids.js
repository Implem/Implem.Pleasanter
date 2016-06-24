$(function () {
    $(document).on('click', '#GridCheckAll', function () {
        $('.grid-check').prop('checked', $('#GridCheckAll').prop('checked'));
        getFormData($(this))['GridUnCheckedItems'] = '';
        getFormData($(this))['GridCheckedItems'] = '';
    });
    $(document).on('change', '.grid-check', function () {
        if ($('#GridCheckAll').prop('checked')) {
            getFormData($(this))['GridUnCheckedItems'] =
                $('.grid-check').filter(':not(:checked)')
                    .map(function () { return $(this).attr('data-id'); })
                    .get()
                    .join(',');
        } else {
            getFormData($(this))['GridCheckedItems'] =
                $('.grid-check').filter(':checked')
                    .map(function () { return $(this).attr('data-id'); })
                    .get()
                    .join(',');
        }
    });
    $(document).on('click', '.grid-row:not(.not-link) td', function () {
        var $control = $(this).find('.grid-check');
        if ($control.length === 0) {
            location.href = $('#BaseUrl').val() + $(this).closest('.grid-row').attr('data-id');
        } else {
            $control.trigger('click');
        }
    });
    $(document).on('click', '#DataViewFilters_Reset', function () {
        $('[id^="DataViewFilters_"]').each(function () {
            switch ($(this).prop('tagName')) {
                case 'INPUT':
                    switch ($(this).prop('type')) {
                        case 'checkbox': $(this).prop('checked', false); break;
                        case 'text': $(this).val(''); break;
                    }
                    break;
                case 'SELECT': $(this).val(''); break;
            }
            setFormData($(this));
        });
        requestByForm(getForm($(this)), $(this));
    });
    $(document).on('change', '#AggregationType', function () {
        if ($(this).val() === 'Count') {
            $('#AggregationTarget').closest('.togglable').hide();
        } else {
            $('#AggregationTarget').closest('.togglable').show();
        }
    });
    $(document).on('click', '#Aggregations .data.link', function () {
        var $control = $($(this).attr('data-selector'));
        if ($control.length === 1) {
            if ($control.val() === '') {
                $control.val($(this).attr('data-value'));
            } else {
                $control.val('');
            }
            $control.trigger('change');
        }
    });
});
$(function () {
    var timer;
    $(document).on('mouseenter', 'th.sortable', function () {
        timer = setTimeout(function ($control) {
            $control.append($('<ul/>')
                .attr('data-target', '#' + $control.children('div').attr('id'))
                .addClass('menu-sort')
                    .addMenu('ui-icon-triangle-1-n', 'Asc')
                    .addMenu('ui-icon-triangle-1-s', 'Desc')
                    .addMenu('ui-icon-close', 'Release')
                    .addMenuReset());
            $('.menu-sort')
                .css('top', $control.position().top + $control.outerHeight())
                .css('left', $control.position().left)
                .outerWidth($control.outerWidth());
        }, 700, $(this));
    });
    $(document).on('mouseleave', 'th.sortable', function () {
        clearTimeout(timer);
        $('.menu-sort').remove();
    });
    $.fn.extend({
        addMenu: function (iconCss, orderType) {
            $(this).append($('<li/>')
                .addClass('sort')
                .attr('data-order-type', orderType)
                .attr('data-action', 'GridRows')
                .attr('data-method', 'post')
                .append($('<span/>').addClass('ui-icon ' + iconCss))
                .append($('<span/>').text(getDisplay('Displays_Order' + orderType))));
            return $(this);
        },
        addMenuReset: function () {
            $(this).append($('<li/>')
                .addClass('reset')
                .attr('data-action', 'GridRows')
                .attr('data-method', 'post')
                .append($('<span/>').addClass('ui-icon ui-icon-power'))
                .append($('<span/>').text(getDisplay('Displays_ResetOrder'))));
            return $(this);
        }
    });
    $(document).on('click', '.menu-sort > li.sort', function (e) {
        var $control = $($(this).parent().attr('data-target'));
        var formData = getFormData($control);
        formData[$control.attr('id')] = $(this).attr('data-order-type');
        requestByForm(getForm($(this)), $(this));
        delete formData[$control.attr('id')];
        e.stopPropagation();
    });
    $(document).on('click', '.menu-sort > li.reset', function (e) {
        var data = getFormData($(this));
        $('[id^="GridSorters_"]').each(function () {
            data[this.id] = '';
        });
        requestByForm(getForm($(this)), $(this));
        $('[id^="GridSorters_"]').each(function () {
            delete data[this.id];
        });
        e.stopPropagation();
    });
    $(document).on('click', 'th.sortable', function () {
        var $control = $(this).find('div');
        var formData = getFormData($control);
        formData[$control.attr('id')] = $control.attr('data-order-type');
        requestByForm(getForm($control), $control);
        delete formData[$control.attr('id')];
    });
});
$(function () {
    var timer;
    $(document).on('mouseenter', '.grid-row .grid-title-body, .grid-row .comment', function () {
        $(this).addClass('focus-inform');
        timer = setTimeout(function ($control) {
            $control.addClass('height-auto');
        }, 700, $(this));
    });
    $(document).on('mouseleave', '.grid-row .grid-title-body, .grid-row .comment', function () {
        clearTimeout(timer);
        $(this)
            .removeClass('height-auto')
            .removeClass('focus-inform');
    });
});
