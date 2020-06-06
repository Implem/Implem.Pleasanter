$(function () {
    $p.apply = function () {
        $('.menu').menu();
        $('#EditorTabsContainer:not(.applied),#MailEditorTabsContainer:not(.applied),#ViewTabsContainer:not(.applied),#ColumnAccessControlTabsContainer:not(.applied),#EditorDetailTabsContainer:not(.applied)').tabs({
            beforeActivate: function (event, ui) {
                if (ui.newPanel.attr('data-action')) {
                    $p.send(ui.newPanel);
                }
            },
            active: $('#EditorTabsContainer').attr("tab-active")
        }).addClass('applied');
        $('.button-icon:not(.applied)').each(function () {
            var $control = $(this);
            var icon = $control.attr('data-icon');
            $control.button({ icon: icon });
        }).addClass('applied');
        $('.button-icon.hidden').toggle(false);
        $('select[multiple]:not(.applied)').multiselect({
            selectedList: 100,
            checkAllText: $p.display('CheckAll'),
            uncheckAllText: $p.display('UncheckAll'),
            noneSelectedText: '',
            beforeopen: function (){
                if ($(this).hasClass('search')) {
                    $p.openDropDownSearchDialog($(this));
                    return false;
                }
            },
            click: function () {
                $p.changeMultiSelect($(this))
            },
            checkAll: function () {
                $p.changeMultiSelect($(this))
            },
            uncheckAll: function () {
                $p.changeMultiSelect($(this))
            }
        }).addClass('applied');
        $('.datepicker:not(.applied)').each(function () {
            var $control = $(this);
            $control.datetimepicker({
                format: $control.attr('data-format'),
                timepicker: $control.attr('data-timepicker') === '1',
                step: 10,
                dayOfWeekStart: 1,
                scrollInput: false
            }).addClass('applied');
        });
        switch ($('#Language').val()) {
            case 'ja':
                $.datetimepicker.setLocale('ja');
                break;
        }
        $('.radio:not(.applied)').buttonset().addClass('applied');
        $('.control-selectable:not(.applied)').selectable({
            selected: function (event, ui) {
                if ($(this).hasClass('single')){
                    $(ui.selected)
                        .addClass("ui-selected")
                        .siblings()
                        .removeClass("ui-selected")
                        .each(function (key, value) {
                            $(value).find('*').removeClass("ui-selected");
                        });
                }
            },
            stop: function () {
                $p.setData($(this));
            }
        }).addClass('applied');
        $('.control-slider-ui').each(function () {
            var $control = $('#' + $(this).attr('id').split(',')[0]);
            $(this).slider({
                min: parseFloat($(this).attr('data-min')),
                max: parseFloat($(this).attr('data-max')),
                step: parseFloat($(this).attr('data-step')),
                value: parseFloat($control.text()),
                slide: function (event, ui) {
                    $control.text(ui.value);
                    $control.attr('data-value', ui.value);
                    $p.setData($control);
                },
                stop: function (event, ui) {
                    if ($control.hasClass('auto-postback')) {
                        $p.send($control);
                    }
                }
            });
        });
        $('.control-spinner:not(.applied)').each(function () {
            var $control = $(this);
            $control.spinner({
                min: $control.attr('data-min'),
                max: $control.attr('data-max'),
                step: $control.attr('data-step')
            }).css('width', function () {
                return $control.attr('data-width');
            });
            $control.addClass('applied');
        });
        $('[class*="enclosed"] .legend:not(.applied)').each(function (e) {
            var $control = $(this);
            if ($control.find('[class^="ui-icon ui-icon-triangle-1-"]').length === 0) {
                $control.prepend($('<span/>').addClass('ui-icon ui-icon-triangle-1-s'));
            }
            $control.addClass('applied');
        });
        $('.control-dropdown:not(.applied)').each(function () {
            var $control = $(this);
            var selectedCss = $control.find('option:selected').attr('data-class');
            if (selectedCss !== undefined) {
                $control
                    .addClass(selectedCss)
                    .addClass('applied');
            }
        });
        $('.control-markdown:not(.applied)').each(function () {
            var $control = $(this);
            var $viewer = $('[id="' + this.id + '.viewer"]');
            $viewer.html($p.markup($control.val()));
            $control.addClass('applied');
        });
        $('.markup:not(.applied)').each(function () {
            var $control = $(this);
            $control.html($p.markup($control.html(), true));
            $control.addClass('applied');
        });
        if ($('#Publish').length === 1) {
            $('a').each(function () {
                var $control = $(this);
                if ($control.attr('href').indexOf('/binaries/') !== -1) {
                    $control.attr('href', $control.attr('href').replace('/binaries/', '/publishbinaries/'))
                }
            });
            $('img').each(function () {
                var $control = $(this);
                if ($control.attr('src').indexOf('/binaries/') !== -1) {
                    $control.attr('src', $control.attr('src').replace('/binaries/', '/publishbinaries/'))
                }
            });
        }
        replaceMenu();
    };
    function replaceMenu() {
        var $header;
        var $menu = $('[id^=GridHeaderMenu__]:visible');
        if (!$menu.length) {
            return;
        }
        var dataName = $menu.attr('id').replace('GridHeaderMenu__', '');
        $header = $("body > thead:visible > tr > th.sortable[data-name='" + dataName + "']");
        if ($header.length) {
            if ($(".menu-sort:visible").length) {
                $(".menu-sort:visible").hide();
            }
            if ($('.ui-multiselect-close:visible').length) {
                $('.ui-multiselect-close:visible').click();
            }
        } else {
            $header = $("table > thead > tr > th.sortable[data-name='" + dataName + "']");
            if (!$header.length) {
                return;
            }
            $menu.css('width', '');
            $menu.css('position', 'absolute')
                .css('top', $header.position().top + $header.outerHeight())
                .css('left', $header.position().left)
                .outerWidth($header.outerWidth() > $menuSort.outerWidth()
                    ? $header.outerWidth()
                    : $menuSort.outerWidth());
        }
        
        var $multiSelect = $('.ui-multiselect-menu:visible');
        var $control = $("[id='ViewFiltersOnGridHeader__" + dataName + "_ms']");
        if ($multiSelect.length && $control.length) {
            $multiSelect.css('top', $control.offset().top + $control.outerHeight())
                .css('left', $control.offset().left);
        }
    }
    $p.apply();
});