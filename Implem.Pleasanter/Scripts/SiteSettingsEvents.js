$(function () {
    $(document).on('selectableselected', '#EditorColumns', function () {
        $p.clearData('ColumnProperty,', $p.getData($(this)), 'startsWith');
    });
    $(document).on('change', '[id="ColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="ColumnPropertyField,Min"]').toggle(visibility);
        $('[id="ColumnPropertyField,Max"]').toggle(visibility);
        $('[id="ColumnPropertyField,Step"]').toggle(visibility);
    });
    $(document).on('change', '[id="ColumnProperty,FormatSelector"]', function () {
        var $control = $(this);
        switch ($control.val()) {
            case '\t':
                $('[id="ColumnPropertyField,Format"]').toggle(true);
                $('[id="ColumnProperty,Format"]').val('');
                break;
            default:
                $('[id="ColumnPropertyField,Format"]').toggle(false);
                $('[id="ColumnProperty,Format"]').val($control.val());
                break;
        }
        $p.getData($control).ColumnProperty, Format = $('[id="ColumnProperty,Format"]').val();
    });
    $(document).on('click', '#SummarySettings button.delete-summary', function () {
        var $control = $(this);
        $p.getData($control).DeleteSummaryId = $control.attr('data-id');
        $p.send($("#DeleteSummary"));
    });
    $(document).on('click', '#CreateNotification,#UpdateNotification', function () {
        var $control = $(this);
        $p.getData($control).NotificationType = $('#NotificationType').val();
        $p.send($control);
    });
    $(document).on('click', '#NotificationSettings .grid-row', function () {
        var $control = $(this);
        $p.getData($control).NotificationType = $control.attr('data-id');
        $p.openNotificationDialog($('#EditNotification'), '#NotificationDialog');
    });
    $(document).on('click', '#NotificationSettings .delete', function (e) {
        var $control = $(this);
        e.stopPropagation();
        $p.getData($control).NotificationType = $control.attr('data-id');
        $p.send($('#DeleteNotification'));
    });
});