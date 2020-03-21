$(function () {
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
                    var dataVer = $(this).closest('.grid-row').attr('data-ver');
                    var dataHistory = $(this).closest('.grid-row').attr('data-history');
                    if (func) {
                        $p.getData($grid)[$grid.attr('data-name')] = dataId;
                        $p[func]($grid);
                    }
                    else {
                        var paramVer = dataHistory ? '?ver=' + dataVer : '';
                        var paramBack = paramVer ? '&back=1' : '?back=1';
                        if ($('#EditorDialog').length === 1) {
                            var data = {};
                            data.EditInDialog = true;
                            url = $('#BaseUrl').val() + dataId
                                + paramVer;
                            $p.ajax(url, 'post', data);
                        } else {
                            location.href = $('#BaseUrl').val() + dataId
                                + paramVer
                                + ($grid.attr('data-value') === 'back'
                                    ? paramBack
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
    $(document).on('mouseenter', 'table > thead > tr > th.sortable', function () {
        clearTimeout(timer);
        if ($(".menu-sort:visible").length) {
            $(".menu-sort:visible").hide();
        }
        if ($('.ui-multiselect-close:visible').length) {
            $('.ui-multiselect-close:visible').click();
        }
        timer = setTimeout(function ($control) {
            var dataName = $control.attr('data-name');
            $menuSort = $(".menu-sort[id='GridHeaderMenu__" + dataName + "']");
            $menuSort.css('width', '');
            $menuSort
                .css('position', 'absolute')
                .css('top', $control.position().top + $control.outerHeight())
                .css('left', $control.position().left)
                .outerWidth($control.outerWidth() > $menuSort.outerWidth()
                    ? $control.outerWidth()
                    : $menuSort.outerWidth())
                .show();
        }, 700, $(this));
    });
    $(document).on('mouseenter', 'body > thead > tr > th.sortable', function () {
        clearTimeout(timer);
        if ($(".menu-sort:visible").length) {
            $(".menu-sort:visible").hide();
        }
        if ($('.ui-multiselect-close:visible').length) {
            $('.ui-multiselect-close:visible').click();
        }
        timer = setTimeout(function ($control) {   
            var dataName = $control.attr('data-name');
            $menuSort = $(".menu-sort[id='GridHeaderMenu__" + dataName+ "']");
            $menuSort.css('width', '');
            $menuSort
                .css('position', 'fixed')
                .css('top', $control.position().top + $control.outerHeight())
                .css('left', $control.position().left + $control.offsetParent().offset().left - window.pageXOffset)
                .outerWidth($control.outerWidth() > $menuSort.outerWidth()
                    ? $control.outerWidth()
                    : $menuSort.outerWidth())
                .show();
        }, 700, $(this));
    });
    $(document).on('mouseleave', 'th.sortable', function () {
        clearTimeout(timer);
    });
    $(document).on('mouseleave', '.menu-sort', function () {
        if (!$('.ui-multiselect-menu:visible').length) {
            $('.menu-sort:visible').hide();
        }
    });
    $(document).on('click', '.menu-sort > li.sort', function (e) {
        sort($($(this).parent().attr('data-target')), $(this).attr('data-order-type'));
        e.stopPropagation();
    });
    $(document).on('click', '.menu-sort > li.reset', function (e) {
        var $control = $(this);
        var $grid = $control.closest('.grid');
        var data = $p.getData($control);
        data.Direction = $grid.attr('data-name');
        data.TableId = $grid.attr('id');
        data.TableSiteId = $grid.attr('data-id');
        $('[data-id^="ViewSorters_"]').each(function () {
            delete data[$(this).attr('data-id')];
        });
        $p.send($('#ViewSorters_Reset'));
        e.stopPropagation();
    });
    $(document).on('click', 'th.sortable', function (e) {
        var $control = $(this).find('div');
        sort($control, $control.attr('data-order-type'));
        e.stopPropagation();
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
