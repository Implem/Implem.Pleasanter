$(function () {
    $p.apply = function () {
        $('.menu').menu();
        $('#EditorTabsContainer:not(.applied),#MailEditorTabsContainer:not(.applied)').tabs({
            beforeActivate: function (event, ui) {
                if (ui.newPanel.attr('data-action')) {
                    $p.send(ui.newPanel);
                }
            }
        }).addClass('applied');
        $('.button-icon:not(.applied)').each(function () {
            var $control = $(this);
            var icon = $control.attr('data-icon');
            $control.button({ icon: icon });
        }).addClass('applied');
        $('select[multiple]:not(.applied)').multiselect({
            selectedList: 100,
            checkAllText: $p.display('Displays_CheckAll'),
            uncheckAllText: $p.display('Displays_UncheckAll'),
            noneSelectedText: '',
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
        $('.datepicker:not(.applied)').datepicker({
            showButtonPanel: true,
            onSelect: function (date) {
                $p.getDataByInnerElement($(this))[this.id] = date;
            }
        }).addClass('applied');
        $('.radio:not(.applied)').buttonset().addClass('applied');
        $('.control-selectable:not(.applied)').selectable({
            stop: function () {
                $p.onSelectableSelected($(this));
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
                    $p.setData($control);
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
            if ($control.find('[class^="ui-icon ui-icon-carat-1-"]').length === 0) {
                $control.prepend($('<span/>').addClass('ui-icon ui-icon-carat-1-s'));
            }
            $control.addClass('applied');
        });
        $('.status:not(.applied)').each(function () {
            var $control = $(this);
            $control
                .addClass($control.find('option:selected').attr('data-class'))
                .addClass('applied');
        });
        $('.control-markdown:not(.applied)').each(function () {
            var $control = $(this);
            var $viewer = $('[id="' + this.id + '.viewer"]');
            $viewer.html($p.markup($control.val()));
            $p.resizeEditor($control, $viewer);
            $control.addClass('applied');
        });
        $('.markup:not(.applied)').each(function () {
            var $control = $(this);
            $control.html($p.markup($control.html(), true));
            $control.addClass('applied');
        });
    }
    $p.apply();
});