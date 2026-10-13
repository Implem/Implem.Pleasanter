//ヘッダのメニューは一覧画面（gridevents.js）と同じ流れで開閉する。
$(function () {
    let showTimer;
    let hideTimer;
    const menuHide = function () {
        $p.hideSorterMenu($('#PermissionsViewerHeaderMenus .menu-sort:visible'));
    };
    const menuShow = function ($control) {
        menuHide();
        $p.showSorterMenu($control, permissionsViewerMenu($control.attr('data-name')), false);
    };
    $(document).on('mouseenter', '#FieldSetPermissionsViewer th.sortable', function () {
        clearTimeout(showTimer);
        clearTimeout(hideTimer);
        showTimer = setTimeout(
            function ($control) {
                menuShow($control);
            },
            700,
            $(this)
        );
    });
    $(document).on('mouseleave', '#FieldSetPermissionsViewer th.sortable', function () {
        clearTimeout(showTimer);
        clearTimeout(hideTimer);
        hideTimer = setTimeout(function () {
            menuHide();
        }, 1000);
    });
    $(document).on('mouseenter', '#PermissionsViewerHeaderMenus', function () {
        clearTimeout(showTimer);
        clearTimeout(hideTimer);
    });
    $(document).on('mouseleave', '#PermissionsViewerHeaderMenus', function () {
        menuHide();
    });
    $(window).scroll(function () {
        if (!$('#PermissionsViewerHeaderMenus').length) return false;
        menuHide();
    });
    $(document).on('click', '#FieldSetPermissionsViewer th.sortable', function (event) {
        const $control = $(this).find('div');
        menuHide();
        //次に適用する並び順はサーバが出力しているため、そのまま送る。
        sort($(this).attr('data-name'), $control.attr('data-order-type'));
        event.stopPropagation();
    });
    $(document).on(
        'click',
        '#PermissionsViewerHeaderMenus > .menu-sort > li.sort',
        function (event) {
            const $menu = $(this).parent();
            menuHide();
            sort(sorterKey($menu), $(this).attr('data-order-type'));
            event.stopPropagation();
        }
    );
    $(document).on(
        'click',
        '#PermissionsViewerHeaderMenus > .menu-sort > li.reset',
        function (event) {
            menuHide();
            $p.resetPermissionsViewerSorters();
            event.stopPropagation();
        }
    );
    //一覧画面と同じく、選択した列の条件を積み上げる。releaseはその列だけを解除する。
    function sort(key, orderType) {
        const sorters = permissionsViewerSorters().filter(function (o) {
            return o.Key !== key;
        });
        if (orderType && orderType !== 'release') {
            sorters.push({ Key: key, OrderType: orderType });
        }
        $('#PermissionsViewerSorters').val(JSON.stringify(sorters));
        sendPermissionsViewer();
    }
    //一覧画面のようにdata-targetを持たせると、一覧画面側のメニュー処理まで
    //反応してしまうため、メニューのIDの末尾から列名を取り出す。
    function sorterKey($menu) {
        return $menu.attr('id').split('__')[1];
    }
    function permissionsViewerMenu(key) {
        return $(".menu-sort[id='PermissionsViewerHeaderMenu__" + key + "']");
    }
});

//フィルタは一覧画面と同じくコントロールのIDから列名を取り出す。
$(function () {
    $(document).on('change', '#FieldSetPermissionsViewer .permissions-viewer-filter', function () {
        $p.setPermissionsViewerFilters();
    });
    //対象の展開は列の構成が変わるため、表を取得し直す。
    $(document).on('change', '#PermissionsViewerExpandTarget', function () {
        sendPermissionsViewer();
    });
    //開閉自体はexpand.jsが行う。タブを開き直しても状態が残るよう、変更を保存する。
    $(document).on('click', '#FieldSetPermissionsViewer .field-section.expand', function () {
        const $expand = $('#PermissionsViewerFilterExpand');
        $expand.val($expand.val() === '1' ? '0' : '1');
        sendPermissionsViewer();
    });
    //テキスト欄はEnterでも絞り込む。
    $(document).on(
        'keypress',
        '#FieldSetPermissionsViewer .permissions-viewer-filter',
        function (event) {
            if (event.which === 13) {
                event.preventDefault();
                $p.setPermissionsViewerFilters();
            }
        }
    );
    //テキスト欄のblurで絞り込みが走ると、読み込み中の表示にclickが吸われるため、
    //先に発生するmousedownで受け、フォーカスも移動させない。
    $(document).on('mousedown', '#ResetPermissionsViewerFilters', function (event) {
        event.preventDefault();
        $p.resetPermissionsViewerFilters();
    });
});

