$p.moveCrosstab = function (type) {
    var $control = $('#CrosstabMonth');
    $control.val($('#Crosstab' + type).val());
    $p.setData($control);
    $p.send($control);
}

$p.setCrosstab = function () {
    $('#CrosstabColumnsField').toggle($('#CrosstabGroupByY').val() === 'Columns');
    $('#CrosstabValueField').toggle(
        $('#CrosstabGroupByY').val() !== 'Columns' &&
        $('#CrosstabAggregateType').val() !== 'Count');
    var date = $('#CrosstabXType').val() === 'datetime';
    $('#CrosstabTimePeriodField').toggle(date);
    $('#CrosstabMonth').toggle(date);
    $('#CrosstabPreviousButton').toggle(date);
    $('#CrosstabNextButton').toggle(date);
    $('#CrosstabThisMonthButton').toggle(date);
}