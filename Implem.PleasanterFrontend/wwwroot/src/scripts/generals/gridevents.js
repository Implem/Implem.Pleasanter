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
                    var dataId = $(this).closest('.grid-row').attr('data-id');
                    if ($grid.hasClass('new-tab')) {
                        url = $('#BaseUrl').val() + dataId;
                        window.open(url, '_blank', 'noopener noreferrer');
                    } else {
                        var func = $grid.attr('data-func');
                        var dataVer = $(this).closest('.grid-row').attr('data-ver');
                        var dataHistory = $(this).closest('.grid-row').attr('data-history');
                        if (func) {
                            $p.getData($grid)[$grid.attr('data-name')] = dataId;
                            $p[func]($grid);
                        }
                        else {
                            var paramVer = dataHistory ? '?ver=' + dataVer : '';
                            if ($('#EditorDialog').length === 1) {
                                var data = {};
                                data.EditInDialog = true;
                                url = $('#BaseUrl').val() + dataId
                                    + paramVer;
                                $p.ajax(url, 'post', data);
                            } else {
                                var params = [];
                                var fromTabIndex = $grid.attr('from-tab-index');
                                if ($grid.attr('data-value') === 'back') {
                                    params.push('back=1');
                                }
                                if (fromTabIndex) {
                                    params.push('FromTabIndex=' + fromTabIndex);
                                }
                                $p.transition($('#BaseUrl').val() + dataId
                                    + paramVer
                                    + (params.length
                                        ? (paramVer ? '$' : '?') + params.join('&')
                                        : ''));
                            }
                        }
                    }
                }
            }
        } else if (!$p.hoverd($control)) {
            $control.prop('checked', !$control.prop('checked'));
            $control.change();
        }
    });
});

$(function () {
    var showTimer;
    var hideTimer;
    var isRwd = $('head').css('font-family') === 'responsive';
    var spToggle = false;
    var filterHide = function () {
        if ($(".menu-sort:visible").length) $(".menu-sort:visible").hide();
        if ($('.ui-multiselect-close:visible').length) $('.ui-multiselect-close:visible').click();
        spToggle = false;
    }

    var filterShow = function ($control) {
        filterHide()
        var dataName = $control.attr('data-name');
        $menuSort = $(".menu-sort[id='GridHeaderMenu__" + dataName + "']");
        $menuSort.css('width', 'auto');

        var cssProps = (() => {
            if (!isRwd) {
                return {
                    position: 'fixed',
                    top: $control.offset().top + $control.outerHeight() - $(window).scrollTop(),
                    left: $control.offset().left - $(window).scrollLeft(),
                    width: Math.max($control.outerWidth(), $menuSort.outerWidth())
                }
            } else {
                var stageWidth = $('#ViewModeContainer').width();
                var cssWidth = Math.max($control.outerWidth(), $menuSort.outerWidth());
                var cssLeft = $control.offset().left;
                cssLeft = cssLeft + cssWidth > stageWidth ? stageWidth - cssWidth : cssLeft;
                return {
                    position: 'absolute',
                    marginTop: $control.outerHeight() - 1,
                    left: cssLeft,
                    width: cssWidth
                }
            }
        })();
        $menuSort.css(cssProps).show();
    }

    $(document).on('mouseenter', '#Grid th.sortable', function () {
        if (isRwd) return false;
        clearTimeout(showTimer);
        clearTimeout(hideTimer);
        showTimer = setTimeout(function ($control) {
            filterShow($control)
        }, 700, $(this));
    });

    $(document).on('mouseleave', '#Grid th.sortable', function () {
        if (isRwd) return false;
        clearTimeout(showTimer);
        clearTimeout(hideTimer);
        hideTimer = setTimeout(function () {
            filterHide();
        }, 1000);
    });

    $(document).on('mouseenter', '#GridHeaderMenus', function () {
        if (isRwd) return false;
        clearTimeout(showTimer);
        clearTimeout(hideTimer);
    });

    $(document).on('mouseleave', '#GridHeaderMenus', function () {
        if (isRwd) return false
        if ($('.ui-multiselect-menu:visible').length) return false;
        if ($('#GridHeaderMenus .menu-sort input:focus').length) return false;
        filterHide();
    });

    $(window).scroll(function () {
        if (!$('#GridHeaderMenus.length')) return false;
        filterHide();
    });

    if ($('#GridHeaderMenus .menu-sort input').length) {
        $('#GridHeaderMenus').find(".menu-sort input").on('blur', function () {
            if ($('#GridHeaderMenus:hover').length) return false;
            filterHide();
        });
        $('#GridHeaderMenus').find('.menu-sort input').on('keydown', function (e) {
            if (e.keyCode === 13) {
                setTimeout(function () {
                    filterHide();
                }, 300);
            }
        });
    }

    $(document).on('click', 'th.sortable', function (e) {
        if (!$('#GridHeaderMenus').length || !isRwd) {
            var $control = $(this).find('div');
            sort($control, $control.attr('data-order-type'));
        } else {
            if (!$(".menu-sort[id='GridHeaderMenu__" + $(this).attr('data-name') + "']:visible").length) {
                spToggle = true;
                filterShow($(this));
            } else {
                filterHide();
            }
        }
        e.stopPropagation();
    });

    // レスポンシブのみ
    document.addEventListener('touchstart', function (e) {
        if (!isRwd) return false
        if (e.target.closest('.ui-multiselect-menu')) {
            if($(".menu-sort:visible").length){
                setTimeout( () => {
                    filterHide();
                }, 100)
            }
            return false
        }
        if (!e.target.closest('#GridHeaderMenus')) {
            setTimeout(function () {
                if (spToggle) {
                    spToggle = false;
                } else {
                    filterHide();
                }
            });
        }
    });

    $('#ViewModeContainer .grid-wrap').scroll(function () {
        if (!$('#GridHeaderMenus.length')) return false;
        filterHide();
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