$p.setPermissionsViewerSorter = function (key, orderType) {
    const sorters = permissionsViewerSorters().filter(function (o) {
        return o.Key !== key;
    });
    if (orderType && orderType !== 'release') {
        sorters.push({ Key: key, OrderType: orderType });
    }
    $('#PermissionsViewerSorters').val(JSON.stringify(sorters));
    sendPermissionsViewer();
};

$p.resetPermissionsViewerSorters = function () {
    $('#PermissionsViewerSorters').val('[]');
    sendPermissionsViewer();
};

$p.setPermissionsViewerFilters = function () {
    const filters = [];
    $('#FieldSetPermissionsViewer .permissions-viewer-filter').each(function () {
        const $control = $(this);
        let value = $control.val();
        //複数選択は一覧画面のフィルタと同じくJSON配列で送る。
        if ($control.prop('multiple')) {
            value = value && value.length ? JSON.stringify(value) : '';
        }
        if (value) {
            filters.push({
                Key: $control.attr('id').split('__')[1],
                Value: value
            });
        }
    });
    //条件が変わっていなければ取得し直さない。
    const json = JSON.stringify(filters);
    if (json === $('#PermissionsViewerFilterConditions').val()) {
        return;
    }
    $('#PermissionsViewerFilterConditions').val(json);
    sendPermissionsViewer();
};

//絞り込み欄は再描画しないため、各コントロールも空に戻す。
$p.resetPermissionsViewerFilters = function () {
    $('#FieldSetPermissionsViewer .permissions-viewer-filter').each(function () {
        const $control = $(this);
        if ($control.prop('multiple')) {
            $p.selectMultiSelect($control, '[]');
        } else {
            $control.val('');
        }
    });
    $('#PermissionsViewerFilterConditions').val('[]');
    sendPermissionsViewer();
};

//ファイルのダウンロードはajaxで受け取れないため、一覧画面の出力と同じくフォームを組んで送る。
$p.exportPermissionsViewer = function () {
    const form = document.createElement('form');
    form.style.display = 'none';
    form.setAttribute(
        'action',
        $('.main-form').attr('action').replace('_action_', 'permissionsviewercsv')
    );
    form.setAttribute('method', 'post');
    const addInput = function (name, value) {
        const input = document.createElement('input');
        input.setAttribute('type', 'hidden');
        input.setAttribute('name', name);
        input.setAttribute('value', value);
        form.appendChild(input);
    };
    addInput('PermissionsViewerSorters', $('#PermissionsViewerSorters').val());
    addInput('PermissionsViewerFilterConditions', $('#PermissionsViewerFilterConditions').val());
    addInput(
        'PermissionsViewerExpandTarget',
        $('#PermissionsViewerExpandTarget').prop('checked'));
    addInput('PermissionsViewerExportEncoding', $('#PermissionsViewerExportEncoding').val());
    if ($('#Token').length) {
        addInput('Token', $('#Token').val());
    }
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
    $('#PermissionsViewerExportDialog').dialog('close');
};

//表示済みの行数を開始位置として、続きの行だけを取得する。
//タブを開き直したときは先頭から取得するよう、送信後に開始位置を戻す。
$p.loadNextPermissionsViewer = function () {
    $('#PermissionsViewerOffset').val($('#PermissionsViewerBody tr').length);
    $p.send($('#FieldSetPermissionsViewer'));
    $('#PermissionsViewerOffset').val(0);
};

function permissionsViewerSorters() {
    const value = $('#PermissionsViewerSorters').val();
    return value ? JSON.parse(value) : [];
}

//条件を変えたときは先頭から表示し直す。
function sendPermissionsViewer() {
    $('#PermissionsViewerOffset').val(0);
    $p.send($('#FieldSetPermissionsViewer'));
}
