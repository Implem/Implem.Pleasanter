//一覧画面とアクセス権タブで共通のソートメニュー表示処理。
//複数選択は開いたままだとメニューを隠しても残るため、先に閉じる。
//渡された要素自身と配下の両方を対象にするためaddBackを使う。
$p.closeMultiSelect = function ($control) {
    $control
        .find('select[multiple].applied')
        .addBack('select[multiple].applied')
        .each(function () {
            if ($(this).multiselect('isOpen')) {
                $(this).multiselect('close');
            }
        });
};

$p.hideSorterMenu = function ($menus) {
    if (!$menus.length) return;
    $p.closeMultiSelect($menus);
    $menus.hide();
};

//ヘッダの直下に、ヘッダとメニューのうち広い方の幅で配置する。
$p.sorterMenuCss = function ($control, $menu, responsive) {
    const width = Math.max($control.outerWidth(), $menu.outerWidth());
    //PCは画面に固定し、レスポンシブは表示領域からはみ出さない位置に置く。
    if (!responsive) {
        return {
            position: 'fixed',
            top: $control.offset().top + $control.outerHeight() - $(window).scrollTop(),
            left: $control.offset().left - $(window).scrollLeft(),
            width: width
        };
    }
    const stageWidth = $('#ViewModeContainer').width();
    const left = $control.offset().left;
    return {
        position: 'absolute',
        marginTop: $control.outerHeight() - 1,
        left: left + width > stageWidth ? stageWidth - width : left,
        width: width
    };
};

$p.showSorterMenu = function ($control, $menu, responsive) {
    $p.closeMultiSelect($('select[multiple].applied'));
    $p.hideSorterMenu($('.menu-sort:visible'));
    $menu.css('width', 'auto');
    $menu.css($p.sorterMenuCss($control, $menu, responsive)).show();
};
