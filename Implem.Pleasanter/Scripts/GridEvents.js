﻿$(function () {
    $(document).on('click', '#GridCheckAll', function () {
        $('.grid-check').prop('checked', $('#GridCheckAll').prop('checked'));
        var data = $p.getData($(this));
        data.GridUnCheckedItems = '';
        data.GridCheckedItems = '';
    });
    $(document).on('change', '.grid-check', function () {
        var $control = $(this);
        if ($('#GridCheckAll').prop('checked')) {
            $p.getData($control).GridUnCheckedItems =
                $('.grid-check').filter(':not(:checked)')
                    .map(function () { return $(this).attr('data-id'); })
                    .get()
                    .join(',');
        } else {
            $p.getData($control).GridCheckedItems =
                $('.grid-check').filter(':checked')
                    .map(function () { return $(this).attr('data-id'); })
                    .get()
                    .join(',');
        }
    });
    $(document).on('change', '.grid .select', function () {
        $p.setData($(this).closest('.grid'))
    });
    $(document).on('change', '.grid .select-all', function () {
        $control = $(this);
        $grid = $(this).closest('.grid');
        $grid.find('.select').prop('checked', $control.prop('checked'));
        $p.setData($grid);
    });
    $(document).on('click', '.grid-row td', function () {
        var $control = $(this).find('.grid-check,.select');
        if ($control.length === 0) {
            var $grid = $(this).closest('.grid');
            if (!$grid.hasClass('not-link')) {
                if ($grid.hasClass('history')) {
                    if (!$p.confirmReload()) return false;
                    var $control = $(this).closest('.grid-row');
                    var data = $p.getData($control);
                    data.Ver = $control.attr('data-ver');
                    data.Latest = $control.attr('data-latest');
                    data.SwitchTargets = $('#SwitchTargets').val();
                    $p.syncSend($control);
                    $p.setCurrentIndex();
                } else {
                    var func = $grid.attr('data-func');
                    var dataId = $(this).closest('.grid-row').attr('data-id');
                    if (func) {
                        $p.getData($grid)[$grid.attr('data-name')] = dataId;
                        $p[func]($grid);
                    }
                    else {
                        if ($('#EditorDialog').length === 1) {
                            var data = {};
                            data.EditInDialog = true;
                            url = $('#BaseUrl').val() + dataId;
                            $p.ajax(url, 'post', data);
                        } else {
                            location.href = $('#BaseUrl').val() + dataId +
                                ($grid.attr('data-value') === 'back'
                                    ? '?back=1'
                                    : '');
                        }
                    }
                }
            }
        } else if (!$p.hoverd($control)) {
            $control.trigger('click');
        }
    });
});
$(function () {
    var timer;
    $(document).on('mouseenter', 'th.sortable', function () {
        timer = setTimeout(function ($control) {
            $control.append($('<ul/>')
                .attr('data-target', '[data-id="' + $control.children('div').attr('data-id') + '"]')
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
                .attr('data-order-type', orderType.toLowerCase())
                .append($('<span/>').addClass('ui-icon ' + iconCss))
                .append($('<span/>').text($p.display('Order' + orderType))));
            return $(this);
        },
        addMenuReset: function () {
            $(this).append($('<li/>')
                .addClass('reset')
                .append($('<span/>').addClass('ui-icon ui-icon-power'))
                .append($('<span/>').text($p.display('ResetOrder'))));
            return $(this);
        }
    });
    $(document).on('click', '.menu-sort > li.sort', function (e) {
        sort($($(this).parent().attr('data-target')), $(this).attr('data-order-type'));
    });
    $(document).on('click', '.menu-sort > li.reset', function (e) {
        var $control = $(this);
        var $grid = $control.closest('.grid');
        var data = $p.getData($control);
        data.Direction = $grid.attr('data-name');
        data.TableId = $grid.attr('id');
        data.TableSiteId = $grid.attr('data-id');
        $grid.find('[data-id^="ViewSorters_"]').each(function () {
            delete data[$(this).attr('data-id')];
        });
        $p.send($('#ViewSorters_Reset'));
        e.stopPropagation();
    });
    $(document).on('click', 'th.sortable', function () {
        var $control = $(this).find('div');
        sort($control, $control.attr('data-order-type'))
    });

    function sort($control, type) {
        var $grid = $control.closest('.grid');
        var data = $p.getData($control);
        data[$control.attr('data-id')] = type;
        data.Direction = $grid.attr('data-name');
        data.TableId = $grid.attr('id');
        data.TableSiteId = $grid.attr('data-id');
        $p.send($grid);
        delete data[$control.attr('id')];
        e.stopPropagation();
    }
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
