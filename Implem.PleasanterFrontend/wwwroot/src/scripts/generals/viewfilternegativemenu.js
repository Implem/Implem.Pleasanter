$(function () {
    // フィルタ条件の否定指定メニュー。
    // 状態は既存の入力要素（プリセットのチェックボックスと
    // ViewFiltersNegative__* のチェックボックス）が持ち、
    // メニューはその読み書きだけを行う。
    var stateOf = function ($item) {
        return $item.attr('data-state') || 'positive';
    };

    var closeMenus = function ($except) {
        $('.view-filter-menu.open').each(function () {
            var $menu = $(this);
            if (!$except || $menu[0] !== $except[0]) {
                $menu.removeClass('open');
            }
        });
    };

    var markCurrent = function ($item) {
        var state = stateOf($item);
        $item.find('> .view-filter-menu > .view-filter-menu-item').each(function () {
            var $li = $(this);
            $li.toggleClass('current', $li.attr('data-value') === state);
        });
    };

    var openMenu = function ($item) {
        var $menu = $item.find('> .view-filter-menu');
        if (!$menu.length) {
            return;
        }
        if ($menu.hasClass('open')) {
            closeMenus();
            return;
        }
        closeMenus($menu);
        markCurrent($item);
        $menu.addClass('open');
    };

    var apply = function ($item, state) {
        var $target = $item.find('.view-filter-state-inputs input[type="checkbox"]');
        var $check = $item.hasClass('view-filter-chip') ? $target.eq(0) : null;
        var $negative = $item.hasClass('view-filter-chip') ? $target.eq(1) : $target.eq(0);
        if ($check && $check.length) {
            $p.set($check, state !== 'none');
        }
        if ($negative.length) {
            $p.set($negative, state === 'negative');
        }
        $item.attr('data-state', state);
        markCurrent($item);
        closeMenus();
        // 一覧画面では値の変更ごとに再取得する（auto-postback が付いている）
        var $postback = $target.filter('.auto-postback').eq(0);
        if ($postback.length) {
            $p.send($postback);
        }
    };

    $(document).on('click', '.view-filter-chip-trigger', function (e) {
        e.preventDefault();
        e.stopPropagation();
        openMenu($(this).closest('.view-filter-item'));
    });

    $(document).on('keydown', '.view-filter-chip-trigger', function (e) {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            openMenu($(this).closest('.view-filter-item'));
        }
    });

    $(document).on('click', '.view-filter-mark', function (e) {
        e.preventDefault();
        e.stopPropagation();
        openMenu($(this).closest('.view-filter-item'));
    });

    // ラベルもトリガーにする。label 要素の既定動作（入力欄へのフォーカス）は
    // ここでは抑止して、メニューの開閉に充てる。
    $(document).on('click', '.view-filter-item .field-label', function (e) {
        var $item = $(this).closest('.view-filter-item');
        if (!$item.find('> .view-filter-menu').length) {
            return;
        }
        e.preventDefault();
        e.stopPropagation();
        openMenu($item);
    });

    $(document).on('click', '.view-filter-menu-item', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var $li = $(this);
        apply($li.closest('.view-filter-item'), $li.attr('data-value'));
    });

    $(document).on('click', function () {
        closeMenus();
    });
});
