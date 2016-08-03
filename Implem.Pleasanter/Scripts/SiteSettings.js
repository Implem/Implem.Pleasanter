$p.openColumnPropertiesDialog = function ($control) {
    var error = $p.send($control, undefined, false);
    if (error === 0) {
        $('#ColumnPropertiesDialog').dialog({
            modal: true,
            width: '90%',
            height: '500',
            appendTo: '.main-form'
        });
        $('#ColumnPropertiesDialog').css('height', 'auto');
    }
}

$p.uploadSiteImage = function ($control) {
    var data = new FormData();
    data.append('SiteImage', $('[id=\'SiteSettings,SiteImage\']').prop('files')[0]);
    $p.upload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        $control.attr('data-method'),
        data);
}

$p.setAggregationDetails = function ($control) {
    var data = $p.getData();
    data.AggregationType = $('#AggregationType').val();
    data.AggregationTarget = $('#AggregationTarget').val();
    $('.ui-dialog-content').dialog('close');
    $p.send($control);
}

$p.addSummary = function ($control) {
    $p.setData($('#SummarySiteId'));
    $p.setData($('#SummaryDestinationColumn'));
    $p.setData($('#SummaryLinkColumn'));
    $p.setData($('#SummaryType'));
    $p.setData($('#SummarySourceColumn'));
    $p.send($control);
}

$(function () {
    $(document).on('selectableselected', '#EditorColumns', function () {
        $p.clearData('ColumnProperty,', $p.getData(), 'startsWith');
    });
    $(document).on('change', '[id="ColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="ColumnPropertyField,Min"]').toggle(visibility);
        $('[id="ColumnPropertyField,Max"]').toggle(visibility);
        $('[id="ColumnPropertyField,Step"]').toggle(visibility);
    });
    $(document).on('change', '[id="ColumnProperty,FormatSelector"]', function () {
        switch ($(this).val()) {
            case '\t':
                $('[id="ColumnPropertyField,Format"]').toggle(true);
                $('[id="ColumnProperty,Format"]').val('');
                break;
            default:
                $('[id="ColumnPropertyField,Format"]').toggle(false);
                $('[id="ColumnProperty,Format"]').val($(this).val());
                break;
        }
        $p.getData().ColumnProperty, Format = $('[id="ColumnProperty,Format"]').val();
    });
});