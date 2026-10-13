$(function () {
    $(document).on('change', '[class^="control-"]:not(select[multiple])', function (e) {
        var $control = $(this);
        if ($control.hasClass('control-spinner')) {
            if ($control.val() === '' && $control.hasClass('allow-blank')) {
                $control.val('');
            } else if (
                $control.val() === '' ||
                $control.val().match(/[^-|^0-9.]/g) ||
                parseInt($control.val()) < parseInt($control.attr('data-min'))
            ) {
                $control.val($control.attr('data-min'));
            } else if (parseInt($control.val()) > parseInt($control.attr('data-max'))) {
                $control.val($control.attr('data-max'));
            }
        }
        $p.setData($control);
        e.preventDefault();
    });
    // 空のoptionは選択肢としてのブランクではなく、値が空という状態の表示。
    // optionを差し替えるレスポンスは直後にchangeを発生させること
    // (callbackRelatingColumn、SelectSearchDropDownResponseのTrigger)
    $(document).on('change', 'select[data-without-blank="1"]', function () {
        var $control = $(this);
        var multiple = $control.attr('multiple') !== undefined;
        // 複数選択は未選択＝空のため空のoptionが不要。valも空配列(真)を返し判定に使えない
        if (!multiple && $control.val()) {
            $control.find('option[value=""]').remove();
        }
        // 検索機能を使う列は以降のdisabled判定のみ対象外。選択肢をダイアログで
        // 取得するため0件が常態で、disabledにするとダイアログも開けなくなる。
        // 空のoptionはダイアログのレスポンスが必ず返すため削除は検索列でも必要
        if ($control.hasClass('search')) {
            return;
        }
        // 空のoptionは選択肢ではないため数えない
        var noChoices =
            $control.find('option').filter(function () {
                return this.value !== '';
            }).length === 0;
        // 選択肢の有無に合わせて切り替える(EditorReadOnlyによる読取専用とは別物)
        if ($control.prop('disabled') !== noChoices) {
            if (multiple) {
                // 操作対象はウィジェットのボタンとメニュー。
                // disableが付けたinputの属性をrefreshが消すため必ずrefreshの後に呼ぶ
                $control.multiselect('refresh');
                $control.multiselect(noChoices ? 'disable' : 'enable');
            } else {
                $control.prop('disabled', noChoices);
            }
        }
    });
    $(document).on('spin', '.control-spinner', function (event, ui) {
        var $control = $(this);
        var data = $p.getData($control);
        data[this.id] = ui.value;
        $p.setGridTimestamp($control, data);
    });
    $(document).on('change', '.control-checkbox.visible', function () {
        show(this.id.substring(7, this.id.length), $(this).prop('checked'));

        function show(selector, value) {
            if (value) {
                $(selector).show();
            } else {
                $(selector).hide();
            }
        }
    });
    $(document).on('change', '.auto-postback:not([type="text"],select[multiple])', function () {
        $p.send($(this));
    });
    $(document).on('change', '.control-auto-postback:not(select[multiple])', function () {
        $p.controlAutoPostBack($(this));
    });
    $(document).on('change', 'select.control-auto-postback[multiple].search.applied', function () {
        $p.controlAutoPostBack($(this));
    });
    $(document).on('change', '.datepicker.auto-postback', function () {
        $p.send($(this));
    });
    $(document).on('keyup', '.auto-postback[type="text"]', function (e) {
        var $control = $(this);
        $p.setData($control);
        if (e.keyCode === 13) {
            $p.debounce(function () {
                $p.send($control);
                delete $p.getData($control)[$control.attr('id')];
            }, 500);
        }
    });

    $p.controlAutoPostBack = function ($control) {
        if ($p.disableAutPostback) return;
        var queryParams = new URLSearchParams(location.search);
        if (queryParams.has('ver')) return;
        var fieldSetTab = $('li[role="tab"][aria-selected=true][aria-controls^=FieldSetTab]');
        var selectedTabIndex = fieldSetTab.parent().children().index(fieldSetTab);
        var $form = $('#MainForm');
        var id = $p.id();
        var action = $p.action();
        // ダイアログ編集の場合にはフォームとidとactionを差し替え
        if ($control.closest('#DialogEditorForm').length) {
            $form = $('#DialogEditorForm');
            id = $('#EditorInDialogRecordId').val();
            action = 'edit';
        }
        var url =
            $('#BaseUrl').val() +
            id +
            '/' +
            action +
            '/?control-auto-postback=1&TabIndex=' +
            selectedTabIndex;
        var data = $p.getData($form);
        $p.setMustData($form);
        data.ControlId = $control.attr('id');
        data.ReplaceFieldColumns = $('#ReplaceFieldColumns').val();
        return $p.ajax(
            url,
            'post',
            data,
            $control,
            false,
            !$control.hasClass('not-set-form-changed')
        );
    };
});
