var crosstabDrillDownReturnKey = 'CrosstabDrillDownReturn';

$(function () {
    $(document).on('click', '.crosstab-drill-down', function () {
        $p.crosstabDrillDown($(this));
    });
    $(document).on('keydown', '.crosstab-drill-down', function (e) {
        if (e.which !== 13 && e.which !== 32) {
            return;
        }
        e.preventDefault();
        $(this).trigger('click');
    });
    $p.setCrosstabDrillDownReturn();
});

$p.crosstabDrillDown = function ($control) {
    var url = $('#CrosstabDrillDownUrl').val();
    var json = $('#CrosstabDrillDownView').val();
    if (!url || !json) {
        return;
    }
    var view = JSON.parse(json);
    view.ColumnFilterHash = view.ColumnFilterHash || {};
    addFilter($control.attr('data-crosstab-x-column'), $control.attr('data-crosstab-x-filter'));
    addFilter($control.attr('data-crosstab-y-column'), $control.attr('data-crosstab-y-filter'));
    //ドリルダウンの絞り込みはセッションに保存されるため、
    //戻る操作で絞り込み前の状態に復元できるよう、遷移元のURLとビューを保持する。
    sessionStorage.setItem(
        crosstabDrillDownReturnKey,
        JSON.stringify({
            url: location.href.split('?')[0],
            view: $('#CrosstabReturnView').val()
        })
    );
    $p.transition(url + '?View=' + encodeURIComponent(JSON.stringify(view)));

    function addFilter(columnName, filter) {
        if (columnName && filter !== undefined) {
            view.ColumnFilterHash[columnName] = filter;
        }
    }
};

//ドリルダウンで遷移してきた場合に、戻る先を絞り込み前のクロス集計にする。
$p.setCrosstabDrillDownReturn = function () {
    var $backUrl = $('#BackUrl');
    if ($backUrl.length !== 1) {
        return;
    }
    var stored = sessionStorage.getItem(crosstabDrillDownReturnKey);
    sessionStorage.removeItem(crosstabDrillDownReturnKey);
    if (!stored || !new URL(location.href).searchParams.has('View')) {
        return;
    }
    var data = JSON.parse(stored);
    if (!data.url || !data.view) {
        return;
    }
    $backUrl.val(data.url + '?View=' + encodeURIComponent(data.view));
};
