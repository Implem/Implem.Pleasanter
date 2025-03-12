$(function () {
    $p.apply();
});
$p.apply = function () {
    $('.menu').menu();
    $('.grid-header-filter').each(function (index, element) {
        $(element).removeClass('ui-menu-item');
        $($(element).children()[0]).removeClass('ui-menu-item-wrapper');
    });
    $('.tab-container:not(.applied)').tabs({
        beforeActivate: function (event, ui) {
            if (ui.newPanel.attr('data-action')) {
                $p.send(ui.newPanel);
            }
        },
        active: $('#EditorTabsContainer').attr("tab-active"),
        items: "> ul > li:not(.ignore-tab)"
    }).addClass('applied')
    .each(function () {
        if ($(this).attr('id') === 'EditorTabsContainer'
            && $('#SimpleModeEnabled').length > 0) {
            SimpleMode.init();
        }
    });
    $('.button-icon:not(.applied)').each(function () {
        var $control = $(this);
        var icon = $control.attr('data-icon');
        $control.button({ icon: icon });
    }).addClass('applied');
    $('.button-icon.hidden').toggle(false);
    $('select[multiple]:not(.applied)').multiselect({
        buttonWidth: 'auto',
        menuWidth: 225,
        selectedList: 100,
        linkInfo: {
            checkAll: {
                text: $p.display('CheckAll'),
                title: $p.display('CheckAll')
            },
            uncheckAll: {
                text: $p.display('UncheckAll'),
                title: $p.display('UncheckAll')
            }
        },
        noneSelectedText: '',
        beforeopen: function () {
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
    $('.field-normal .ui-widget.ui-state-default.ui-multiselect').css('width', '100%').removeAttr('title');
    $('.field-wide .ui-widget.ui-state-default.ui-multiselect').css('width', '100%').removeAttr('title');
    $('.field-auto-thin .ui-widget.ui-state-default.ui-multiselect').css('width', '140px').removeAttr('title');
    $('.datepicker:not(.applied)').each(function () {
        var $control = $(this);
        var $step = parseInt($control.attr('data-step'), 10);
        $control.datetimepicker({
            format: $control.attr('data-format'),
            timepicker: $control.attr('data-timepicker') === '1',
            step: isNaN($step) ? 10 : $step,
            dayOfWeekStart: 1,
            scrollInput: false
        })
            .addClass('applied');
    });
    switch ($('#Language').val()) {
        case 'ja':
            $.datetimepicker.setLocale('ja');
            break;
    }
    $('.radio:not(.applied)').buttonset().addClass('applied');
    $('.control-selectable:not(.applied)').selectable({
        selected: function (event, ui) {
            if ($(this).hasClass('single')) {
                $(ui.selected)
                    .addClass("ui-selected")
                    .siblings()
                    .removeClass("ui-selected")
                    .each(function (key, value) {
                        $(value).find('*').removeClass("ui-selected");
                    });
            }
        },
        filter: ".ui-widget-content",
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
            step: $control.attr('data-step'),
            spin: function (event, ui) {
                if ($(this).hasClass('control-auto-postback')) {
                    $p.controlAutoPostBack($(this));
                }
            }
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
        var id = $control.attr('id');
        var enableLightBox = Boolean($control.data('enablelightbox'));
        var $viewer = $('[id="' + id + '.viewer"]');
        var markup = $p.markup($control.val(), enableLightBox);
        $viewer.html($p.setInputGuide(id, $control.val(), markup));
        $control.addClass('applied');
        $p.setTargetBlank();
    });
    $('.markup:not(.applied)').each(function () {
        var $control = $(this);
        var id = $control.attr('id');
        var enableLightBox = Boolean($control.data('enablelightbox'));
        var dataId = $control.closest('.grid-row').data('id');
        var markup = $p.markup($control.html(), enableLightBox, true, dataId);
        $control.html($p.setInputGuide(id, $control.html(), markup));
        $control.addClass('applied');
        $p.setTargetBlank();
    });
    if ($('#Publish').length === 1) {
        $('a').each(function () {
            var $control = $(this);
            if ($control.attr('href')
                && $control.attr('href').indexOf('/binaries/') !== -1) {
                $control.attr('href', $control.attr('href').replace('/binaries/', '/publishbinaries/'))
            }
        });
        $('img').each(function () {
            var $control = $(this);
            if ($control.attr('src')
                && $control.attr('src').indexOf('/binaries/') !== -1) {
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
        $menu.css('position', 'fixed')
            .css('top', $header.offset().top + $header.outerHeight())
            .css('left', $header.offset().left)
            .outerWidth($header.outerWidth() > $menuSort.outerWidth()
                ? $header.outerWidth()
                : $menuSort.outerWidth());
    }
    var $multiSelect = $('.ui-multiselect-menu:visible');
    var $control = $("[id='ViewFiltersOnGridHeader__" + dataName + "_ms']");
    if ($multiSelect.length && $control.length) {
        $multiSelect
            .css('top', $control.offset().top + $control.outerHeight())
            .css('left', $control.offset().left);
    }
}


// シンプルモード管理オブジェクト（即時実行関数式でカプセル化）
const SimpleMode = (function () {
    'use strict';

    // プライベート変数
    // 設定値（初期化時に設定）
    let _enabled = false;
    let _default = false;
    let _displaySwitch = false;
    let _simpleModeTabNames = [];

    // 処理中フラグ（再帰呼び出し防止用）
    let _processing = false;

    // 初期化済みフラグ
    let _initialized = false;

    // シンプルモード切替中フラグ - onbeforeunloadイベント制御用
    let _switchingMode = false;

    // 元のonbeforeunloadイベントハンドラを保存するための変数
    let _originalBeforeUnload = null;

    // タブIDとタブ名のマッピング
    const _tabMapping = {
        'General': '#FieldSetGeneral',
        'Guide': '#GuideEditor',
        'SiteImageSettingsEditor': '#SiteImageSettingsEditor',
        'Styles': '#StylesSettingsEditor',
        'Scripts': '#ScriptsSettingsEditor',
        'Html': '#HtmlsSettingsEditor',
        'DashboardParts': '#DashboardPartSettingsEditor',
        'DataView': '#ViewsSettingsEditor',
        'Notifications': '#NotificationsSettingsEditor',
        'Mail': '#MailSettingsEditor',
        'Publish': '#PublishSettingsEditor',
        'Grid': '#GridSettingsEditor',
        'Filters': '#FiltersSettingsEditor',
        'Aggregations': '#AggregationsSettingsEditor',
        'Editor': '#EditorSettingsEditor',
        'Links': '#LinksSettingsEditor',
        'Histories': '#HistoriesSettingsEditor',
        'Move': '#MoveSettingsEditor',
        'Summaries': '#SummariesSettingsEditor',
        'Formulas': '#FormulasSettingsEditor',
        'Processes': '#ProcessesSettingsEditor',
        'StatusControls': '#StatusControlsSettingsEditor',
        'Reminders': '#RemindersSettingsEditor',
        'Import': '#ImportsSettingsEditor',
        'Export': '#ExportsSettingsEditor',
        'Calendar': '#CalendarSettingsEditor',
        'Crosstab': '#CrosstabSettingsEditor',
        'Gantt': '#GanttSettingsEditor',
        'BurnDown': '#BurnDownSettingsEditor',
        'TimeSeries': '#TimeSeriesSettingsEditor',
        'Analy': '#AnalySettingsEditor',
        'Kanban': '#KambanSettingsEditor',
        'ImageLib': '#ImageLibSettingsEditor',
        'Search': '#SearchSettingsEditor',
        'SiteIntegration': '#SiteIntegrationEditor',
        'ServerScript': '#ServerScriptsSettingsEditor',
        'SiteAccessControl': '#FieldSetSiteAccessControl',
        'RecordAccessControl': '#FieldSetRecordAccessControl',
        'ColumnAccessControl': '#FieldSetColumnAccessControl',
        'ChangeHistoryList': '#FieldSetHistories'
    };

    /**
     * シンプルモードを適用する
     * @private
     */
    const _applySimpleMode = function () {
        // シンプルモード機能が無効の場合は何もしない
        if (!_enabled) {
            $('#SimpleModeToggleContainer').hide();
            return;
        }

        // 有効なタブIDが無い場合には処理しない
        const validTabKeys = Object.keys(_tabMapping);
        const hasValidTab = _simpleModeTabNames.some(tab => validTabKeys.includes(tab));
        if (!hasValidTab) {
            $('#SimpleModeToggleContainer').hide();
            return;
        }

        // タブが初期化されていない場合は何もしない
        if (!$('#EditorTabsContainer').hasClass('ui-tabs')) return;

        // ignore-tab クラスではないタブ要素を取得
        const $tabs = $('#EditorTabs > li:not(.ignore-tab)');
        let firstVisibleTabIndex = -1;

        // 現在選択されているタブを取得
        const currentActiveIndex = $('#EditorTabsContainer').tabs('option', 'active');
        let currentTabVisible = false;

        // すべてのタブを一旦非表示にする（ignore-tab クラスのタブは除外）
        $tabs.hide();

        // シンプルモードで表示するタブだけを表示する
        _simpleModeTabNames.forEach(function (tabName) {
            const tabSelector = _tabMapping[tabName];

            if (tabSelector) {
                const $tab = $('#EditorTabs > li > a[href="' + tabSelector + '"]').parent();

                if ($tab.length) {
                    $tab.show();

                    // 現在選択中のタブが表示対象かどうか確認
                    if ($tabs.index($tab) === currentActiveIndex) {
                        currentTabVisible = true;
                    }

                    // 最初に表示されるタブのインデックスを記録
                    if (firstVisibleTabIndex === -1) {
                        // ignore-tabを除外したタブリスト内でのインデックスを取得
                        firstVisibleTabIndex = $tabs.index($tab);
                    }
                }
            }
        });

        // 現在表示中のタブが非表示になる場合は、表示対象の最初のタブを選択
        if (!currentTabVisible && firstVisibleTabIndex !== -1) {
            $('#EditorTabsContainer').tabs('option', 'active', firstVisibleTabIndex);
        }
    };

    /**
     * ノーマルモードを適用する
     * @private
     */
    const _applyNormalMode = function () {
        // タブが初期化されていない場合は何もしない
        if (!$('#EditorTabsContainer').hasClass('ui-tabs')) return;

        // すべてのタブを表示（ignore-tab クラスのタブは除外）
        $('#EditorTabs > li:not(.ignore-tab)').show();
    };

    /**
     * モード切替時のチェックボックス制御を行う
     * @param {boolean} checked チェックボックスのチェック状態
     * @private
     */
    const _toggleWithSafetyCheck = function (checked) {
        if (_processing) return;

        _processing = true;

        // チェックボックスを一時的に無効化して状態変更を防止
        $('#SimpleModeToggle').prop('disabled', true);

        // チェックボックスの状態を保証
        $('#SimpleModeToggle').prop('checked', checked);

        // モード切替
        if (checked) {
            _applySimpleMode();
        } else {
            _applyNormalMode();
        }

        // 処理完了後に再度有効化
        setTimeout(function () {
            $('#SimpleModeToggle').prop('disabled', false);
            _processing = false;
        }, 300); // 処理時間に余裕を持たせるため300msに延長
    };

    /**
     * 現在の状態でシンプルモードとノーマルモードに違いがあるかチェックする
     * @returns {boolean} 違いがあればtrue、なければfalse
     * @private
     */
    const _hasDifferenceBetweenModes = function () {
        // タブが初期化されていない場合は判定できないのでfalse
        if (!$('#EditorTabsContainer').hasClass('ui-tabs')) return false;

        // 表示されているタブを取得（ignore-tab クラスのタブは除外）
        const $tabs = $('#EditorTabs > li:not(.ignore-tab)');

        // 実際に表示されるシンプルモードのタブを数える
        let simpleModelVisibleTabCount = 0;
        const simpleModeTabs = new Set();

        _simpleModeTabNames.forEach(function (tabName) {
            const tabSelector = _tabMapping[tabName];
            if (tabSelector) {
                const $tab = $('#EditorTabs > li > a[href="' + tabSelector + '"]').parent();
                if ($tab.length && !$tab.hasClass('ignore-tab')) {
                    simpleModelVisibleTabCount++;
                    simpleModeTabs.add($tab[0]);
                }
            }
        });

        // タブ数が異なる場合は違いがある
        if (simpleModelVisibleTabCount !== $tabs.length) {
            return true;
        }

        // タブ数が同じ場合、全く同じタブかどうかをチェック
        let allTabsIncluded = true;
        $tabs.each(function () {
            if (!simpleModeTabs.has(this)) {
                allTabsIncluded = false;
                return false; 
            }
        });

        return !allTabsIncluded;
    };

    // パブリックメソッド
    return {
        /**
         * 初期化処理
         * @public
         */
        init: function () {
            // 既に初期化済みの場合は処理しない
            if (_initialized) return;

            // 設定値を取得
            _enabled = $('#SimpleModeEnabled').length > 0 && $('#SimpleModeEnabled').val() === 'true';
            _default = $('#SimpleModeDefault').length > 0 && $('#SimpleModeDefault').val() === 'true';
            _displaySwitch = $('#SimpleModeDisplaySwitch').length > 0 && $('#SimpleModeDisplaySwitch').val() === 'true';
            _simpleModeTabNames = $('#SimpleModeTabs').length > 0 ? $('#SimpleModeTabs').val().split(',') : [];

            // シンプルモード機能が無効の場合は何もしない
            if (!_enabled) {
                // シンプルモード切り替えUI自体を非表示
                $('#SimpleModeToggleContainer').hide();
                return;
            }

            // シンプルモードとノーマルモードに違いがあるかどうか確認（画面上での変更はないので、初期化時のみのチェックで）
            const hasDifference = _hasDifferenceBetweenModes();

            // 違いがない場合は切り替えUIを非表示
            if (!hasDifference) {
                $('#SimpleModeToggleContainer').hide();
                return;
            }

            // シンプルモード切り替えUIを表示
            $('#SimpleModeToggleContainer').toggle(_displaySwitch);

            // チェックボックスの初期状態を設定
            $('#SimpleModeToggle').prop('checked', _default);

            // チェックボックスの変更イベントを登録
            $('#SimpleModeToggle').on('change', function () {
                _toggleWithSafetyCheck($(this).prop('checked'));
            });

            // 初期化完了フラグを設定
            _initialized = true;

            // デフォルトがシンプルモードならすぐに適用
            if (_default) {
                _applySimpleMode();
            }
        }

    };
})();