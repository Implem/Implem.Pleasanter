$p.moveCrosstab = function (type) {
    $control = $('#CrosstabMonth');
    $control.val($('#Crosstab' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCrosstab = function () {
    $('#CrosstabValueField').toggle($('#CrosstabAggregateType').val() !== 'Count');
    var date = $('#CrosstabXType').val() === 'datetime';
    $('#CrosstabTimePeriodField').toggle(date);
    $('#CrosstabMonth').toggle(date);
    $('#CrosstabPreviousButton').toggle(date);
    $('#CrosstabNextButton').toggle(date);
    $('#CrosstabThisMonthButton').toggle(date);
}